using TimeHollow.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace TimeHollow.Audio
{
    public sealed class GameAudio : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource sourceA;
        [SerializeField] private AudioSource sourceB;
        [SerializeField, Min(0f)] private float crossfadeSeconds = 1f;

        private const string MasterParameter = "MasterVolumeDb";
        private const string MusicParameter = "MusicVolumeDb";
        private bool masterErrorLogged;
        private bool musicErrorLogged;
        private bool started;
        private bool volumesApplied;
        private MusicTrack pendingTrack;
        private MusicTrack currentTrack;
        private MusicTrack outgoingTrack;
        private AudioSource incoming;
        private AudioSource outgoing;
        private float incomingStart;
        private float outgoingStart;
        private float fadeElapsed;
        private bool fading;

        public float MasterVolume { get; private set; }
        public float MusicVolume { get; private set; }

        private void Awake()
        {
            foreach (var audio in FindObjectsByType<GameAudio>(FindObjectsSortMode.None))
            {
                if (audio != this && audio.incoming != null)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            incoming = sourceA;
            outgoing = sourceB;
            DontDestroyOnLoad(gameObject);
            gameObject.name = "GameAudio";
            MasterVolume = AudioVolume.ReadMaster();
            MusicVolume = AudioVolume.ReadMusic();
        }

        private void Start()
        {
            started = true;
            ApplyVolumes();
        }

        public void SetMasterVolume(float value)
        {
            MasterVolume = AudioVolume.Normalize(value, AudioVolume.DefaultMaster);
            PlayerPrefs.SetFloat(AudioVolume.MasterKey, MasterVolume);
            if (volumesApplied)
                ApplyVolume(MasterParameter, MasterVolume, ref masterErrorLogged);
            else if (started)
                ApplyVolumes();
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = AudioVolume.Normalize(value, AudioVolume.DefaultMusic);
            PlayerPrefs.SetFloat(AudioVolume.MusicKey, MusicVolume);
            if (volumesApplied)
                ApplyVolume(MusicParameter, MusicVolume, ref musicErrorLogged);
            else if (started)
                ApplyVolumes();
        }

        public void SaveVolumes() => PlayerPrefs.Save();

        private void ApplyVolumes()
        {
            var masterApplied = ApplyVolume(MasterParameter, MasterVolume, ref masterErrorLogged);
            var musicApplied = ApplyVolume(MusicParameter, MusicVolume, ref musicErrorLogged);
            volumesApplied = masterApplied && musicApplied;
            if (volumesApplied && pendingTrack != null)
            {
                var track = pendingTrack;
                pendingTrack = null;
                PlayMusic(track);
            }
        }

        private bool ApplyVolume(string parameter, float value, ref bool errorLogged)
        {
            if (mixer != null && mixer.SetFloat(parameter, AudioVolume.LinearToDecibels(value)))
                return true;

            if (!errorLogged)
            {
                Debug.LogError($"Unable to set audio mixer parameter '{parameter}'.", this);
                errorLogged = true;
            }

            return false;
        }

        public void PlayMusic(MusicTrack track)
        {
            if (track == null || track.Clip == null)
            {
                Debug.LogError("Cannot play music without a track and audio clip.", this);
                return;
            }

            if (!volumesApplied)
            {
                pendingTrack = track;
                return;
            }

            if (track == currentTrack)
                return;

            if (track == outgoingTrack)
            {
                SwapSources();
            }
            else
            {
                // Reuse the quieter source if both are playing. Keep the louder one fading out.
                if (currentTrack != null && (outgoingTrack == null || incoming.volume >= outgoing.volume))
                    SwapSources();

                incoming.Stop();
                incoming.clip = track.Clip;
                incoming.volume = 0f;
                incoming.Play();
                currentTrack = track;
            }

            incomingStart = incoming.volume;
            outgoingStart = outgoing.volume;
            fadeElapsed = 0f;
            fading = true;
            if (crossfadeSeconds <= 0f)
                CompleteFade();
        }

        private void SwapSources()
        {
            (incoming, outgoing) = (outgoing, incoming);
            (currentTrack, outgoingTrack) = (outgoingTrack, currentTrack);
        }

        private void Update()
        {
            if (!fading)
                return;

            fadeElapsed += Time.unscaledDeltaTime;
            if (fadeElapsed >= crossfadeSeconds)
            {
                CompleteFade();
                return;
            }

            var fraction = fadeElapsed / crossfadeSeconds;
            incoming.volume = Mathf.Lerp(incomingStart, 1f, fraction);
            if (outgoingTrack != null)
                outgoing.volume = Mathf.Lerp(outgoingStart, 0f, fraction);
        }

        private void CompleteFade()
        {
            incoming.volume = 1f;
            outgoing.volume = 0f;
            outgoing.Stop();
            outgoing.clip = null;
            outgoingTrack = null;
            fading = false;
        }
    }
}
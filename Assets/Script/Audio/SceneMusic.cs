using TimeHollow.Audio;
using UnityEngine;

namespace TimeHollow.Audio
{
    public sealed class SceneMusic : MonoBehaviour
    {
        [SerializeField] private GameAudio audioPrefab;
        [SerializeField] private MusicTrack track;

        private void Awake()
        {
            var audio = FindAnyObjectByType<GameAudio>();
            if (audio == null)
                audio = Instantiate(audioPrefab);

            audio.PlayMusic(track);
        }
    }
}

using TimeHollow.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TimeHollow.Audio;

namespace TimeHollow.UI
{
    public sealed class VolumeSettingsPanel : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private TMP_Text masterValue;
        [SerializeField] private TMP_Text musicValue;

        private GameAudio gameAudio;
        private bool started;
        private bool missingAudioLogged;

        private void Start()
        {
            started = true;
            Bind();
        }

        private void OnEnable()
        {
            if (started)
                Bind();
        }

        private void Bind()
        {
            gameAudio = FindAnyObjectByType<GameAudio>();
            if (gameAudio == null)
            {
                if (!missingAudioLogged)
                {
                    Debug.LogError("Volume settings require GameAudio.", this);
                    missingAudioLogged = true;
                }
                return;
            }

            masterSlider.SetValueWithoutNotify(gameAudio.MasterVolume);
            musicSlider.SetValueWithoutNotify(gameAudio.MusicVolume);
            ShowPercent(masterValue, gameAudio.MasterVolume);
            ShowPercent(musicValue, gameAudio.MusicVolume);
            masterSlider.onValueChanged.AddListener(SetMaster);
            musicSlider.onValueChanged.AddListener(SetMusic);
        }

        private void OnDisable()
        {
            masterSlider.onValueChanged.RemoveListener(SetMaster);
            musicSlider.onValueChanged.RemoveListener(SetMusic);
            if (gameAudio != null)
                gameAudio.SaveVolumes();
            gameAudio = null;
        }

        private void SetMaster(float value)
        {
            if (gameAudio != null)
            {
                gameAudio.SetMasterVolume(value);
                ShowPercent(masterValue, gameAudio.MasterVolume);
            }
        }

        private void SetMusic(float value)
        {
            if (gameAudio != null)
            {
                gameAudio.SetMusicVolume(value);
                ShowPercent(musicValue, gameAudio.MusicVolume);
            }
        }

        private static void ShowPercent(TMP_Text label, float value) =>
            label.text = $"{Mathf.RoundToInt(value * 100f)}%";
    }
}

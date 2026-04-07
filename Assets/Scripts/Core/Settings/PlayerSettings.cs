using System;
using Helteix.Singletons.MonoSingletons;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Settings
{
    public class PlayerSettings : MonoSingleton<PlayerSettings>
    {
        [field: SerializeField] public bool SubtitlesEnabled { get; private set; } = true;
        [field : SerializeField] public float MouseSensitivity { get; private set; }
        [field : SerializeField] public float MasterVolume { get; private set; }

        public event Action<bool> OnSubtitlesChanged;
        public event Action<float> OnSensitivityChanged;
        public event Action<float> OnVolumeChanged;

        private void Awake()
        {
            LoadSettings();
        }

        public void SetSubtitles(bool enabled)
        {
            SubtitlesEnabled = enabled;
            OnSubtitlesChanged?.Invoke(enabled);
            SaveSettings();
        }

        public void SetSensitivity(float value)
        {
            MouseSensitivity = Mathf.Clamp(value, 0.1f, 10f);
            OnSensitivityChanged?.Invoke(MouseSensitivity);
            SaveSettings();
        }

        public void SetMasterVolume(float value)
        {
            MasterVolume = Mathf.Clamp01(value);
            OnVolumeChanged?.Invoke(MasterVolume);
            SaveSettings();
        }
        

        private void LoadSettings()
        {
            SubtitlesEnabled = PlayerPrefs.GetInt("SubtitlesEnabled", 1) == 1;
            MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 3f);
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        }
        private void SaveSettings()
        {
            PlayerPrefs.SetInt  ("SubtitlesEnabled", SubtitlesEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("MouseSensitivity", MouseSensitivity);
            PlayerPrefs.SetFloat("MasterVolume",     MasterVolume);
            PlayerPrefs.Save();
        }

    }
}
using UnityEngine;

namespace TimeHollow.Audio
{
    public static class AudioVolume
    {
        public const string MasterKey = "TimeHollow.Audio.MasterVolume";
        public const string MusicKey = "TimeHollow.Audio.MusicVolume";
        public const float DefaultMaster = 1f;
        public const float DefaultMusic = 0.8f;

        public static float LinearToDecibels(float value)
        {
            value = Mathf.Clamp01(value);
            return value <= 0.0001f ? -80f : 20f * Mathf.Log10(value);
        }

        public static float ReadMaster() => Read(MasterKey, DefaultMaster);
        public static float ReadMusic() => Read(MusicKey, DefaultMusic);

        internal static float Normalize(float value, float fallback) =>
            float.IsNaN(value) ? fallback : Mathf.Clamp01(value);

        private static float Read(string key, float fallback) =>
            Normalize(PlayerPrefs.GetFloat(key, fallback), fallback);
    }
}


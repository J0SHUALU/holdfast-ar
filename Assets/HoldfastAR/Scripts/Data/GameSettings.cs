using System;
using UnityEngine;

namespace HoldfastAR.Data
{
    public static class GameSettings
    {
        public static readonly float[] VolumeSteps = { 0f, 0.5f, 1f };
        public static readonly string[] VolumeLabels = { "OFF", "LOW", "HIGH" };

        private const string SoundKey = "HoldfastAR.Settings.Sound";
        private const string MusicKey = "HoldfastAR.Settings.Music";
        private const string VibrationKey = "HoldfastAR.Settings.Vibration";
        private const string MarkersKey = "HoldfastAR.Settings.Markers";

        public static event Action Changed;

        public static int SoundLevel
        {
            get => Mathf.Clamp(PlayerPrefs.GetInt(SoundKey, 2), 0, VolumeSteps.Length - 1);
            set => Save(SoundKey, value);
        }

        public static int MusicLevel
        {
            get => Mathf.Clamp(PlayerPrefs.GetInt(MusicKey, 2), 0, VolumeSteps.Length - 1);
            set => Save(MusicKey, value);
        }

        public static bool Vibration
        {
            get => PlayerPrefs.GetInt(VibrationKey, 1) == 1;
            set => Save(VibrationKey, value ? 1 : 0);
        }

        public static bool SpawnMarkers
        {
            get => PlayerPrefs.GetInt(MarkersKey, 1) == 1;
            set => Save(MarkersKey, value ? 1 : 0);
        }

        public static float SoundVolume => VolumeSteps[SoundLevel];
        public static float MusicVolume => VolumeSteps[MusicLevel];

        private static void Save(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
            Changed?.Invoke();
        }
    }
}

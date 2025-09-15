using System.Collections.Generic;
using System.IO;
using System;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.UI
{
    /// <summary>
    /// Allows for Settings and Keybinds to be saved to and loaded from a file, settings.txt
    /// </summary>
	public class OptionsSaver : Singleton<OptionsSaver>
	{
		public static string SettingsFile => Application.persistentDataPath + "/settings.txt";

        /// <summary>
        /// Attempts to load the user's settings from settings.txt, returning Settings.Default if the file doesn't exist
        /// </summary>
        /// <returns></returns>
        public Settings LoadSettings()
        {
            if(!File.Exists(SettingsFile)) return Settings.Default.Clone();

            using StreamReader fileReader = new StreamReader(SettingsFile);
            string settingsString = fileReader.ReadToEnd();
            fileReader.Close();

            SerializableSettings serializable = JsonUtility.FromJson<SerializableSettings>(settingsString);
            return new Settings(
                serializable.resolution,
                serializable.fullScreenMode,
                serializable.maxFramerate,
                serializable.useVSync,
                serializable.uiScale,
                serializable.animationSpeed,

                serializable.quickPurchase,
                serializable.showDamageIndicators,
                serializable.togglePostProcessing,
                serializable.screenShakeStrength,
                serializable.rigorousness,

                serializable.masterVolume,
                serializable.musicVolume,
                serializable.soundVolume,
                serializable.enemyVolume,
                serializable.uiVolume,

                Keybindings.Generate(serializable.keybindPairs)
            );
        }

        /// <summary>
        /// Saves the current settings to the file, settings.txt, overwriting anything previously there
        /// </summary>
        public void SaveSettings()
        {
            Settings current = Options.Current;
            SerializableSettings serializable = new SerializableSettings(
                current.Resolution,
                current.FullScreenMode,
                current.MaxFramerate,
                current.UseVSync,
                current.UIScale,
                current.AnimationSpeed,

                current.QuickPurchase,
                current.ShowDamageIndicators,
                current.TogglePostProcessing,
                current.ScreenShakeStrength,
                current.Rigorousness,

                current.MasterVolume,
                current.MusicVolume,
                current.SoundVolume,
                current.EnemyVolume,
                current.UIVolume,

                Options.Keybinds.Serialise()
            );

            string settingsString = JsonUtility.ToJson(serializable, true);

            using StreamWriter fileWriter = new StreamWriter(SettingsFile, false);
            fileWriter.WriteLine(settingsString);
            fileWriter.Close();
        }

        /// <summary>
        /// Saves any settings before closing the application
        /// </summary>
        private void OnApplicationQuit() => SaveSettings();
	}

    /// <summary>
    /// A version of settings with plain serializable fields rather than properties
    /// </summary>
    [Serializable]
    public struct SerializableSettings
    {
        public Vector2Int resolution;
        public FullScreenMode fullScreenMode;
        public int maxFramerate;
        public bool useVSync;
        public float uiScale;
        public float animationSpeed;

        public bool quickPurchase;
        public bool showDamageIndicators;
        public bool togglePostProcessing;
        public float screenShakeStrength;
        public RigorousnessLevel rigorousness;

        public float masterVolume;
        public float musicVolume;
        public float soundVolume;
        public float enemyVolume;
        public float uiVolume;

        public List<KeybindPair> keybindPairs;

        //i really don't like how many parameters there are...
        public SerializableSettings(Vector2Int resolution, FullScreenMode fullScreenMode, int maxFramerate, bool useVSync, float uiScale, float animationSpeed, bool quickPurchase, bool showDamageIndicators, bool togglePostProcessing, float screenShakeStrength, RigorousnessLevel rigorousness, float masterVolume, float musicVolume, float soundVolume, float enemyVolume, float uiVolume, List<KeybindPair> keybindPairs)
        {
            this.resolution = resolution;
            this.fullScreenMode = fullScreenMode;
            this.maxFramerate = maxFramerate;
            this.useVSync = useVSync;
            this.uiScale = uiScale;
            this.animationSpeed = animationSpeed;

            this.quickPurchase = quickPurchase;
            this.showDamageIndicators = showDamageIndicators;
            this.togglePostProcessing = togglePostProcessing;
            this.screenShakeStrength = screenShakeStrength;
            this.rigorousness = rigorousness;

            this.masterVolume = masterVolume;
            this.musicVolume = musicVolume;
            this.soundVolume = soundVolume;
            this.enemyVolume = enemyVolume;
            this.uiVolume = uiVolume;

            this.keybindPairs = keybindPairs;
        }
    }

    /// <summary>
    /// A single serializable key-value pair keybinding
    /// </summary>
    [Serializable]
	public struct KeybindPair
	{
		public Key key;
		public KeyCode code;

		public KeybindPair(Key key, KeyCode code)
		{
			this.key = key;
			this.code = code;
		}
	}
}

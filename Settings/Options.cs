using System.Collections.Generic;
using System.Linq;
using Sigmoid.Audio;
using Sigmoid.Cameras;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.UI
{
    /// <summary>
    /// Holds all user preferences and keybinds
    /// </summary>
	public class Options : Singleton<Options>
	{
		private Settings current;
        private void Awake() => current = OptionsSaver.Instance.LoadSettings();

        public static Settings Current => Instance.current;
        public static Keybindings Keybinds => Current.Keybinds;
    }

    /// <summary>
    /// Contains properties for each of the available settings in the game
    /// </summary>
    public class Settings
    {
        private Vector2Int resolution;
        public Vector2Int Resolution
        {
            get => resolution;
            set
            {
                resolution = value;
                Screen.SetResolution(value.x, value.y, Screen.fullScreen);
            }
        }

        private FullScreenMode fullScreenMode;
        public FullScreenMode FullScreenMode
        {
            get => fullScreenMode;
            set => Screen.fullScreenMode = fullScreenMode = value;
        }

        private int maxFramerate;
        public int MaxFramerate
        {
            get => maxFramerate;
            set => Application.targetFrameRate = maxFramerate = value;
        }

        private bool useVSync;
        public bool UseVSync
        {
            get => useVSync;
            set => QualitySettings.vSyncCount = (useVSync = value) ? 1 : 0;
            //this is the most cursed looking thing ever :skull:
        }

        private float uiScale;
        public float UIScale
        {
            get => uiScale;
            set
            {
                uiScale = value;
                uiReferenceMultiplier = Mathf.Pow(1.5f, 1f - 0.02f * uiScale);
            }
        }

        private float uiReferenceMultiplier;
        public float UIReferenceMultiplier => uiReferenceMultiplier;

        private float animationSpeed;
        public float AnimationSpeed
        {
            get => animationSpeed;
            set
            {
                animationSpeed = value;
                animationTimeMultiplier = value == 100f ? 0f : Mathf.Pow(4f, 1f - 0.02f * AnimationSpeed);
            }
        }

        private float animationTimeMultiplier;
        public float AnimationTimeMultiplier => animationTimeMultiplier;

        public bool QuickPurchase { get; set; }
        public bool ShowDamageIndicators { get; set; }
        public bool TogglePostProcessing { get; set; }
        public float ScreenShakeStrength { get; set; }
        public RigorousnessLevel Rigorousness { get; set; }

        private float masterVolume;
        public float MasterVolume
        {
            get => masterVolume;
            set
            {
                masterVolume = value;
                AudioManager.Instance.Mixer.SetFloat("MasterVolume", AudioHelper.PercentageToDecibels(value));
            }
        }
        private float musicVolume;
        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = value;
                AudioManager.Instance.Mixer.SetFloat("MusicVolume", AudioHelper.PercentageToDecibels(value));
            }
        }
        private float soundVolume;
        public float SoundVolume
        {
            get => soundVolume;
            set
            {
                soundVolume = value;
                AudioManager.Instance.Mixer.SetFloat("SoundVolume", AudioHelper.PercentageToDecibels(value));
            }
        }
        private float enemyVolume;
        public float EnemyVolume
        {
            get => enemyVolume;
            set
            {
                enemyVolume = value;
                AudioManager.Instance.Mixer.SetFloat("EnemyVolume", AudioHelper.PercentageToDecibels(value));
            }
        }
        private float uiVolume;
        public float UIVolume
        {
            get => uiVolume;
            set
            {
                uiVolume = value;
                AudioManager.Instance.Mixer.SetFloat("UIVolume", AudioHelper.PercentageToDecibels(value));
            }
        }

        public Keybindings Keybinds { get; set; }



        public Settings(Vector2Int resolution, FullScreenMode fullScreenMode, int maxFramerate, bool useVSync, float uiScale, float animationSpeed, bool quickPurchase, bool showDamageIndicators, bool togglePostProcessing, float screenShakeStrength, RigorousnessLevel rigorousness, float masterVolume, float musicVolume, float soundVolume, float enemyVolume, float uiVolume, Keybindings keybinds)
        {
            Resolution = resolution;
            FullScreenMode = fullScreenMode;
            MaxFramerate = maxFramerate;
            UseVSync = useVSync;
            UIScale = uiScale;
            AnimationSpeed = animationSpeed;

            QuickPurchase = quickPurchase;
            ShowDamageIndicators = showDamageIndicators;
            TogglePostProcessing = togglePostProcessing;
            ScreenShakeStrength = screenShakeStrength;
            Rigorousness = rigorousness;

            MasterVolume = masterVolume;
            MusicVolume = musicVolume;
            SoundVolume = soundVolume;
            EnemyVolume = enemyVolume;
            UIVolume = uiVolume;

            Keybinds = keybinds;
        }

        public Settings Clone() => new Settings(
            Resolution,
            FullScreenMode,
            MaxFramerate,
            UseVSync,
            UIScale,
            AnimationSpeed,

            QuickPurchase,
            ShowDamageIndicators,
            TogglePostProcessing,
            ScreenShakeStrength,
            Rigorousness,

            MasterVolume,
            MusicVolume,
            SoundVolume,
            EnemyVolume,
            UIVolume,

            Keybinds
        );

        public static readonly Settings Default = new Settings(
            new Vector2Int(1920, 1080),      //Resolution
            FullScreenMode.FullScreenWindow, //FullScreenMode
            144,  //MaxFramerate
            true, //UseVSync
            50f,  //UIScale
            50f,  //AnimationSpeed

            false, //QuickPurchase
            true,  //ShowDamageIndicators
            true,  //TogglePostProcessing
            100f,  //ScreenShakeStrength
            RigorousnessLevel.Sufficient, //Rigorousness

            100f, //MasterVolume
            60f,  //MusicVolume
            80f,  //SoundVolume
            75f,  //EnemyVolume
            70f,  //UIVolume

            new Keybindings()
        );



        /// <summary>
        /// Nicely prints out the Settings (primarily for debugging purposes)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("SCREEN:");
            builder.AppendLine($"Resolution: {Resolution.x}x{Resolution.y}");
            builder.AppendLine($"Fullscreen Mode: {FullScreenMode}");
            builder.AppendLine($"Framerate: {MaxFramerate} (VSync {(UseVSync ? "On" : "Off")})");
            builder.AppendLine($"UI Scale: {UIScale * 100f}%");
            builder.AppendLine($"Animation Speed: {AnimationSpeed * 100f}%");

            builder.AppendLine("\nGAMEPLAY:");
            builder.AppendLine($"Quick Buy: {QuickPurchase}");
            builder.AppendLine($"Damage Numbers: {ShowDamageIndicators}");
            builder.AppendLine($"Post-Processing: {TogglePostProcessing}");
            builder.AppendLine($"Screen Shake: {ScreenShakeStrength * 100f}%");
            builder.AppendLine($"Rigorousness: {Rigorousness}");

            builder.AppendLine("\nAUDIO:");
            builder.AppendLine($"Master: {MasterVolume}%");
            builder.AppendLine($"Music: {MusicVolume}%");
            builder.AppendLine($"Sound: {SoundVolume}%");
            builder.AppendLine($"Enemy: {EnemyVolume}%");
            builder.AppendLine($"UI: {UIVolume}%");

            builder.AppendLine("\nCONTROLS:");
            foreach(KeybindPair pair in Keybinds.Serialise())
                builder.AppendLine($"{pair.key}: {pair.code}");

            return builder.ToString();
        }
    }

    /// <summary>
    /// Stores the user's preferred keybinds, indexable by Key
    /// </summary>
	public class Keybindings
	{
		private readonly Dictionary<Key, KeyCode> bindings;
		public KeyCode this[Key key]
		{
            //as much as TryGetValue would be safer here, i'm 99% sure it's unnecessary
			get => bindings[key];
			set => bindings[key] = value;
		}

        /// <summary>
        /// Returns the current movement (addition of WASD equivalent) as a non-normalised Vector2
        /// </summary>
        /// <returns></returns>
		public Vector2 GetMovement()
		{
			Vector2 result = Vector2.zero;
            if(Input.GetKey(this[Key.Left])) result -= Vector2.right;
            if(Input.GetKey(this[Key.Right])) result += Vector2.right;
            if(Input.GetKey(this[Key.Down])) result -= Vector2.up;
            if(Input.GetKey(this[Key.Up])) result += Vector2.up;
			return result;
		}

        public Keybindings() => bindings = new Dictionary<Key, KeyCode>()
        {
            { Key.Up, KeyCode.W },
            { Key.Down, KeyCode.S },
            { Key.Left, KeyCode.A },
            { Key.Right, KeyCode.D },
            { Key.Sprint, KeyCode.LeftShift },
            { Key.Slide, KeyCode.Space },

            { Key.Interact, KeyCode.F },
            { Key.Map, KeyCode.M },
            { Key.ZoomIn, KeyCode.Equals },
            { Key.ZoomOut, KeyCode.Minus },

            { Key.Potion1, KeyCode.Mouse0 },
            { Key.Potion2, KeyCode.Mouse1 },
            { Key.Reload, KeyCode.R },

            { Key.ToggleUI, KeyCode.F1 },
            { Key.Screenshot, KeyCode.F2 },
            { Key.DebugMenu, KeyCode.F3 }
        };

        /// <summary>
        /// Factory method for creating a Keybindings based on a List of KeybindPairs from a JSON file
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static Keybindings Generate(List<KeybindPair> pairs)
        {
            Keybindings keybinds = new Keybindings();
            foreach(KeybindPair pair in pairs)
                keybinds[pair.key] = pair.code;

            return keybinds;
        }

        /// <summary>
        /// Converts the keybinds into a list of objects of JSON serialisation
        /// </summary>
        /// <returns></returns>
        public List<KeybindPair> Serialise() => bindings.Select(pair => new KeybindPair(pair.Key, pair.Value)).ToList();

        /// <summary>
        /// A list of keybinds that cannot be used for controls for any reason
        /// </summary>
        public static readonly KeyCode[] BANNED_KEYBINDS = 
        {
            //keys that already do something important
            KeyCode.Escape,
            KeyCode.Menu,
            KeyCode.LeftWindows,
            KeyCode.RightWindows,
            KeyCode.LeftMeta,
            KeyCode.RightMeta,

            //literally who tf is gonna use these buttons
            KeyCode.Insert,
            KeyCode.Delete,
            KeyCode.Home,
            KeyCode.End,
            KeyCode.PageUp,
            KeyCode.PageDown,
            KeyCode.Print,
            KeyCode.ScrollLock,
            KeyCode.Pause,

            //handled elsewhere
            KeyCode.Mouse0,
            KeyCode.F1,
            KeyCode.F2,
            KeyCode.F3
        };
    }

	public enum Key
	{
		Up,
		Down,
		Left,
		Right,
		Sprint,
		Slide,

        Interact,
        Map,
		ZoomIn,
		ZoomOut,

        Potion1,
        Potion2,
        Reload,

        ToggleUI,
		Screenshot,
		DebugMenu
	};

    public enum RigorousnessLevel
    {
        Careless,
        Sufficient,
        Meticulous
    }
}

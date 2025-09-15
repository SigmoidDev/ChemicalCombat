using System.Collections.Generic;
using System.Linq;
using System;
using Sigmoid.Utilities;
using Sigmoid.Audio;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Sigmoid.UI
{
    /// <summary>
    /// Handles interactions between the physical menu and the Options class
    /// </summary>
	public class OptionsMenu : Singleton<OptionsMenu>
	{
        private bool isOpen;
        public static bool IsOpen => Instance.isOpen;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject parentObject;
        [SerializeField] private RectTransform whiteboardRect;
        [SerializeField] private float offScreenHeight;

        [SerializeField] private AudioPlayer stickyNoteSound;

        private const float ANIMATION_TIME = 0.35f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;

        public void Open(float delay)
        {
            isOpen = true;

            parentObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            DOTween.Kill("OptionsMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("OptionsMenu");
            sequence.SetUpdate(true);

            sequence.Insert(delay * Options.Current.AnimationTimeMultiplier, whiteboardRect.DOAnchorPos(Vector2.zero, AnimationTime * 2f).SetEase(Ease.OutQuart));
        }

        public void Close()
        {
            isOpen = false;
            OptionsSaver.Instance.SaveSettings();

            DOTween.Kill("OptionsMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("OptionsMenu");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, whiteboardRect.DOAnchorPos(new Vector2(0f, offScreenHeight), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                parentObject.SetActive(false);
            });
        }

        #region Display
        [Header("Display"), SerializeField] private TMP_Dropdown resolutionDropdown;
        public int Resolution
        {
            get => availableResolutions.FindIndex(r => r.x == Options.Current.Resolution.x && r.y == Options.Current.Resolution.y);
            set => Options.Current.Resolution = availableResolutions[value];
        }

        private List<Vector2Int> availableResolutions;
        private void UpdateAvailableResolutions()
        {
            Resolution[] allResolutions = Screen.resolutions;
            double targetRatio = Screen.currentResolution.refreshRateRatio.value;

            availableResolutions = allResolutions.Where(r => r.refreshRateRatio.value == targetRatio).Select(r => new Vector2Int(r.width, r.height)).ToList();
            List<string> dropdownOptions = availableResolutions.Select(r => $"{r.x}x{r.y}").ToList();

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(dropdownOptions);
        }


        [SerializeField] private TMP_Dropdown windowModeDropdown;
        public int ScreenMode
        {
            get => Options.Current.FullScreenMode switch
            {
                FullScreenMode.ExclusiveFullScreen => 1,
                FullScreenMode.FullScreenWindow => 2,
                _ => 0
            };
            set => Options.Current.FullScreenMode = value switch
            {
                1 => FullScreenMode.ExclusiveFullScreen,
                2 => FullScreenMode.FullScreenWindow,
                _ => FullScreenMode.Windowed
            };
        }


        [SerializeField] private TMP_Dropdown framerateDropdown;
        public int MaxFramerate
        {
            get => Options.Current.MaxFramerate switch
            {
                30 => 0,
                60 => 1,
                90 => 2,
                144 => 3,
                _ => 4
            };
            set => Options.Current.MaxFramerate = value switch
            {
                0 => 30,
                1 => 60,
                2 => 90,
                3 => 144,
                _ => -1
            };
        }


        [SerializeField] private Toggle vSyncToggle;
        public bool UseVSync
        {
            get => Options.Current.UseVSync;
            set => Options.Current.UseVSync = value;
        }


        [SerializeField] private Slider uiScaleSlider;
        public float UIScale
        {
            get => Options.Current.UIScale;
            set
            {
                Options.Current.UIScale = value;
                OnUIResized?.Invoke(value);
            }
        }
        public event Action<float> OnUIResized;


        [SerializeField] private Slider animSpeedSlider;
        public float AnimationSpeed
        {
            get => Options.Current.AnimationSpeed;
            set => Options.Current.AnimationSpeed = value;
        }
        #endregion

        #region Gameplay
        [Space, Header("Gameplay"), SerializeField] private Toggle quickBuyToggle;
        public bool QuickBuy
        {
            get => Options.Current.QuickPurchase;
            set => Options.Current.QuickPurchase = value;
        }

        [SerializeField] private Toggle damageIndicatorsToggle;
        public bool ShowDamageIndicators
        {
            get => Options.Current.ShowDamageIndicators;
            set => Options.Current.ShowDamageIndicators = value;
        }

        [SerializeField] private Toggle postProcessingToggle;
        public bool TogglePostProcessing
        {
            get => Options.Current.TogglePostProcessing;
            set
            {
                Options.Current.TogglePostProcessing = value;
                OnPostProcessingToggled?.Invoke(value);
            }
        }
        public event Action<bool> OnPostProcessingToggled;

        [SerializeField] private Slider screenShakeSlider;
        public float ScreenShake
        {
            get => Options.Current.ScreenShakeStrength;
            set => Options.Current.ScreenShakeStrength = value;
        }

        [SerializeField] private TMP_Dropdown rigorousnessDropdown;
        public int Rigorousness
        {
            get => (int) Options.Current.Rigorousness;
            set => Options.Current.Rigorousness = value switch
            {
                0 => RigorousnessLevel.Careless,
                1 => RigorousnessLevel.Sufficient,
                _ => RigorousnessLevel.Meticulous
            };
        }
        #endregion

        #region Audio
        [Space, Header("Audio"), SerializeField] private Slider masterVolumeSlider;
        public float MasterVolume
        {
            get => Options.Current.MasterVolume;
            set => Options.Current.MasterVolume = value;
        }

        [SerializeField] private Slider musicVolumeSlider;
        public float MusicVolume
        {
            get => Options.Current.MusicVolume;
            set => Options.Current.MusicVolume = value;
        }

        [SerializeField] private Slider soundVolumeSlider;
        public float SoundVolume
        {
            get => Options.Current.SoundVolume;
            set => Options.Current.SoundVolume = value;
        }

        [SerializeField] private Slider enemyVolumeSlider;
        public float EnemyVolume
        {
            get => Options.Current.EnemyVolume;
            set => Options.Current.EnemyVolume = value;
        }

        [SerializeField] private Slider uiVolumeSlider;
        public float UIVolume
        {
            get => Options.Current.UIVolume;
            set => Options.Current.UIVolume = value;
        }
        #endregion

        #region Keybinds
        [Space, Header("Controls"), SerializeField] private Keybinder upKeybinder;
        [SerializeField] private Keybinder downKeybinder;
        [SerializeField] private Keybinder leftKeybinder;
        [SerializeField] private Keybinder rightKeybinder;
        [SerializeField] private Keybinder sprintKeybinder;
        [SerializeField] private Keybinder slideKeybinder;

        [SerializeField] private Keybinder interactKeybinder;
        [SerializeField] private Keybinder reloadKeybinder;
        [SerializeField] private Keybinder potion1Keybinder;
        [SerializeField] private Keybinder potion2Keybinder;

        [SerializeField] private Keybinder mapKeybinder;
        [SerializeField] private Keybinder zoomInKeybinder;
        [SerializeField] private Keybinder zoomOutKeybinder;
        #endregion

        #region Sections
        [SerializeField] private List<CanvasGroup> sectionGroups;
        private int selectedSection;

        private const float FADE_TIME = 0.3f;
        private static float FadeTime => FADE_TIME * Options.Current.AnimationTimeMultiplier;

        /// <summary>
        /// Enables a specific tab, disabling the currently active one
        /// </summary>
        /// <param name="index"></param>
        public void ToggleSection(int index)
        {
            if(selectedSection == index) return;
            stickyNoteSound.Play();

            CanvasGroup oldGroup = sectionGroups[selectedSection];
            oldGroup.DOFade(0f, FadeTime).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                oldGroup.interactable = false;
                oldGroup.blocksRaycasts = false;
            });            

            selectedSection = index;

            CanvasGroup toggledGroup = sectionGroups[selectedSection];
            toggledGroup.DOFade(1f, FadeTime).SetDelay(FadeTime * 0.667f).SetEase(Ease.InOutQuad);
            toggledGroup.interactable = true;
            toggledGroup.blocksRaycasts = true;
        }
        #endregion

        private void Start()
        {
            ToggleSection(0);
            UpdateAvailableResolutions();

            RefreshDisplay();
            RefreshGameplay();
            RefreshAudio();
            RefreshControls();
            parentObject.SetActive(false);
        }

        /// <summary>
        /// Resets all display-related settings to their default values
        /// </summary>
        public void ResetDisplay()
        {
            Settings defaultSettings = Settings.Default;
            Options.Current.Resolution = defaultSettings.Resolution;
            Options.Current.FullScreenMode = defaultSettings.FullScreenMode;
            Options.Current.MaxFramerate = defaultSettings.MaxFramerate;
            Options.Current.UseVSync = defaultSettings.UseVSync;
            Options.Current.UIScale = defaultSettings.UIScale;
            Options.Current.AnimationSpeed = defaultSettings.AnimationSpeed;
            RefreshDisplay();
        }

        /// <summary>
        /// Updates all display UI elements to show the currently stored values
        /// </summary>
        public void RefreshDisplay()
        {
            resolutionDropdown.value = Resolution;
            windowModeDropdown.value = ScreenMode;
            framerateDropdown.value = MaxFramerate;
            vSyncToggle.isOn = UseVSync;
            uiScaleSlider.value = UIScale;
            animSpeedSlider.value = AnimationSpeed;
        }

        /// <summary>
        /// Resets all gameplay-related settings to their default values
        /// </summary>
        public void ResetGameplay()
        {
            Settings defaultSettings = Settings.Default;
            Options.Current.QuickPurchase = defaultSettings.QuickPurchase;
            Options.Current.ShowDamageIndicators = defaultSettings.ShowDamageIndicators;
            Options.Current.TogglePostProcessing = defaultSettings.TogglePostProcessing;
            Options.Current.ScreenShakeStrength = defaultSettings.ScreenShakeStrength;
            Options.Current.Rigorousness = defaultSettings.Rigorousness;
            RefreshGameplay();
        }

        /// <summary>
        /// Updates all gameplay UI elements to show the currently stored values
        /// </summary>
        public void RefreshGameplay()
        {
            quickBuyToggle.isOn = QuickBuy;
            damageIndicatorsToggle.isOn = ShowDamageIndicators;
            postProcessingToggle.isOn = TogglePostProcessing;
            screenShakeSlider.value = ScreenShake;
            rigorousnessDropdown.value = Rigorousness;
        }

        /// <summary>
        /// Resets all audio-related settings to their default values
        /// </summary>
        public void ResetAudio()
        {
            Settings defaultSettings = Settings.Default;
            Options.Current.MasterVolume = defaultSettings.MasterVolume;
            Options.Current.MusicVolume = defaultSettings.MusicVolume;
            Options.Current.SoundVolume = defaultSettings.SoundVolume;
            Options.Current.EnemyVolume = defaultSettings.EnemyVolume;
            Options.Current.UIVolume = defaultSettings.UIVolume;
            RefreshAudio();
        }

        /// <summary>
        /// Updates all audio UI elements to show the currently stored values
        /// </summary>
        public void RefreshAudio()
        {
            masterVolumeSlider.value = MasterVolume;
            musicVolumeSlider.value = MusicVolume;
            soundVolumeSlider.value = SoundVolume;
            enemyVolumeSlider.value = EnemyVolume;
            uiVolumeSlider.value = UIVolume;
        }

        /// <summary>
        /// Resets all keybinds to their default values
        /// </summary>
        public void ResetControls()
        {
            Options.Current.Keybinds = new Keybindings();
            RefreshControls();
        }

        /// <summary>
        /// Updates all Keybinder elements to show the currently stored controls
        /// </summary>
        public void RefreshControls()
        {
            upKeybinder.Set(Options.Keybinds[Key.Up]);
            downKeybinder.Set(Options.Keybinds[Key.Down]);
            leftKeybinder.Set(Options.Keybinds[Key.Left]);
            rightKeybinder.Set(Options.Keybinds[Key.Right]);
            sprintKeybinder.Set(Options.Keybinds[Key.Sprint]);
            slideKeybinder.Set(Options.Keybinds[Key.Slide]);

            interactKeybinder.Set(Options.Keybinds[Key.Interact]);
            reloadKeybinder.Set(Options.Keybinds[Key.Reload]);
            potion1Keybinder.Set(Options.Keybinds[Key.Potion1]);
            potion2Keybinder.Set(Options.Keybinds[Key.Potion2]);

            mapKeybinder.Set(Options.Keybinds[Key.Map]);
            zoomInKeybinder.Set(Options.Keybinds[Key.ZoomIn]);
            zoomOutKeybinder.Set(Options.Keybinds[Key.ZoomOut]);
        }
	}
}

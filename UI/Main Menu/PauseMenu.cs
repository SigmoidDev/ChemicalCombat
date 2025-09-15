using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.Game;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.UI
{
    /// <summary>
    /// Allows for the game to be temporarily paused, and for the OptionsMenu to be opened mid-game
    /// </summary>
	public class PauseMenu : Singleton<PauseMenu>
	{
        private bool isOpen = false;
		public static bool IsOpen => Instance.isOpen;

        private bool inConfirmation;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject menuObject;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private RectTransform titleRect;
        [SerializeField] private RectTransform buttonsRect;
        [SerializeField] private RectTransform confirmationRect;

		private void Update()
        {
            if(!Player.Instance.IsAlive) return;

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(OptionsMenu.IsOpen) CloseOptions();
                else if(inConfirmation) CancelConfirmation();
                else if(isOpen) Close();
                else if(!PlayerUI.InMenu) Open();
            }
        }

        private static readonly Color BackgroundVisible = new Color(1f, 1f, 1f, 0.1f);
        private static readonly Color BackgroundHidden = new Color(1f, 1f, 1f, 0f);
        private const float ANIMATION_TIME = 0.35f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;
        private const float OPEN_DELAY = 0.1f;
        private static float OpenDelay => OPEN_DELAY * Options.Current.AnimationTimeMultiplier;

		public void Open()
		{
			PlayerUI.Instance.Hide();
            Time.timeScale = 0f;
			isOpen = true;

            Show();
		}

        public void Show(bool fromBottom = true)
        {
            menuObject.SetActive(true);
            canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;

            DOTween.Kill("PauseMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("PauseMenu");
            sequence.SetUpdate(true);

            sequence.Insert(OpenDelay, titleRect.DOAnchorPos(new Vector2(0f, -32f), AnimationTime).SetEase(Ease.OutQuad));
            sequence.Insert(OpenDelay, buttonsRect.DOAnchorPos(new Vector2(0f, -16f), AnimationTime).SetEase(Ease.OutQuad));
            if(fromBottom) sequence.Insert(OpenDelay, backgroundImage.DOColor(BackgroundVisible, AnimationTime).SetEase(Ease.OutQuad));
        }

		public void Close()
		{
            Hide();
			PlayerUI.Instance.Show(OpenDelay);
            Time.timeScale = 1f;
            isOpen = false;
		}

        public void Hide(bool toBottom = true)
        {
            DOTween.Kill("PauseMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("PauseMenu");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, titleRect.DOAnchorPos(new Vector2(0f, toBottom ? -144f : 72f), AnimationTime).SetEase(Ease.OutQuad));
            sequence.Insert(0.0f, buttonsRect.DOAnchorPos(new Vector2(0f, toBottom ? -116f : 96f), AnimationTime).SetEase(Ease.OutQuad));
            if(toBottom)
            {
                sequence.Insert(0.0f, backgroundImage.DOColor(BackgroundHidden, AnimationTime).SetEase(Ease.OutQuad));
                sequence.OnComplete(() =>
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                    menuObject.SetActive(false);
                });
            }
        }

        public void OpenOptions()
        {
            OptionsMenu.Instance.Open(0f);
            Hide(false);
        }

        public void CloseOptions()
        {
            OptionsMenu.Instance.Close();
            Show(false);
        }

        /// <summary>
        /// Displays a confirmation button to warn the user that runs are not saved
        /// </summary>
        public void DisplayConfirmation()
        {
            inConfirmation = true;
            Hide(false);

            DOTween.Kill("ConfirmationPopup");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("ConfirmationPopup");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, confirmationRect.DOAnchorPos(Vector2.zero, AnimationTime).SetEase(Ease.OutQuad));
        }

        /// <summary>
        /// Closes the confirmation menu if the above was not intended
        /// </summary>
        public void CancelConfirmation()
        {
            inConfirmation = false;
            Show(false);

            DOTween.Kill("ConfirmationPopup");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("ConfirmationPopup");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, confirmationRect.DOAnchorPos(new Vector2(0f, -104f), AnimationTime).SetEase(Ease.OutQuad));
        }

        public void ReturnToMainMenu() => SceneLoader.Instance.ReturnToMenu();
	}
}

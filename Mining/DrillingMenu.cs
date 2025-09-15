using System.Collections;
using System;
using Sigmoid.Utilities;
using Sigmoid.Chemicals;
using Sigmoid.Cameras;
using Sigmoid.Audio;
using Sigmoid.Game;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Sigmoid.UI
{
    /// <summary>
    /// Allows the user to unlock more elements in exchange for coins
    /// </summary>
	public class DrillingMenu : Singleton<DrillingMenu>
	{
        private static WaitForSecondsRealtime _waitForSecondsRealtime0_2 = new WaitForSecondsRealtime(0.2f);
        private bool isOpen = false;
		public static bool IsOpen => Instance.isOpen;

		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private GameObject menuObject;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private RectTransform panelRect;

        private void Start() => menuObject.SetActive(false);
		private void Update()
		{
			if(isOpen && Input.GetKeyDown(KeyCode.Escape)) Close();
		}

		private static readonly Color BackgroundVisible = new Color(1f, 1f, 1f, 0.1f);
        private static readonly Color BackgroundHidden = new Color(1f, 1f, 1f, 0f);
        private const float ANIMATION_TIME = 0.35f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;
        private const float OPEN_DELAY = 0.1f;
        private static float OpenDelay => OPEN_DELAY * Options.Current.AnimationTimeMultiplier;

		public void Open()
		{
            Select(Chemical.None);
			PlayerUI.Instance.Hide();
            Time.timeScale = 0f;
			isOpen = true;

            menuObject.SetActive(true);
            canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;

            DOTween.Kill("DrillingMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("DrillingMenu");
            sequence.SetUpdate(true);

            sequence.Insert(OpenDelay, backgroundImage.DOColor(BackgroundVisible, AnimationTime).SetEase(Ease.OutQuad));
            sequence.Insert(OpenDelay, panelRect.DOAnchorPos(Vector2.zero, AnimationTime).SetEase(Ease.OutQuad));
		}

		public void Close()
		{
            DOTween.Kill("DrillingMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("DrillingMenu");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, backgroundImage.DOColor(BackgroundHidden, AnimationTime).SetEase(Ease.OutQuad));
            sequence.Insert(0.0f, panelRect.DOAnchorPos(new Vector2(0f, -124f), AnimationTime).SetEase(Ease.OutQuad));
            sequence.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                menuObject.SetActive(false);
            });

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

			PlayerUI.Instance.Show(OpenDelay);
            Time.timeScale = 1f;
            isOpen = false;
		}



        [Space, Header("Information")]
        [SerializeField] private TextMeshProUGUI selectedTitle;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private GameObject purchasingArea;
        [SerializeField] private Image purchaseButton;
        [SerializeField] private TextMeshProUGUI purchaseLabel;
        [SerializeField] private AudioPlayer errorSound;
        [SerializeField] private AudioPlayer purchaseSound;

        private Chemical selectedChemical;
        /// <summary>
        /// Updates the information panel on the right to show the correct values for this element
        /// </summary>
        /// <param name="chemical"></param>
        public void Select(Chemical chemical)
        {
            selectedChemical = chemical;
            if(selectedChemical == Chemical.None)
            {
                selectedTitle.SetText("");
                statusText.SetText("");
                priceText.SetText("");
                purchasingArea.SetActive(false);
            }
            else
            {
                selectedTitle.SetText(chemical.ToString());
                statusText.SetText("Unlocked: " + (ChemicalManager.IsUnlocked(chemical) ? "Yes" : "No"));
                priceText.SetText("Price: " + currentPrice.ToString());
                purchasingArea.SetActive(!ChemicalManager.IsUnlocked(chemical));
            }
        }

        private int currentPrice;
        private void Awake() => currentPrice = 50;

        /// <summary>
        /// Attempts to purchase the currently selected element (if you can afford it)
        /// </summary>
        public void Purchase()
        {
            if(selectedChemical == Chemical.None) return;
            if(!CoinManager.CanAfford(currentPrice))
            {
                if(activeFlash != null) StopCoroutine(activeFlash);
                activeFlash = StartCoroutine(FlashPrice());
                return;
            }

            ChemicalManager.Unlock(selectedChemical);
            CoinManager.Spend(currentPrice);
            currentPrice += 50;

            Select(selectedChemical);
            purchaseSound.Play();
        }



        private Coroutine activeFlash;
        private static readonly Color buttonColour = new Color(0.902f, 0.937f, 0.933f);
        private static readonly Color errorColour = new Color(1.000f, 0.420f, 0.478f);

        /// <summary>
        /// Plays a short red flash animation if you can't afford the element
        /// </summary>
        /// <returns></returns>
        private IEnumerator FlashPrice()
        {
            MainCamera.ShakeScreen(0.3f, 0.2f, 0.97f, true);
            errorSound.Play();

            purchaseButton.color = errorColour;
            purchaseLabel.color = errorColour;
            priceText.color = errorColour;
            yield return _waitForSecondsRealtime0_2;

            purchaseButton.color = buttonColour;
            purchaseLabel.color = buttonColour;
            priceText.color = buttonColour;
        }
	}
}

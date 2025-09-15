using Sigmoid.Utilities;
using Sigmoid.Chemicals;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.UI
{
    /// <summary>
    /// The UI panel which handles the brewing of potions
    /// </summary>
	public class PotionBench : Singleton<PotionBench>
	{
		private bool isOpen = false;
		public static bool IsOpen => Instance.isOpen;

		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private GameObject menuObject;
		[SerializeField] private Image backgroundImage;
		[SerializeField] private RectTransform shelfRect;
		[SerializeField] private RectTransform tableRect;

		[SerializeField] private PotionChanger potion1;
		[SerializeField] private PotionChanger potion2;

        /// <summary>
        /// Allows for a potion to be accessed by index (I probably should've just used an array, but there's only two potions)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public PotionChanger GetPotion(int number)
        {
            if(number == 1) return potion1;
            else return potion2;
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

            menuObject.SetActive(true);
            canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;

            DOTween.Kill("PotionBench");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("PotionBench");
            sequence.SetUpdate(true);

            sequence.Insert(OpenDelay, shelfRect.DOAnchorPos(new Vector2(-69f, 0f), AnimationTime).SetEase(Ease.OutQuart));
            sequence.Insert(OpenDelay, tableRect.DOAnchorPos(new Vector2(57f, 0f), AnimationTime).SetEase(Ease.OutQuart));
            sequence.Insert(OpenDelay, backgroundImage.DOColor(BackgroundVisible, AnimationTime).SetEase(Ease.OutQuad));
		}

		public void Close()
		{
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            StopDragging();

            DOTween.Kill("PotionBench");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("PotionBench");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, shelfRect.DOAnchorPos(new Vector2(-69f, -131f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.0f, tableRect.DOAnchorPos(new Vector2(57f, -131f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.0f, backgroundImage.DOColor(BackgroundHidden, AnimationTime).SetEase(Ease.OutQuad));
            sequence.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                menuObject.SetActive(false);
            });

			PlayerUI.Instance.Show(OpenDelay);
            Time.timeScale = 1f;
            isOpen = false;
		}

        private void Start() => menuObject.SetActive(false);
        private void Update()
		{
			if(isOpen && Input.GetKeyDown(KeyCode.Escape)) Close();
		}

		[SerializeField] private Image draggingImage;
		private Chemical pickedUpChemical;
        public bool IsDragging => pickedUpChemical != Chemical.None;

        /// <summary>
        /// Triggered on mouse down for drag and drop
        /// </summary>
        /// <param name="chemical"></param>
		public void Pickup(Chemical chemical)
		{
			pickedUpChemical = chemical;
			draggingImage.sprite = ChemicalManager.Get(chemical).sprite;
			draggingImage.enabled = true;
		}

        /// <summary>
        /// Triggered on mouse up to stop dragging
        /// </summary>
		private void StopDragging()
		{
			pickedUpChemical = Chemical.None;
			draggingImage.enabled = false;
		}

		private void LateUpdate()
		{
			if(!IsDragging) return;
			draggingImage.transform.position = Input.mousePosition;

			if(Input.GetMouseButtonUp(0))
			{
				if(potion1.IsHovering) potion1.Add(pickedUpChemical);
				else if(potion2.IsHovering) potion2.Add(pickedUpChemical);
				StopDragging();
			}
		}
	}
}

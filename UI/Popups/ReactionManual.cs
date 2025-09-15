using Sigmoid.Utilities;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.UI
{
	public class ReactionManual : Singleton<ReactionManual>
	{
		private bool isOpen = false;
		public static bool IsOpen => Instance.isOpen;

		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private GameObject menuObject;
		[SerializeField] private Image backgroundImage;
		[SerializeField] private RectTransform bookRect;

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

            DOTween.Kill("ReactionManual");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("ReactionManual");
            sequence.SetUpdate(true);

            sequence.Insert(OpenDelay, bookRect.DOAnchorPos(Vector2.zero, AnimationTime).SetEase(Ease.OutQuart));
            sequence.Insert(OpenDelay, backgroundImage.DOColor(BackgroundVisible, AnimationTime).SetEase(Ease.OutQuad));
		}

		public void Close()
		{
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            DOTween.Kill("ReactionManual");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("ReactionManual");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, bookRect.DOAnchorPos(new Vector2(0f, -128f), AnimationTime).SetEase(Ease.InOutQuad));
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
	}
}

using Sigmoid.Utilities;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.UI
{
    /// <summary>
    /// Allows the player to view logbook entries about enemies, reactions, and more, as well as records of previous runs
    /// </summary>
	public class RecordsMenu : Singleton<RecordsMenu>
	{
        private bool isOpen;
        public static bool IsOpen => Instance.isOpen;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject parentObject;
        [SerializeField] private RectTransform textMessage;

        private const float ANIMATION_TIME = 0.5f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;

        private void Start() => parentObject.SetActive(false);
		public void Open()
        {
            isOpen = true;

            parentObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            DOTween.Kill("RecordsMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("RecordsMenu");
            sequence.SetUpdate(true);

            sequence.Insert(0.4f * Options.Current.AnimationTimeMultiplier, textMessage.DOAnchorPos(Vector2.zero, AnimationTime).SetEase(Ease.OutQuad));
            //more stuff when i add it
        }

        public void Close()
        {
            isOpen = false;

            DOTween.Kill("RecordsMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("RecordsMenu");
            sequence.SetUpdate(true);

            sequence.Insert(0f, textMessage.DOAnchorPos(new Vector2(0f, -100f), 0.5f * AnimationTime).SetEase(Ease.InQuad));
            sequence.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                parentObject.SetActive(false);
            });
        }
	}
}

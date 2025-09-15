using Sigmoid.Audio;
using Sigmoid.UI;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.Game
{
	public class ReportPage : MonoBehaviour
	{
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private AudioPlayer pageNoise;

        private const float ANIMATION_TIME = 0.5f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;

		public void Slide()
        {
            pageNoise.Play();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            Sequence sequence = DOTween.Sequence();
            sequence.SetId(gameObject.name);
            sequence.SetUpdate(true);

            sequence.Insert(0f, rectTransform.DOAnchorPos(new Vector2(0f, -96f), AnimationTime).SetEase(Ease.OutSine));
            sequence.Insert(0f, canvasGroup.DOFade(0f, AnimationTime).SetEase(Ease.InOutQuint));
            sequence.OnComplete(() =>
            {
                rectTransform.anchoredPosition = new Vector2(0f, -2f);
                transform.SetSiblingIndex(0);
                canvasGroup.alpha = 0f;
                gameObject.SetActive(false);
            });
        }

        public void Reveal()
        {
            DOTween.Kill(gameObject.name);
            rectTransform.anchoredPosition = new Vector2(0f, -2f);

            gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
	}
}

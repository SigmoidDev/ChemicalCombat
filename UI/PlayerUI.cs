using Sigmoid.Utilities;
using Sigmoid.Game;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.UI
{
	public class PlayerUI : Singleton<PlayerUI>
	{
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private GameObject menuObject;
        [SerializeField] private RectTransform healthRect;
        [SerializeField] private RectTransform coinsRect;
        [SerializeField] private RectTransform minimapRect;
        [SerializeField] private RectTransform ammoRect;

        private const float ANIMATION_TIME = 0.35f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;

        public void Show(float delay = 0f)
        {
            menuObject.SetActive(true);
            canvasGroup.alpha = 1f;
            delay *= Options.Current.AnimationTimeMultiplier;

            DOTween.Kill("PlayerUI");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("PlayerUI");
            sequence.SetUpdate(true);

            sequence.Insert(delay, healthRect.DOAnchorPos(new Vector2(5f, -5f), AnimationTime).SetEase(Ease.OutBack));
            sequence.Insert(delay, coinsRect.DOAnchorPos(new Vector2(5f, -30f), AnimationTime).SetEase(Ease.OutBack));
            sequence.Insert(delay, minimapRect.DOAnchorPos(new Vector2(-8f, -8f), AnimationTime).SetEase(Ease.OutBack));
            sequence.Insert(delay, ammoRect.DOAnchorPos(new Vector2(-4f, 6f), AnimationTime).SetEase(Ease.OutBack));
        }

        public void Hide()
        {
            DOTween.Kill("PlayerUI");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("PlayerUI");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, healthRect.DOAnchorPos(new Vector2(-70f, -5f), AnimationTime).SetEase(Ease.OutQuart));
            sequence.Insert(0.0f, coinsRect.DOAnchorPos(new Vector2(-70f, -30f), AnimationTime).SetEase(Ease.OutQuart));
            sequence.Insert(0.0f, minimapRect.DOAnchorPos(new Vector2(40f, -8f), AnimationTime).SetEase(Ease.OutQuart));
            sequence.Insert(0.0f, ammoRect.DOAnchorPos(new Vector2(64f, 6f), AnimationTime).SetEase(Ease.OutQuart));
            sequence.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                menuObject.SetActive(false);
            });
        }

        /// <summary>
        /// Whether or not the player is in any of the Potion Bench, Drilling Menu, Perk Tree, or Fullscreen Map
        /// </summary>
        public static bool InMenu => SceneLoader.Instance.IsLoading || PotionBench.IsOpen || DrillingMenu.IsOpen || PerkTree.IsOpen || FullscreenMap.IsOpen || ReactionManual.IsOpen || PauseMenu.IsOpen;
	}
}

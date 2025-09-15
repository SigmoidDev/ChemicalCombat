using Sigmoid.Utilities;
using Sigmoid.Audio;
using Sigmoid.Game;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.UI
{
    /// <summary>
    /// Allows the player to open the tutorial and/or choose a difficulty
    /// </summary>
	public class DifficultySelector : Singleton<DifficultySelector>
	{
        private bool isOpen;
        public static bool IsOpen => Instance.isOpen;

        [SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private GameObject menuObject;
        [SerializeField] private RectTransform terminalRect;

        private const float ANIMATION_TIME = 0.5f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;

        private void Start() => menuObject.SetActive(false);
		public void Open()
        {
            isOpen = true;

            menuObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            DOTween.Kill("DifficultySelector");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("DifficultySelector");
            sequence.SetUpdate(true);

            sequence.Insert(0.5f * Options.Current.AnimationTimeMultiplier, terminalRect.DOAnchorPos(new Vector2(20f, -6f), AnimationTime).SetEase(Ease.OutQuad));
            CLISimulator.Instance.BootTerminal();
        }

        public void Close()
        {
            isOpen = false;

            DOTween.Kill("DifficultySelector");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("DifficultySelector");
            sequence.SetUpdate(true);

            CLISimulator.Instance.KillTerminal();
            sequence.Insert(0.0f, terminalRect.DOAnchorPos(new Vector2(20, -160f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                menuObject.SetActive(false);
            });
        }

        public void StartTutorial()
        {
            DifficultyManager.Difficulty = Difficulty.Rookie;
            AudioManager.Instance.FadeOut();
            SceneLoader.Instance.StartTutorial();
        }

        public void ChooseDifficulty(Difficulty difficulty)
        {
            DifficultyManager.Difficulty = difficulty;
            AudioManager.Instance.FadeOut();
            SceneLoader.Instance.StartGame();
        }
	}
}

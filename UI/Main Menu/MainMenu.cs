using Sigmoid.Utilities;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.UI
{
	public class MainMenu : Singleton<MainMenu>
	{
        [SerializeField] private CanvasGroup mainGroup;
        [SerializeField] private GameObject menuObject;
        [SerializeField] private RectTransform playButton;
        [SerializeField] private RectTransform recordsButton;
        [SerializeField] private RectTransform optionsButton;
        [SerializeField] private RectTransform titleRect;
        [SerializeField] private RectTransform linksRect;
        [SerializeField] private RectTransform quitRect;



        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape) && !KeybindManager.Instance.IsChoosing)
            {
                if(OptionsMenu.IsOpen) OptionsMenu.Instance.Close();
                else if(RecordsMenu.IsOpen) RecordsMenu.Instance.Close();
                else if(DifficultySelector.IsOpen) DifficultySelector.Instance.Close();
                Open();
            }
        }

        /// <summary>
        /// Opens a given string as a link in your default browser
        /// </summary>
        /// <param name="link"></param>
        public void OpenLink(string link) => Application.OpenURL(link);

        /// <summary>
        /// Safely shuts down the application
        /// </summary>
        public void QuitApp() => Application.Quit();



        private const float ANIMATION_TIME = 0.35f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;

        //i have just thought of a way better way of doing these animations but i cba

        public void Open()
        {
            menuObject.SetActive(true);
            mainGroup.alpha = 1f;
            mainGroup.interactable = true;
            mainGroup.blocksRaycasts = true;

            DOTween.Kill("MainMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("MainMenu");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, titleRect.DOAnchorPos(new Vector2(4f, -8f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.0f * Options.Current.AnimationTimeMultiplier, quitRect.DOAnchorPos(new Vector2(-8f, -8f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.1f * Options.Current.AnimationTimeMultiplier, linksRect.DOAnchorPos(new Vector2(6f, 4f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.0f, playButton.DOAnchorPos(new Vector2(0f, 24f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.1f * Options.Current.AnimationTimeMultiplier, recordsButton.DOAnchorPos(new Vector2(0f, 0f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.2f * Options.Current.AnimationTimeMultiplier, optionsButton.DOAnchorPos(new Vector2(0f, -24f), AnimationTime).SetEase(Ease.InOutQuad));
        }

        public void Close()
        {
            DOTween.Kill("MainMenu");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("MainMenu");
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, titleRect.DOAnchorPos(new Vector2(4f, 60f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.1f * Options.Current.AnimationTimeMultiplier, quitRect.DOAnchorPos(new Vector2(20f, -8f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.1f * Options.Current.AnimationTimeMultiplier, linksRect.DOAnchorPos(new Vector2(6f, -20f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.0f, playButton.DOAnchorPos(new Vector2(-112f, 24f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.1f * Options.Current.AnimationTimeMultiplier, recordsButton.DOAnchorPos(new Vector2(-112f, 0f), AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0.2f * Options.Current.AnimationTimeMultiplier, optionsButton.DOAnchorPos(new Vector2(-112f, -24f), AnimationTime).SetEase(Ease.InOutQuad));

            sequence.OnComplete(() =>
            {
                mainGroup.alpha = 0f;
                mainGroup.interactable = false;
                mainGroup.blocksRaycasts = false;
                menuObject.SetActive(false);
            });
        }
    }
}

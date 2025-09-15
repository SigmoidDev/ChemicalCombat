using System.Collections.Generic;
using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Upgrading;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.UI
{
	public class PerkTree : Singleton<PerkTree>
	{
        private bool isOpen = false;
		public static bool IsOpen => Instance.isOpen;

		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private GameObject menuObject;
		[SerializeField] private Image backgroundImage;
		[SerializeField] private CanvasGroup containerGroup;
        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform coinDisplay;

        private static readonly Color BackgroundVisible = new Color(1f, 1f, 1f, 0.1f);
        private static readonly Color BackgroundHidden = new Color(1f, 1f, 1f, 0f);
        private const float ANIMATION_TIME = 0.35f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;
        private const float OPEN_DELAY = 0.1f;
        private static float OpenDelay => OPEN_DELAY * Options.Current.AnimationTimeMultiplier;

        private void Awake() => container.sizeDelta = new Vector2(132f, 136f + 144f * Options.Current.UIReferenceMultiplier);
        private void Start() => menuObject.SetActive(false);
        public void Open()
		{
			PlayerUI.Instance.Hide();
            Time.timeScale = 0f;
			isOpen = true;

            menuObject.SetActive(true);
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;

            DOTween.Kill("PerkTree");
			Sequence sequence = DOTween.Sequence();
            sequence.SetId("PerkTree");
            sequence.SetUpdate(true);

            sequence.Insert(OpenDelay * 1.5f, coinDisplay.DOAnchorPos(new Vector2(5f, -5f), AnimationTime)).SetEase(Ease.OutSine);
            sequence.Insert(OpenDelay, container.DOSizeDelta(new Vector2(132f, 132f), AnimationTime).SetEase(Ease.OutExpo));
            sequence.Insert(OpenDelay, backgroundImage.DOColor(BackgroundVisible, AnimationTime).SetEase(Ease.OutQuad));
		}

		public void Close()
		{
            canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

            DOTween.Kill("PerkTree");
			Sequence sequence = DOTween.Sequence();
            sequence.SetId("PerkTree");
            sequence.SetUpdate(true);

            sequence.Insert(OpenDelay, coinDisplay.DOAnchorPos(new Vector2(5f, 16f), AnimationTime)).SetEase(Ease.OutQuad);
			sequence.Insert(0f, container.DOSizeDelta(new Vector2(132f, 136f + 144f * Options.Current.UIReferenceMultiplier), AnimationTime).SetEase(Ease.OutSine));
            sequence.Insert(0f, backgroundImage.DOColor(BackgroundHidden, AnimationTime).SetEase(Ease.OutQuad));
            sequence.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                menuObject.SetActive(false);
            });

			PlayerUI.Instance.Show(OPEN_DELAY);
			Time.timeScale = 1f;
			isOpen = false;
		}

		private void Update()
		{
			if(isOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                if(!isFocused) Close();
                else if(!isAnimating) Unfocus();
            }
		}



        private readonly List<TreeNode> allNodes = new List<TreeNode>();
        public void RegisterNode(TreeNode node) => allNodes.Add(node);
        public IEnumerable IterateNodes() => allNodes;




        private bool isFocused;
        private bool isAnimating;
        private Vector2 storedPosition;

        public void FocusOn(TreeNode node, bool side, Sprite sprite)
        {
            isAnimating = true;
            isFocused = true;

            RectTransform nodeRect = (RectTransform) node.transform;
            RectTransform rect = PerkBuyer.Instance.Rect;

            storedPosition = nodeRect.position;
            rect.position = nodeRect.position;
            rect.sizeDelta = nodeRect.sizeDelta;

            Sprite clonedSprite = CloneSprite(sprite);
            PerkBuyer.Instance.Initialise(node, side, clonedSprite);

            Vector2 iconPosition = Vector2.right * (side ? 80f : -80f);
            containerGroup.interactable = false;
			containerGroup.blocksRaycasts = false;

            DOTween.Kill("NodeFocus");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("NodeFocus");
            sequence.SetUpdate(true);

            sequence.Insert(0f, containerGroup.DOFade(0f, AnimationTime).SetEase(Ease.OutCubic));
            sequence.Insert(0f, rect.DOAnchorPos(iconPosition, AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0f, rect.DOSizeDelta(Vector2.one * 44f, AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0f, PerkBuyer.Instance.Box.transform.DOScale(Vector2.one, AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0f, PerkBuyer.Instance.Information.DOFade(1f, AnimationTime).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() => isAnimating = false);
        }

        public void Unfocus()
        {
            isAnimating = true;

            DOTween.Kill("NodeFocus");
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("NodeFocus");
            sequence.SetUpdate(true);

            sequence.Insert(0f, PerkBuyer.Instance.Box.transform.DOScale(Vector2.zero, AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0f, PerkBuyer.Instance.Information.DOFade(0f, AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0f, containerGroup.DOFade(1f, AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0f, PerkBuyer.Instance.Rect.DOMove(storedPosition, AnimationTime).SetEase(Ease.InOutQuad));
            sequence.Insert(0f, PerkBuyer.Instance.Rect.DOSizeDelta(Vector2.one * 22f, AnimationTime).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                containerGroup.interactable = true;
			    containerGroup.blocksRaycasts = true;
                isFocused = false;
                isAnimating = false;

                PerkBuyer.Instance.Rect.sizeDelta = Vector2.zero;
                PerkBuyer.Instance.Box.gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// Generates the isolated sprite which appears when focussing on a node
        /// (This is not a great way of doing it - it literally just copies the sprite and cuts off the edges)
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        private Sprite CloneSprite(Sprite sprite)
        {
            int startX = (int) sprite.rect.x;
            int startY = (int) sprite.rect.y;
            int width = (int) sprite.rect.width;
            int height = (int) sprite.rect.height;

            Color[] pixels = sprite.texture.GetPixels(startX, startY, width, height);
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    bool onEdge = x == 0 || x == width - 1 || y == 0 || y == height - 1;
                    if(onEdge) pixels[y * width + x] = Color.clear;
                }
            }

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(pixels);
            texture.Apply();

            Rect rect = new Rect(0, 0, width, height);
            return Sprite.Create(texture, rect, sprite.pivot / sprite.rect.size, sprite.pixelsPerUnit);
        }
	}
}

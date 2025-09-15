using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.Audio;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace Sigmoid.UI
{
	public class FullscreenMap : Singleton<FullscreenMap>
	{
		private bool isOpen = false;
		public static bool IsOpen => Instance.isOpen;

		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private GameObject mapObject;
        [SerializeField] private Image canvas;
		[SerializeField] private RectTransform rect;
        [SerializeField] private AudioPlayer player;
        [SerializeField] private ScriptableAudio openSound;
        [SerializeField] private ScriptableAudio closeSound;

        private const float ANIMATION_TIME = 0.4f;
        private static float AnimationTime => ANIMATION_TIME * Options.Current.AnimationTimeMultiplier;

        private void Start()
        {
            canvas.material = Map.MapMaterial;
            zoom = 1f;
            mapObject.SetActive(false);
        }

		private void Update()
		{
            if(!PlayerUI.InMenu && Input.GetKeyDown(Options.Keybinds[Key.Map])) Open();
			else if(isOpen && (Input.GetKeyDown(Options.Keybinds[Key.Map]) || Input.GetKeyDown(KeyCode.Escape))) Close();

            if(!isOpen) return;
            HandlePanning();
            HandleZooming();
		}

		public void Open()
		{
            player.Play(openSound, AudioChannel.UI);
			PlayerUI.Instance.Hide();

            mapObject.SetActive(true);
            DOTween.Kill(canvasGroup);
			canvasGroup.DOFade(1f, AnimationTime).SetEase(Ease.InOutQuad);
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;

			Time.timeScale = 0f;
			isOpen = true;

            CentreOnPlayer();
		}

		public void Close()
		{
            player.Play(closeSound, AudioChannel.UI);
			PlayerUI.Instance.Show();

            DOTween.Kill(canvasGroup);
			canvasGroup.DOFade(0f, AnimationTime).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                canvasGroup.interactable = false;
			    canvasGroup.blocksRaycasts = false;
                mapObject.SetActive(false);
            });

			Time.timeScale = 1f;
			isOpen = false;
		}

        public void UpdateCanvas(Vector2Int size)
        {
			rect.sizeDelta = size;
            rect.gameObject.SetActive(false);
			rect.gameObject.SetActive(true);
        }

        private const float KEY_MOVE_SPEED = 36f;
        private const float PAN_ELASTICITY = 10f;
        private static Vector2 PAN_PADDING = new Vector2(32f, 32f);

        private Vector2 dragPosition;
        private bool isDragging;
        private void HandlePanning()
        {
            if(!isDragging && Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                dragPosition = MouseToCanvas(Input.mousePosition);
            }
            else if(isDragging && Input.GetMouseButtonUp(0)) isDragging = false;



            if(isDragging)
            {
                Vector2 mousePosition = MouseToCanvas(Input.mousePosition);
                Vector2 deltaPosition = mousePosition - dragPosition;
                dragPosition = mousePosition;

                rect.anchoredPosition += deltaPosition;
            }
            else
            {
                float horizontal = Input.GetAxisRaw("Horizontal");
                float vertical = Input.GetAxisRaw("Vertical");
                if(horizontal != 0f || vertical != 0f)
                {
                    Vector2 movement = new Vector2(horizontal, vertical).normalized;
                    rect.anchoredPosition -= Time.unscaledDeltaTime * KEY_MOVE_SPEED * movement;
                }

                Vector2 scaledSize = rect.sizeDelta * zoom;
                Vector2 sizeDifference = scaledSize - CanvasSize;

                Vector2 offset = new Vector2(
                    sizeDifference.x < 0f ? 0.5f * (CanvasSize.x - scaledSize.x) : 0f,
                    sizeDifference.y < 0f ? 0.5f * (CanvasSize.y - scaledSize.y) : 0f
                );

                Vector2 boundsMin = Vector2.Min(Vector2.zero, -sizeDifference) + offset;
                Vector2 boundsMax = offset;

                Vector2 clampedPosition = new Vector2(
                    Mathf.Clamp(rect.anchoredPosition.x, boundsMin.x - PAN_PADDING.x, boundsMax.x + PAN_PADDING.x),
                    Mathf.Clamp(rect.anchoredPosition.y, boundsMin.y - PAN_PADDING.y, boundsMax.y + PAN_PADDING.y)
                );

                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, clampedPosition, Time.unscaledDeltaTime * PAN_ELASTICITY);
            }
        }

        private void CentreOnPlayer()
        {
            Vector2 playerPosition = Player.Instance.transform.position;
            rect.anchoredPosition = 0.5f * CanvasSize - (playerPosition + Map.Instance.Offset) * zoom;
        }

        private static Vector2 ScreenSize => new Vector2(Screen.width, Screen.height);
        private static Vector2 CanvasSize => new Vector2(256, 144) * Options.Current.UIReferenceMultiplier;
        private static Vector2 MouseToCanvas(Vector2 position) => position / ScreenSize * CanvasSize;

        private const float KEY_ZOOM_SPEED = 16f;
        private const float ZOOM_SENSITIVITY = 0.1f;

        private float zoom;
        private void HandleZooming()
        {
            bool ignoreMouse = false;
            float scrollY = Input.mouseScrollDelta.y;
            if(scrollY == 0f)
            {
                if(Input.GetKey(Options.Keybinds[Key.ZoomIn]))
                {
                    ignoreMouse = true;
                    scrollY = KEY_ZOOM_SPEED * Time.unscaledDeltaTime;
                }
                else if(Input.GetKey(Options.Keybinds[Key.ZoomOut]))
                {
                    ignoreMouse = true;
                    scrollY = -KEY_ZOOM_SPEED * Time.unscaledDeltaTime;
                }
                else return;
            }

            Vector2 mousePosition = ignoreMouse ? 0.5f * CanvasSize : MouseToCanvas(Input.mousePosition);
            Vector2 scaledPosition = (mousePosition - rect.anchoredPosition) / zoom;

            zoom = Mathf.Clamp(zoom + scrollY * ZOOM_SENSITIVITY, 0.6f, 4.0f);
            rect.localScale = Vector2.one * zoom;

            rect.anchoredPosition = mousePosition - scaledPosition * zoom;
        }

        public void TeleportMouse()
        {
            Vector2 normalisedPosition = Input.mousePosition / ScreenSize;
            Vector2 normalisedMin = rect.position / ScreenSize;
            Vector2 normalisedWidth = rect.sizeDelta * zoom / CanvasSize;
            Vector2 normalisedCoords = (normalisedPosition - normalisedMin) / normalisedWidth;

            Player.Instance.transform.position = normalisedCoords * rect.sizeDelta - Map.Instance.Offset;
        }
	}
}

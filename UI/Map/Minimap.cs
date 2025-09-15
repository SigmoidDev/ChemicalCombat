using Sigmoid.Utilities;
using Sigmoid.Players;
using UnityEngine;
using UnityEngine.UI;

namespace Sigmoid.UI
{
	public class Minimap : Singleton<Minimap>
	{
        [SerializeField] private Image canvas;
		[SerializeField] private RectTransform rect;
		[SerializeField] private RectTransform scale;
		[SerializeField] private RectTransform offset;

        public float Scale { get; private set; }

        private void Start()
        {
            canvas.material = Map.MapMaterial;
            Scale = 1f;
        }

        public void UpdateCanvas(Vector2Int size)
        {
            offset.anchoredPosition = -Map.Instance.Offset;

			rect.sizeDelta = size;
            rect.gameObject.SetActive(false);
			rect.gameObject.SetActive(true);
        }

		private void Update()
		{
			if(Input.GetKey(Options.Keybinds[Key.ZoomIn])) ZoomIn();
			else if(Input.GetKey(Options.Keybinds[Key.ZoomOut])) ZoomOut();

			Vector2 playerPosition = Player.Instance.transform.position;
			rect.anchoredPosition = -playerPosition;
		}

		private void ZoomIn()
		{
			Scale = Mathf.Clamp(Scale + Time.deltaTime * 2f, 0.5f, 2f);
			scale.localScale = Vector2.one * Scale;
		}

		private void ZoomOut()
		{
			Scale = Mathf.Clamp(Scale - Time.deltaTime * 2f, 0.5f, 2f);
			scale.localScale = Vector2.one * Scale;
		}
	}
}

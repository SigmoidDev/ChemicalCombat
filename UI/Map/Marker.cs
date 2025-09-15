using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.UI
{
	public class Marker : MonoBehaviour
	{
		[SerializeField] private RectTransform rect;
		[SerializeField] private Image image;

        public Marker Initialise(Vector2 position, Sprite sprite)
        {
            rect.anchoredPosition = position;
            image.sprite = sprite;
            return this;
        }
	}
}

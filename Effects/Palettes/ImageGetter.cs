using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.Effects
{
	public class ImageGetter : IColourable
	{
		[SerializeField] private Image uiImage;

		public override Sprite Sprite
		{
			get => uiImage.sprite;
			set => uiImage.sprite = value;
		}

		public override Color Colour
		{
			get => uiImage.color;
			set => uiImage.color = value;
		}

		public override Material Material
		{
			get => uiImage.material;
			set => uiImage.material = value;
		}
	}
}

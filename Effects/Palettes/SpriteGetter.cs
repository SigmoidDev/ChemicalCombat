using UnityEngine;

namespace Sigmoid.Effects
{
	public class SpriteGetter : IColourable
	{
		[SerializeField] private SpriteRenderer spriteRenderer;

		public override Sprite Sprite
		{
			get => spriteRenderer.sprite;
			set => spriteRenderer.sprite = value;
		}

		public override Color Colour
		{
			get => spriteRenderer.color;
			set => spriteRenderer.color = value;
		}

		public override Material Material
		{
			get => spriteRenderer.material;
			set => spriteRenderer.material = value;
		}
	}
}

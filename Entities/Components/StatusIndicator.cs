using UnityEngine;
using Sigmoid.Chemicals;

namespace Sigmoid.Enemies
{
	public class StatusIndicator : MonoBehaviour
	{
		[field: SerializeField] public SpriteRenderer Sprite { get; private set; }

		private Chemical chemical;
		public Chemical Chemical => chemical;

		public StatusIndicator Initialise(Chemical chemical, Transform parent)
		{
			this.chemical = chemical;
			Sprite.sprite = ChemicalManager.Get(chemical).statusSprite;
            Sprite.color = Color.white;
			transform.parent = parent;
			return this;
		}
	}
}

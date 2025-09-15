using UnityEngine;

namespace Sigmoid.Effects
{
	/// <summary>
	/// Allows for interaction with the PaletteSwap shader, which replaces certain colours with others
	/// </summary>
	public class PaletteSwapper : MonoBehaviour
	{
		[SerializeField] private IColourable colourable;
		[field: SerializeField] public Color[] Originals { get; private set; }
        [field: SerializeField] public Color[] Replacements { get; private set; }

		private void Awake() => UpdateLUT(Originals, Replacements);
		public void UpdateLUT(Color[] newOriginals, Color[] newReplacements)
		{
			int numReplacements = newOriginals.Length;
			if(numReplacements != newReplacements.Length)
			{
				Debug.LogError("Mismatch in number of replacement colours!", gameObject);
				return;
			}

            Originals = newOriginals;
            Replacements = newReplacements;

			Texture2D lut = new Texture2D(numReplacements, 2);
			lut.filterMode = FilterMode.Point;
			for(int i = 0; i < numReplacements; i++)
			{
				lut.SetPixel(i, 0, newOriginals[i]);
				lut.SetPixel(i, 1, newReplacements[i]);
			}
			lut.Apply(false, true);

			colourable.Material.SetTexture("_LookupTex", lut);
			colourable.Material.SetInt("_Replacements", numReplacements);
		}
	}

	/// <summary>
	/// I know it's a class, but that's only cause it has to derive from MonoBehaviour so that it can be serialized<br/>
	/// Enforces that a sprite, colour, and material can all be accessed (regardless of whether or not it's a SpriteRenderer or an Image)
	/// </summary>
	public abstract class IColourable : MonoBehaviour
	{
		public abstract Sprite Sprite { get; set; }
		public abstract Color Colour { get; set; }
		public abstract Material Material { get; set; }

		private void Awake() => Material = new Material(Material);
	}
}

using UnityEngine.UI;
using UnityEngine;
using Sigmoid.Chemicals;
using Sigmoid.Utilities;

namespace Sigmoid.UI
{
	public class BulletDisplay : MonoBehaviour
	{
		[SerializeField] private Image image;

		public Chemical Chemical { get; private set; }
		public BulletDisplay Initialise(Chemical chemical)
		{
			Chemical = chemical;
			gameObject.name = chemical.ToString();
			Regain();
			return this;
		}

		public void Expend()
		{
			image.sprite = ChemicalManager.Get(Chemical.None).miniSprite;
		}

		public void Regain()
		{
			image.sprite = ChemicalManager.Get(Chemical).miniSprite;
		}
	}
}

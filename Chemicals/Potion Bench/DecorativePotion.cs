using System.Collections.Generic;
using Sigmoid.Chemicals;
using Sigmoid.Effects;
using UnityEngine;

namespace Sigmoid.UI
{
    /// <summary>
    /// The decorative potion objects that sit on the actual table
    /// </summary>
	public class DecorativePotion : MonoBehaviour
	{
        [SerializeField] private PaletteSwapper swapper;
		[SerializeField] private int linkedPotion;

        private void Awake() => PotionBench.Instance.GetPotion(linkedPotion).OnModify += OnModify; //unsubscribed manually below in OnDestroy
        private void OnDestroy()
        {
            if(!PotionBench.InstanceExists) return;
            PotionBench.Instance.GetPotion(linkedPotion).OnModify -= OnModify;
        }

        /// <summary>
        /// Recolours the potion similarly to how it is done in the UI
        /// </summary>
        /// <param name="concoction"></param>
        private void OnModify(List<Chemical> concoction)
        {
            if(concoction.Count == 0)
            {
                swapper.UpdateLUT(swapper.Originals, new Color[3]
                {
                    new Color(0.8352942f, 0.8705883f, 0.8784314f),
                    new Color(0.7098039f, 0.7372549f, 0.7686275f),
                    new Color(0.5529412f, 0.5725490f, 0.6313726f)
                });
                return;
            }

            Color[] colours = new Color[3];
			foreach(Chemical chemical in concoction)
			{
				ScriptableChemical info = ChemicalManager.Get(chemical);
				for(int i = 0; i < 3; i++)
				{
					colours[i] += info.colours[i + 1] / concoction.Count;
				}
			}

            swapper.UpdateLUT(swapper.Originals, colours);
        }
	}
}

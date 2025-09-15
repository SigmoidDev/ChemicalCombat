using System.Collections.Generic;
using Sigmoid.Enemies;
using Sigmoid.Effects;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// A specialisation of Explosion that chooses a random colour palette on initialisation
    /// </summary>
	public class Firework : Explosion
	{
		[SerializeField] private PaletteSwapper palette;
        [SerializeField] private List<WhyDoINeedToWrapThisArrayInAClass> colourings;

        public override void Initialise(DetonatedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
            base.Initialise(pool, point, owner, hitMask, damageMultiplier);

            int randomIndex = Random.Range(0, colourings.Count);
            Color[] randomColouring = colourings[randomIndex].colours;
            palette.UpdateLUT(palette.Originals, randomColouring);
        }
	}

    [System.Serializable]
    public class WhyDoINeedToWrapThisArrayInAClass
    {
        public string name;
        public Color[] colours;
    }
}

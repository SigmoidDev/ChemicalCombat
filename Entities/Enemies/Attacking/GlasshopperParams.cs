using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Glasshopper Params", menuName = "Enemies/Attacking/Glasshopper")]
	public class GlasshopperParams : AttackerParams
	{
        public override IAttacker CreateModule(Enemy enemy) => new GlasshopperAttacker(enemy, this);
	}
}

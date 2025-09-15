using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Hellhound Params", menuName = "Enemies/Attacking/Hellhound")]
	public class HellhoundParams : AttackerParams
	{
		[field: SerializeField] public float AggroRange { get; private set; }
		[field: SerializeField] public float ChargeCooldown { get; private set; }

        public override IAttacker CreateModule(Enemy enemy) => new HellhoundAttacker(enemy, this);
	}
}

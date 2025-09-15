using UnityEngine;

namespace Sigmoid.Enemies
{
	[CreateAssetMenu(fileName = "New Droid Params", menuName = "Enemies/Attacking/Droid")]
	public class DroidParams : AttackerParams
	{
		[field: SerializeField] public int ShieldHealth { get; private set; }
		[field: SerializeField] public float ShieldDuration { get; private set; }
		[field: SerializeField] public float ShieldCooldown { get; private set; }

        public override IAttacker CreateModule(Enemy enemy) => new DroidAttacker(enemy, this);
	}
}

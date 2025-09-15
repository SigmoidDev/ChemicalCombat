using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Melee Params", menuName = "Enemies/Attacking/Melee")]
	public class MeleeParams : TimedParams
	{
		[field: SerializeField] public DamageType DamageType { get; private set; }
		[field: SerializeField] public Vector2 AttackOrigin { get; private set; }
		[field: SerializeField] public Vector2 AttackArea { get; private set; }

        public override IAttacker CreateModule(Enemy enemy) => new MeleeAttacker(enemy, this);
	}
}

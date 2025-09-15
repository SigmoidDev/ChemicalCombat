using Sigmoid.Projectiles;
using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Instant Params", menuName = "Enemies/Attacking/Instant")]
	public class InstantParams : TimedParams
	{
		[field: SerializeField] public Projectile Projectile { get; private set; }

        public override IAttacker CreateModule(Enemy enemy) => new InstantAttacker(enemy, this);
	}
}

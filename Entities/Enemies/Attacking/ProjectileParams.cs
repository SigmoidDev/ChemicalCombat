using Sigmoid.Projectiles;
using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Projectile Params", menuName = "Enemies/Attacking/Projectile")]
	public class ProjectileParams : TimedParams
	{
		[field: SerializeField] public Projectile Projectile { get; private set; }
        [field: SerializeField] public Vector2 SpawnOrigin { get; private set; }
		[field: SerializeField] public float PredictionDistance { get; private set; }

        public override IAttacker CreateModule(Enemy enemy) => new ProjectileAttacker(enemy, this);
	}
}

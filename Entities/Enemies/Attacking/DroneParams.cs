using Sigmoid.Projectiles;
using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Drone Params", menuName = "Enemies/Attacking/Drone")]
	public class DroneParams : AttackerParams
	{
		[field: SerializeField] public float DetectionRange { get; private set; }
		[field: SerializeField] public float BombCooldown { get; private set; }
		[field: SerializeField] public Bomb BombPrefab { get; private set; }
		[field: SerializeField] public float SpeedBoost { get; private set; }

        public override IAttacker CreateModule(Enemy enemy) => new DroneAttacker(enemy, this);
	}
}

using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Movement/Teleport")]
	public class TeleportParams : MovementParams
	{
		[field: SerializeField] public float TeleportInterval { get; private set; }
        [field: SerializeField] public float TeleportDelay { get; private set; }
        [field: SerializeField] public string TeleportAnimation { get; private set; }

        public override IMovement CreateModule(Enemy enemy) => new TeleportMovement(enemy, this);
    }
}

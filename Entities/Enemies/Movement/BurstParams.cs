using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Movement/Burst")]
	public class BurstParams : MovementParams
	{
		[field: SerializeField] public float JumpInterval { get; private set; }
        [field: SerializeField] public float JumpForce { get; private set; }
        [field: SerializeField] public float JumpDelay { get; private set; }
        [field: SerializeField] public string JumpAnimation { get; private set; }

        public override IMovement CreateModule(Enemy enemy) => new BurstMovement(enemy, this);
    }
}

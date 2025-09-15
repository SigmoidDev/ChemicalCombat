using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Bounce Params", menuName = "Enemies/Targeting/Bounce")]
	public class BounceParams : TargetingParams
	{
        [field: SerializeField] public LayerMask GroundMask { get; private set; }
        [field: SerializeField] public Vector2 EdgeDistance { get; private set; }
        [field: SerializeField] public Vector2 CapsuleSize { get; private set; }

        public override ITargeting CreateModule(Enemy enemy) => new BounceTargeting(enemy, this);
	}
}

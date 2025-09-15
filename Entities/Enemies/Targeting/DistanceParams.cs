using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Targeting/Distance")]
	public class DistanceParams : TargetingParams
	{
        [field: SerializeField] public float TargetDistance { get; private set; }
        [field: SerializeField] public float CloseThreshold { get; private set; }
        [field: SerializeField] public float FarThreshold { get; private set; }

        public override ITargeting CreateModule(Enemy enemy) => new DistanceTargeting(enemy, this);
	}
}

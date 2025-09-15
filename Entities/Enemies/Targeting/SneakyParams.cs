using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Targeting/Sneaky")]
	public class SneakyParams : TargetingParams
	{
        [field: SerializeField] public float TargetDistance { get; private set; }
        [field: SerializeField] public float SwerveFactor { get; private set; }

        public override ITargeting CreateModule(Enemy enemy) => new SneakyTargeting(enemy, this);
	}
}

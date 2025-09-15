using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Targeting/Random")]
	public class RandomParams : TargetingParams
	{
        [field: SerializeField] public ScriptableInterval RepathInterval { get; private set; }
        [field: SerializeField] public float TargetDistance { get; private set; }
        [field: SerializeField] public float StepDistance { get; private set; }
        [field: SerializeField] public float DirectionalBias { get; private set; }

        public override ITargeting CreateModule(Enemy enemy) => new RandomTargeting(enemy, this);
	}
}

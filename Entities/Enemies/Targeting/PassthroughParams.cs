using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Targeting/Passthrough")]
	public class PassthroughParams : TargetingParams
	{
        [field: SerializeField] public float AggroRange { get; private set; }
        [field: SerializeField] public float DeaggroRange { get; private set; }
        [field: SerializeField] public float ContinuationDistance { get; private set; }
        [field: SerializeField] public float PredictionLength { get; private set; }
        [field: SerializeField] public float FinishDistance { get; private set; }
		[field: SerializeField] public LayerMask SolidMask { get; private set; }

        public override ITargeting CreateModule(Enemy enemy) => new PassthroughTargeting(enemy, this);
	}
}

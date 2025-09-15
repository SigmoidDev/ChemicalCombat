using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Wraith Params", menuName = "Enemies/Attacking/Wraith")]
	public class WraithParams : AttackerParams
	{
        [field: SerializeField] public AnimationCurve DashCurve { get; private set; }

        public override IAttacker CreateModule(Enemy enemy) => new WraithAttacker(enemy, this);
	}
}

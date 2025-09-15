using System.Collections.Generic;
using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Targeting/Composite")]
	public class CompositeTargetingParams : TargetingParams
	{
        [field: SerializeField] public List<TargetingParams> Params { get; private set; }

        public override ITargeting CreateModule(Enemy enemy) => new CompositeTargeting(enemy, this);
	}
}

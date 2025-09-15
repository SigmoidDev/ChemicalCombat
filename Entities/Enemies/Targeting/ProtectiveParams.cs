using UnityEngine;

namespace Sigmoid.Enemies
{
	[CreateAssetMenu(fileName = "New Protective Params", menuName = "Enemies/Targeting/Protective")]
	public class ProtectiveParams : TargetingParams
	{
        public override ITargeting CreateModule(Enemy enemy) => new ProtectiveTargeting(enemy, this);
	}
}

using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Wizard Params", menuName = "Enemies/Attacking/Wizard")]
	public class WizardParams : ProjectileParams
	{
        public override IAttacker CreateModule(Enemy enemy) => new WizardAttacker(enemy, this);
	}
}

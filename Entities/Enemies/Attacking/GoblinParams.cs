using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Goblin Params", menuName = "Enemies/Attacking/Goblin")]
	public class GoblinParams : MeleeParams
	{
        public override IAttacker CreateModule(Enemy enemy) => new GoblinAttacker(enemy, this);
	}
}

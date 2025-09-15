using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Spider Params", menuName = "Enemies/Attacking/Spider")]
	public class SpiderParams : AttackerParams
	{
        public override IAttacker CreateModule(Enemy enemy) => new SpiderAttacker(enemy, this);
	}
}

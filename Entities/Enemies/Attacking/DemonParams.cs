using UnityEngine;

namespace Sigmoid.Enemies
{
	[CreateAssetMenu(fileName = "New Demon Params", menuName = "Enemies/Attacking/Demon")]
    public class DemonParams : CompositeAttackerParams
    {
        public override IAttacker CreateModule(Enemy enemy) => new DemonAttacker(enemy, this);
    }
}

using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Phantom Params", menuName = "Enemies/Attacking/Phantom")]
	public class PhantomParams : AttackerParams
	{
        public override IAttacker CreateModule(Enemy enemy) => new PhantomAttacker(enemy, this);
	}
}

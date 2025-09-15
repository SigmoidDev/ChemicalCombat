using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Ooze Params", menuName = "Enemies/Attacking/Ooze")]
	public class OozeParams : AttackerParams
	{
		[field: SerializeField] public float JumpRadius { get; private set; }
		[field: SerializeField] public float BurrowRadius { get; private set; }

        public override IAttacker CreateModule(Enemy enemy) => new OozeAttacker(enemy, this);
	}
}

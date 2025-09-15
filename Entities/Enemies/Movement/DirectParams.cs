using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Movement/Direct")]
	public class DirectParams : MovementParams
	{
		[field: SerializeField] public float Speed { get; private set; }

        public override IMovement CreateModule(Enemy enemy) => new DirectMovement(enemy, this);
    }
}

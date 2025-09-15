using System.Collections.Generic;
using UnityEngine;

namespace Sigmoid.Enemies
{
    [CreateAssetMenu(fileName = "New Movement Params", menuName = "Enemies/Movement/Composite")]
    public class CompositeMovementParams : MovementParams
    {
        [field: SerializeField] public List<MovementParams> Params { get; private set; }

        public override IMovement CreateModule(Enemy enemy) => new CompositeMovement(enemy, this);
    }
}

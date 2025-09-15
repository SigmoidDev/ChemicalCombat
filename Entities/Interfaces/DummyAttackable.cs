using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Holds a reference to a TargetDummy so that a torque can be applied on hit
    /// </summary>
    public class DummyAttackable : DamageableAttackable
    {
        [field: SerializeField] public TargetDummy Dummy { get; private set; }
        public override Vector2 Velocity
        {
            get => Vector2.zero;
            set {}
        }
    }
}

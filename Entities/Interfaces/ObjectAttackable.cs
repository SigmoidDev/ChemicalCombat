using Sigmoid.Rooms;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Causes a destructible object to immediately explode upon hit
    /// </summary>
    public class ObjectAttackable : MonoBehaviour, IAttackable
    {
        [field: SerializeField] public DestructibleObject Destructible { get; private set; }
        public Vector2 Position
        {
            get => transform.parent.position;
            set => transform.parent.position = value;
        }

        public Vector2 Velocity
        {
            get => Vector2.zero;
            set {}
        }

        public void ReceiveAttack(DamageContext context) => Destructible.Hit();
    }
}

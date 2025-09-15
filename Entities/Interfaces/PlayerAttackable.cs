using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Players
{
    /// <summary>
    /// Allows easy access to the player's position and average velocity
    /// </summary>
    public class PlayerAttackable : MonoBehaviour, IAttackable
    {
        public Vector2 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        private const float MAX_VELOCITY = 100f;

        public Vector2 Velocity
        {
            get
            {
                Vector2 velocity = Player.Instance.Velocity;
                return (velocity.x > MAX_VELOCITY
                || velocity.y > MAX_VELOCITY) ? Vector2.zero : velocity;
            }

            set => Player.Instance.Movement.Body.velocity = value;
        }

        public void AddForce(Vector2 force) => Player.Instance.Movement.ExternalVelocity += force;

        public void ReceiveAttack(DamageContext context) => Player.Instance.TakeDamage(context);
    }
}

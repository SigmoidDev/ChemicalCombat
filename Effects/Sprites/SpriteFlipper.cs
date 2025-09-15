using UnityEngine;

namespace Sigmoid.Effects
{
	public abstract class SpriteFlipper : MonoBehaviour
	{
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Animator animator;

        protected abstract Vector2 Velocity { get; }
        protected abstract bool Enabled { get; }

        private void Update()
        {
            if(!Enabled) return;

            //If facing right and moving left, face left
		    if(!sprite.flipX && Velocity.x < -0.01f) sprite.flipX = true;
            //If facing left and moving right, move right
			else if(sprite.flipX && Velocity.x > 0.01f) sprite.flipX = false;
            //Set animator's speed to the magnitude of velocity
			animator.SetFloat("Speed", Velocity.normalized.magnitude);
        }
	}
}

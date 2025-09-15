using System.Collections;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Teleports instantly to the target position with some cooldown
    /// </summary>
	public class TeleportMovement : MovementBase<TeleportParams>
	{
        public TeleportMovement(Enemy enemy, TeleportParams parameters) : base(enemy, parameters)
        {
            cooldown = parameters.TeleportInterval;
            delay = parameters.TeleportDelay;
            animation = parameters.TeleportAnimation;
        }
        public override void Initialise() => me.Agent.enabled = false;

		private readonly string animation;
		private readonly float cooldown;
		private readonly float delay;
        private float timer;

        public override void Update(Vector2 targetPosition, float deltaTime)
        {
            me.Sprite.flipX = Player.Instance.transform.position.x < me.transform.position.x;

            timer -= deltaTime;
            if(timer <= 0f)
            {
                timer = cooldown;
                me.StartCoroutine(Teleport(targetPosition));
            }
        }

        private IEnumerator Teleport(Vector2 targetPosition)
        {
            if(!string.IsNullOrEmpty(animation)) me.Animator.Play(animation);
            yield return new WaitForSeconds(delay);
            me.transform.position = targetPosition;
        }
	}
}

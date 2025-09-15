using System.Collections;
using Sigmoid.Game;
using UnityEngine.AI;
using UnityEngine;
using System;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// For enemies who jump or launch themselves in bursts every set interval (as compared to constant movement)
    /// </summary>
	public class BurstMovement : MovementBase<BurstParams>
	{
        public BurstMovement(Enemy enemy, BurstParams parameters) : base(enemy, parameters)
        {
            jumpForce = parameters.JumpForce * DifficultyManager.SpeedMultipler;
            moveDelay = parameters.JumpDelay;
            jumpCooldown = parameters.JumpInterval;
            animation = parameters.JumpAnimation;
            Initialise();
        }

        public override void Initialise() => me.Agent.enabled = false;

        private readonly float jumpForce;
		private readonly float moveDelay;
        private readonly float jumpCooldown;
        private readonly string animation;
        private float timer;

        public void AddDelay(float delay) => timer = delay;

        public override void Update(Vector2 targetPosition, float deltaTime)
        {
            timer -= deltaTime;
            if(timer <= 0f)
            {
                timer = jumpCooldown;
                me.StartCoroutine(Jump(targetPosition));
            }
        }

        public event Action<Vector2> OnJump;
        private IEnumerator Jump(Vector2 targetPosition)
        {
            if(!string.IsNullOrEmpty(animation)) me.Animator.Play(animation);
            yield return new WaitForSeconds(moveDelay);

            if(NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(me.transform.position, hit.position, NavMesh.AllAreas, path);
                if(path.corners.Length < 2) yield break;

                targetPosition = path.corners[1];
            }

            Vector2 direction = targetPosition - (Vector2) me.transform.position;
            me.Body.AddForce(me.SpeedMultiplier * jumpForce * direction.normalized, ForceMode2D.Impulse);
            OnJump?.Invoke(direction);
        }
    }
}

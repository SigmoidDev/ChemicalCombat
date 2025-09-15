using Sigmoid.Players;
using UnityEngine.AI;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Aims to sneak up behind the target, flanking to either side in the process
    /// </summary>
    public class SneakyTargeting : TargetingBase<SneakyParams>
    {
        public SneakyTargeting(Enemy enemy, SneakyParams parameters) : base(enemy, parameters)
        {
            preferredDistance = parameters.TargetDistance;
            swerveFactor = parameters.SwerveFactor;
        }

        private readonly float preferredDistance;
        private readonly float swerveFactor;

        public override bool GetDestination(IAttackable target, out Vector2 destination)
        {
            Vector2 myPosition = me.transform.position;
            Vector2 otherPosition = target.Position;

            bool facingLeft = Player.Instance.Sprite.flipX;
            bool isAbove = myPosition.y >= otherPosition.y;

            //Offset behind the target by an amount proportional to the preferredDistance
            Vector2 targetPosition = otherPosition + preferredDistance * (facingLeft ? Vector2.right : Vector2.left);
            Vector2 flankDirection = isAbove ? Vector2.up : Vector2.down;

            float distanceToTarget = Vector2.Distance(myPosition, targetPosition);
            float distanceToPlayer = Vector2.Distance(myPosition, otherPosition);
            if(distanceToTarget <= preferredDistance * 0.8f || distanceToPlayer <= preferredDistance * 0.8f)
            {
                me.Sprite.flipX = myPosition.x >= otherPosition.x;
                me.Agent.velocity = Vector2.zero;
                destination = myPosition;
                return true;
            }

            //Uses a Gaussian distribution curve on the distance to get a factor for how much to swerve around by
            float dx = myPosition.x - targetPosition.x;
            float distanceFactor = swerveFactor * (1f - Mathf.Exp(-0.25f * dx * dx));

            Vector2 velocity = (targetPosition - myPosition + distanceFactor * flankDirection).normalized;
            Vector2 preferredDestination = myPosition + velocity;

            //Prevent it from getting stuck on walls
            if(NavMesh.SamplePosition(preferredDestination, out NavMeshHit hit, 0.4f, NavMesh.AllAreas))
            {
                destination = hit.position;
                return false;
            }

            destination = otherPosition;
            return false;
        }
    }
}

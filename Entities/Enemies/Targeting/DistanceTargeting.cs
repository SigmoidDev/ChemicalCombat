using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Aims to maintain a constant distance from the target, moving away if too close
    /// </summary>
	public class DistanceTargeting : TargetingBase<DistanceParams>
    {
        public DistanceTargeting(Enemy enemy, DistanceParams parameters) : base(enemy, parameters)
        {
            targetDistance = parameters.TargetDistance;
            closeThreshold = targetDistance * parameters.CloseThreshold;
            farThreshold = targetDistance * parameters.FarThreshold;
        }

        private readonly float targetDistance;
        private readonly float closeThreshold;
        private readonly float farThreshold;

        public override bool GetDestination(IAttackable target, out Vector2 destination)
        {
            Vector2 targetPosition = target.Position;
            Vector2 deltaPosition = targetPosition - (Vector2) me.transform.position;
            float distance = deltaPosition.magnitude;

            if(distance > farThreshold)
            {
                Vector2 stayBack = targetDistance * deltaPosition.normalized;
                destination = targetPosition - stayBack;
                return false;
            }
            
            if(distance < closeThreshold)
            {
                Vector2 targetVelocity = target.Velocity.normalized;
                Vector2 retreatDirection = -deltaPosition.normalized;
                Vector2 finalDirection = (retreatDirection + targetVelocity * 0.3f).normalized;

                float moveFactor = targetDistance - distance;
                destination = (Vector2) me.transform.position + moveFactor * finalDirection;
                return false;
            }

            destination = me.transform.position;
            return true;
        }
    }
}

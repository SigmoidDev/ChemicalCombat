using UnityEngine;
using UnityEngine.AI;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Upon entering an aggro range, will charge towards and through the player for a certain distance
    /// </summary>
	public class PassthroughTargeting : TargetingBase<PassthroughParams>
    {
        public PassthroughTargeting(Enemy enemy, PassthroughParams parameters) : base(enemy, parameters)
        {
            continuationDistance = parameters.ContinuationDistance;
            deaggroRange = parameters.DeaggroRange;
            aggroRange = parameters.AggroRange;
            prediction = parameters.PredictionLength;
            finishDistance = parameters.FinishDistance;
            mask = parameters.SolidMask;
        }

        private readonly float continuationDistance;
        private readonly float deaggroRange;
        private readonly float aggroRange;
        private readonly float prediction;
        private readonly float finishDistance;
        private readonly LayerMask mask;

        private bool isCharging;
        private Vector2 currentDestination;

        public override bool GetDestination(IAttackable target, out Vector2 destination)
        {
            Vector2 myPosition = me.transform.position;
            Vector2 direction = target.Position - myPosition;

            if(isCharging)
            {
                float endDistance = Vector2.Distance(myPosition, currentDestination);
                if(endDistance < finishDistance)
                {
                    isCharging = false;
                    destination = myPosition;
                    return true;
                }

                //if too far away from the target, recalculate
                if(direction.magnitude > deaggroRange)
                {
                    destination = CalcualatePosition(myPosition, target);
                    currentDestination = destination;
                    return false;
                }

                destination = currentDestination;
                return false;
            }

            destination = CalcualatePosition(myPosition, target);
            currentDestination = destination;
            isCharging = direction.magnitude < aggroRange;
            return false;
        }

        /// <summary>
        /// Attempts to run in front of the target (based on their velocity)
        /// </summary>
        /// <param name="myPosition"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private Vector2 CalcualatePosition(Vector2 myPosition, IAttackable target)
        {
            Vector2 normalisedVelocity = target.Velocity.normalized;
            RaycastHit2D hit = Physics2D.Raycast(target.Position, normalisedVelocity, 10f, mask);
            float maxPrediction = hit ? hit.distance : prediction;
            Vector2 targetPosition = target.Position + maxPrediction * normalisedVelocity;

            Vector2 direction = targetPosition - myPosition;
            hit = Physics2D.Raycast(targetPosition, direction.normalized, continuationDistance, mask);
            float maxDistance = hit ? hit.distance - 0.3f : continuationDistance;

            float totalDistance = direction.magnitude + maxDistance;
            Vector2 destination = myPosition + totalDistance * direction.normalized;
            return NavMesh.SamplePosition(destination, out NavMeshHit meshHit, 2f, NavMesh.AllAreas) ? meshHit.position : target.Position;
        }
    }
}

using Sigmoid.Utilities;
using UnityEngine.AI;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Chooses a random direction and lerps it towards the target direction based on directionalBias
    /// </summary>
	public class RandomTargeting : TargetingBase<RandomParams>
    {
        public RandomTargeting(Enemy enemy, RandomParams parameters) : base(enemy, parameters)
        {
            targetDistance = parameters.TargetDistance;
            stepDistance = parameters.StepDistance;
            directionalBias = parameters.DirectionalBias;
            repather = parameters.RepathInterval.Create();
        }

        public readonly RepathInterval repather;
        private readonly float targetDistance;
        private readonly float stepDistance;
        private float directionalBias;
        private Vector2 storedDestination;

        public override bool GetDestination(IAttackable target, out Vector2 destination)
        {
            Vector2 myPosition = me.transform.position;
            float distance = Vector2.Distance(myPosition, storedDestination);

            repather.Update();
            if(!repather.ShouldRepath)
            {
                destination = storedDestination;
                return distance <= targetDistance;
            }
            repather.Reset();

            Vector2 direction = target.Position - myPosition;
            float targetAngle = Mathf.Atan2(direction.y, direction.x);

            for(int attempts = 0; attempts < 10; attempts++)
            {
                float randomAngle = Random.Range(-Mathf.PI, Mathf.PI);
                float angle = MathsHelper.LerpAngle(randomAngle, targetAngle, directionalBias);
                Vector2 moveDirection = stepDistance * new Vector2(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle)
                );

                Vector2 preferredDestination = (Vector2) me.transform.position + moveDirection;
                if(NavMesh.SamplePosition(preferredDestination, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                {
                    destination = hit.position;
                    storedDestination = destination;
                    return false;
                }
            }

            destination = me.transform.position;
            storedDestination = destination;
            return false;
        }

        public void OverrideBias(float newValue) => directionalBias = newValue;
    }
}

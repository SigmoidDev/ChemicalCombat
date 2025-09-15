using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Performs raycasts and reflection calculations to allow linear, bouncing movement
    /// </summary>
	public class BounceTargeting : TargetingBase<BounceParams>
    {
        public BounceTargeting(Enemy enemy, BounceParams parameters) : base(enemy, parameters)
        {
            groundMask = parameters.GroundMask;
            capsuleSize = parameters.CapsuleSize;
            edgeDistances = parameters.EdgeDistance;

            float angle = Random.Range(-Mathf.PI, Mathf.PI);
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        private readonly LayerMask groundMask;
        private readonly Vector2 edgeDistances;
        private readonly Vector2 capsuleSize;
        private Vector2 direction;
        private Vector2 storedDestination;

        public override bool GetDestination(IAttackable target, out Vector2 destination)
        {
            Vector2 myPosition = me.transform.position;
            if((Mathf.Abs(myPosition.x - storedDestination.x) <= edgeDistances.x
            &&  Mathf.Abs(myPosition.y - storedDestination.y) <= edgeDistances.y)
            || storedDestination == Vector2.zero) //represents an uninitialised destination (this is fine as (0, 0) in the labyrinth should always be inside a wall)
            {
                Vector2 rayOrigin = (Vector2) me.transform.position - direction.normalized;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, 30f, groundMask);
                if(!hit)
                {
                    //uh oh
                    destination = myPosition;
                    return false;
                }

                //reflection formula: r = v - 2(v ⋅ n)n
                float dot = Vector2.Dot(direction, hit.normal);
                direction -= 2 * dot * hit.normal;
                direction.Normalize();

                if(direction == Vector2.zero)
                {
                    float angle = Random.Range(-Mathf.PI, Mathf.PI);
                    direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                }

                rayOrigin = (Vector2) me.transform.position + direction.normalized;
                hit = Physics2D.CapsuleCast(rayOrigin, capsuleSize, CapsuleDirection2D.Horizontal, 0f, direction, 30f, groundMask);

                if(!hit)
                {
                    //uh oh
                    destination = myPosition;
                    return false;
                }
                storedDestination = hit.point;
            }

            destination = storedDestination;
            return false;
        }
    }
}

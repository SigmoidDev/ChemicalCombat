using System.Collections.Generic;
using Sigmoid.Utilities;
using Sigmoid.Rooms;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Patrols between a calculated set of points in a room
    /// </summary>
	public class FixedTargeting : TargetingBase<FixedParams>
    {
        private const float CORNER_RADIUS = 1.5f;

        public FixedTargeting(Enemy enemy, FixedParams parameters) : base(enemy, parameters)
        {
            patrolPoints = new List<Vector2>();
            pointAccuracy = parameters.PointAccuracy;

            if(!RoomGetter.TryGetByPosition(me.transform.position, out PhysicalRoom room)) return;
            Rect rect = room.Room.interior;

            switch(parameters.Type)
            {
                case PathType.Corners:
                {
                    patrolPoints.Add(new Vector2(rect.xMin, rect.yMin + CORNER_RADIUS));
                    patrolPoints.Add(new Vector2(rect.xMin + CORNER_RADIUS, rect.yMin));
                    patrolPoints.Add(new Vector2(rect.xMax - CORNER_RADIUS, rect.yMin));
                    patrolPoints.Add(new Vector2(rect.xMax, rect.yMin + CORNER_RADIUS));
                    patrolPoints.Add(new Vector2(rect.xMax, rect.yMax - CORNER_RADIUS));
                    patrolPoints.Add(new Vector2(rect.xMax - CORNER_RADIUS, rect.yMax));
                    patrolPoints.Add(new Vector2(rect.xMin + CORNER_RADIUS, rect.yMax));
                    patrolPoints.Add(new Vector2(rect.xMin, rect.yMax - CORNER_RADIUS));
                    break;
                }
            }

            numPoints = patrolPoints.Count;

            Vector2 myPosition = me.transform.position;
            float minDistance = Mathf.Infinity;
            for(int i = 0; i < patrolPoints.Count; i++)
            {
                float distance = Vector2.Distance(myPosition, patrolPoints[i]);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    currentIndex = i;
                }
            }
        }

        private readonly List<Vector2> patrolPoints;
        private readonly int numPoints; //might as well cache the length
        private readonly float pointAccuracy;
        private int currentIndex;
        private float timeWaited;

        public override bool GetDestination(IAttackable target, out Vector2 destination)
        {
            if(numPoints == 0)
            {
                destination = target.Position;
                return false;
            }

            bool closeEnough = Vector2.Distance(me.transform.position, patrolPoints[currentIndex]) < pointAccuracy;
            if(closeEnough)
            {
                timeWaited -= Time.deltaTime;
                if(timeWaited < 0f)
                {
                    timeWaited = Random.Range(0.3f, 0.7f);

                    currentIndex = Random.value < 0.5f ? //might randomly turn around
                    (int) MathsHelper.Mod(currentIndex - 1, numPoints) //(i-1) % n
                    : (currentIndex + 1) % numPoints;                  //(i+1) % n
                }
            }

            destination = patrolPoints[currentIndex];
            return closeEnough;
        }
    }
}

using Sigmoid.Rooms;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Follows other nearby enemies if present, otherwise runs directly towards the target
    /// </summary>
	public class ProtectiveTargeting : TargetingBase<ProtectiveParams>
    {
        public ProtectiveTargeting(Enemy enemy, ProtectiveParams parameters) : base(enemy, parameters)
        {
            if(!RoomGetter.TryGetByPosition(me.transform.position, out PhysicalRoom foundRoom)) return;
            enemyRoom = foundRoom as EnemyRoom;
        }

        private readonly EnemyRoom enemyRoom;
        public bool CanProtect { get; private set; }
        public bool IsProtecting { get; set; }

        public override bool GetDestination(IAttackable target, out Vector2 destination)
        {
            Vector2 positionSum = me.transform.position;
            int numNearby = 1;

            if(enemyRoom != null)
            {
                foreach(Enemy enemy in enemyRoom)
                {
                    if(enemy.EnemyType == me.EnemyType) continue;

                    Vector2 enemyPosition = enemy.transform.position;
                    if(Vector2.Distance(enemyPosition, me.transform.position) > 3f) continue;

                    positionSum += enemyPosition;
                    numNearby++;
                }
            }

            CanProtect = numNearby > 1;
            destination = numNearby == 1 ? target.Position :
                          positionSum / numNearby;
            return IsProtecting;
        }
    }
}

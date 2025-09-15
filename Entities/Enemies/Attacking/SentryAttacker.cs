using Sigmoid.Rooms;
using UnityEngine;

namespace Sigmoid.Enemies
{
    public class SentryAttacker : ProjectileAttacker
    {
        public SentryAttacker(Enemy enemy, ProjectileParams parameters) : base(enemy, parameters)
        {
            Vector2 myPosition = me.transform.position;
            if(!RoomGetter.TryGetByPosition(myPosition, out PhysicalRoom room)) return;

            Rect rect = room.Room.interior;
            float left = Mathf.Abs(myPosition.x - rect.xMin);
            float right = Mathf.Abs(myPosition.x - rect.xMax);

            me.transform.position = new Vector2(
                (left < right) ?
                rect.xMin : rect.xMax,
                myPosition.y
            );
        }

        private const float MAX_DISTANCE = 9f;
        private const float MAX_ANGLE = 10f;
        public override void Attack()
        {
            if(target == null) return;

            Vector2 myPosition = me.transform.position;
            Vector2 deltaPosition = target.Position - myPosition;

            //only attack the player when in a reasonable range
            if(deltaPosition.magnitude > MAX_DISTANCE)
            {
                timer = 0f;
                return;
            }

            float angle = Mathf.Atan(deltaPosition.y / deltaPosition.x);
            float absolute = Mathf.Abs(angle * Mathf.Rad2Deg);

            //only allow it to shoot basically straight
            if(absolute > MAX_ANGLE)
            {
                timer = 0f;
                return;
            }

            me.StartCoroutine(CAttack());
        }

        protected override Vector2 GetProjectileDirection(Vector2 spawnPosition) => target.Position.x < spawnPosition.x ? Vector2.left : Vector2.right;
        protected override Quaternion GetProjectileOrientation() => base.GetProjectileOrientation(); //TODO
    }
}

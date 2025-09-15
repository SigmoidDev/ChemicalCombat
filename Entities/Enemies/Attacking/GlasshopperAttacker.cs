using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Players;
using Sigmoid.Rooms;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Logic for the Glasshopper enemy to split into multiple every random amount of time
    /// </summary>
    public class GlasshopperAttacker : AttackerBase<GlasshopperParams>
    {
        private static WaitForSeconds _waitForSeconds0_2 = new WaitForSeconds(0.2f);

        public GlasshopperAttacker(Enemy enemy, GlasshopperParams parameters) : base(enemy, parameters)
        {
            filter = new ContactFilter2D();
            filter.SetLayerMask(me.HitboxMask);

            BurstMovement burst = (BurstMovement) me.Movement;
            burst.OnJump += MakeJumpNoise;
        }

        public override void Initialise()
        {
            if(splits > 1)
            {
                elapsed = float.NegativeInfinity;
                return;
            }

            splitTime = Random.Range(2f, 3f);
            elapsed = 0f;
            splits++;
        }

        private float splitTime;
        private float elapsed;
        private int splits;

        public override void Update(IAttackable target, float deltaTime)
        {
            elapsed += deltaTime;
            if(elapsed >= splitTime)
            {
                Initialise();
                Enemy clone = EnemySpawner.Instance.SpawnEnemy(me.EnemyType, me.transform.position);
                if(clone != null && RoomGetter.TryGetRoom(out PhysicalRoom playerRoom))
                {
                    if(playerRoom is EnemyRoom enemyRoom)
                        enemyRoom.RegisterEnemy(clone);
                }
            }
        }

        private readonly Collider2D[] buffer = new Collider2D[20];
        private ContactFilter2D filter;

        public override void Destroy()
        {
            BurstMovement burst = (BurstMovement) me.Movement;
            burst.OnJump -= MakeJumpNoise;
        }
        private void MakeJumpNoise(Vector2 direction) => me.TimedAudio.Play();

        public override IEnumerator Kill()
        {
            me.Animator.Play("Shatter");
            me.Body.velocity = Vector2.zero;
            me.ForceStop();

            yield return _waitForSeconds0_2;

            int numHits = Physics2D.OverlapCircle(me.transform.position, 3f, filter, buffer);
            for(int i = 0; i < numHits; i++)
            {
                if(!buffer[i].TryGetComponent(out IAttackable attackable)) continue;
                if(attackable is PlayerAttackable player)
                {
                    //This is to save on doing two different OverlapCircles with different radii
                    float distance = Vector2.Distance(me.transform.position, attackable.Position);
                    if(distance > 1f) continue;

                    DamageContext context = new DamageContext(1, DamageType.Physical, DamageCategory.Blunt, me);
                    player.ReceiveAttack(context);
                }
                else if(attackable is EnemyAttackable enemy && enemy.Enemy.EnemyType == me.EnemyType)
                    enemy.Enemy.Die();
            }

            yield return new WaitForAnimation(me.Animator, 0.9f);
        }
    }
}

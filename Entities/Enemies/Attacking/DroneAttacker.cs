using Sigmoid.Projectiles;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Flies in a fixed path around the edges of the current room, before chasing the player and dropping bombs upon detection
    /// </summary>
    public class DroneAttacker : AttackerBase<DroneParams>
    {
        public DroneAttacker(Enemy enemy, DroneParams parameters) : base(enemy, parameters)
        {
            targeting = (CompositeTargeting) me.BaseTargeting;
            movement = (PhaseMovement) me.Movement;
            detectionRange = parameters.DetectionRange;
            bombCooldown = parameters.BombCooldown;
            pool = ProjectileManager.Instance.Get(parameters.BombPrefab);

            me.Damageable.OnDamage += Alert;
            me.TimedAudio.Play();
        }

        private readonly float detectionRange;
        private readonly CompositeTargeting targeting;
        private readonly PhaseMovement movement;
        private int currentMode;

        private readonly ProjectilePool pool;
        private readonly float bombCooldown;
        private float bombTimer;

        public override void Update(IAttackable target, float deltaTime)
        {
            Vector2 myPosition = me.transform.position;

            if(currentMode == 1)
            {
                Vector2 predictedPosition = target.Position + 0.4f * target.Velocity;
                float distance1 = Vector2.Distance(predictedPosition, myPosition);

                bombTimer -= Time.deltaTime;
                if(distance1 < 3f && bombTimer < 0f)
                {
                    HitMask hitMask = target is PlayerAttackable ? HitMask.Players
                                    : target is EnemyAttackable ? HitMask.Enemies
                                    : HitMask.None;

                    bombTimer = bombCooldown;
                    Bomb bomb = (Bomb) pool.Fetch();
                    bomb.Initialise(me, pool, hitMask, me.transform.position);
                }
                return;
            }

            Vector2 targetPosition = target.Position;
            float distance2 = Vector2.Distance(targetPosition, myPosition);
            if(distance2 < detectionRange)
            {
                me.PlayOtherSound();

                currentMode = 1;
                targeting.SetMode(1);
                movement.UpdateBaseSpeed(parameters.SpeedBoost);
            }
        }

        public void Alert(int damage)
        {
            me.Damageable.OnDamage -= Alert;
            if(currentMode == 1) return;

            currentMode = 1;
            targeting.SetMode(1);
        }

        public override void Destroy() => me.Damageable.OnDamage -= Alert;
    }
}

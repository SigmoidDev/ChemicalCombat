using Sigmoid.Players;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Enemies
{
    public class HellhoundAttacker : AttackerBase<HellhoundParams>
    {
        public HellhoundAttacker(Enemy enemy, HellhoundParams parameters) : base(enemy, parameters)
        {
            howlSound = me.EnemyType.otherSound;
            aggroRange = parameters.AggroRange;
            chargeCooldown = parameters.ChargeCooldown;
            chargeTimer = 0f;

            movement = (CompositeMovement) me.BaseMovement;
            targeting = (CompositeTargeting) me.BaseTargeting;
            CurrentMode = 0;

            me.TimedAudio.Play();
            Physics2D.IgnoreCollision(me.Collider, Player.Instance.Collider, true);
        }

        private readonly ScriptableAudio howlSound;

        private readonly float aggroRange;
        private readonly float chargeCooldown;
        private float chargeTimer;

        private readonly CompositeMovement movement;
        private readonly CompositeTargeting targeting;

        private int currentMode;
        private int CurrentMode
        {
            get => currentMode;
            set
            {
                currentMode = value;
                movement.SetMode(value);
                targeting.SetMode(value);
            }
        }

        public override void Update(IAttackable target, float deltaTime)
        {
            Vector2 myPosition = me.transform.position;
            Vector2 targetPosition = target.Position;
            float distance = Vector2.Distance(targetPosition, myPosition);

            if(distance < 0.5f)
            {
                DamageContext context = new DamageContext(1, DamageType.Fire, DamageCategory.Blunt, me);
                target.ReceiveAttack(context);
            }

            //0 is Sneaking; 1 is Charging
            else if(CurrentMode == 0 && (chargeTimer -= deltaTime) < 0f && distance < aggroRange)
            {
                if(howlSound) AudioManager.Instance.Play(howlSound, me.transform.position, AudioChannel.Enemy);
                CurrentMode = 1;
            }

            else if(CurrentMode == 1 && me.AtDestination)
            {
                chargeTimer = chargeCooldown;
                CurrentMode = 0;
            }
        }
    }
}

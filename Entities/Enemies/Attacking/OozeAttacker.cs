using System.Collections;
using Sigmoid.Reactions;
using Sigmoid.Players;
using Sigmoid.Audio;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Complex controller for the Ooze, allowing for switching between modes based on distance
    /// </summary>
    public class OozeAttacker : AttackerBase<OozeParams>
    {
        private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
        private static WaitForSeconds _waitForSeconds0_1 = new WaitForSeconds(0.1f);

        public OozeAttacker(Enemy enemy, OozeParams parameters) : base(enemy, parameters)
        {
            attackSound = me.EnemyType.attackSound;
            burrowRange = parameters.BurrowRadius;
            jumpRange = parameters.JumpRadius;

            movement = (CompositeMovement) me.BaseMovement;
            targeting = (CompositeTargeting) me.BaseTargeting;
            CurrentMode = 0;

            Physics2D.IgnoreCollision(me.Collider, Player.Instance.Collider, true);
        }

        private readonly ScriptableAudio attackSound;
        private readonly float burrowRange;
        private readonly float jumpRange;

        private readonly CompositeMovement movement;
        private readonly CompositeTargeting targeting;

        private int currentMode;
        private int CurrentMode
        {
            get => currentMode;
            set
            {
                if(value == 0 && movement.CurrentMovement is BurstMovement burst)
                    burst.OnJump -= MakeJumpNoise;

                currentMode = value;
                movement.SetMode(value);
                targeting.SetMode(value);

                me.Animator.SetBool("Burrowing", value == 0);
                if(value == 1 && movement.CurrentMovement is BurstMovement burst2)
                {
                    burst2.OnJump += MakeJumpNoise;
                    burst2.AddDelay(0.3f);
                }
            }
        }

        private IAttackable target;
        public override void Update(IAttackable target, float deltaTime)
        {
            this.target = target;
            Vector2 myPosition = me.transform.position;
            Vector2 targetPosition = target.Position;
            float distance = Vector2.Distance(targetPosition, myPosition);

            if(distance < 0.5f)
            {
                if(attackSound) AudioManager.Instance.Play(attackSound, me.transform.position, AudioChannel.Enemy);
                DamageContext context = new DamageContext(1, DamageType.Toxic, DamageCategory.Blunt, me);
                target.ReceiveAttack(context);
            }

            //1 is Jumping; 0 is Burrowing
            else if(CurrentMode == 0 && distance < jumpRange)
                CurrentMode = 1;

            else if(CurrentMode == 1 && distance > burrowRange)
                CurrentMode = 0;
        }

        public override void Destroy()
        {
            if(currentMode == 1)
            {
                BurstMovement burst = (BurstMovement) movement.CurrentMovement;
                burst.OnJump -= MakeJumpNoise;
            }
        }

        private void MakeJumpNoise(Vector2 direction) => me.TimedAudio.Play();

        public override IEnumerator Kill()
        {
            me.Body.velocity = Vector2.zero;
            me.ForceStop();

            yield return _waitForSeconds0_1;
            me.Animator.Play("Explode");
            yield return _waitForSeconds0_5;

            HitMask hitMask = target is PlayerAttackable ? HitMask.Players
                            : target is EnemyAttackable ? HitMask.Enemies
                            : HitMask.None;

            ToxicSludge sludge = (ToxicSludge) ReactionPool.Instance.SludgePool.Fetch();
            sludge.Initialise(ReactionPool.Instance.SludgePool, me.transform.position, me, hitMask, 1f);
        }
    }
}

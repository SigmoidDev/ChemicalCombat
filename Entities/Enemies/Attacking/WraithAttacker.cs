using System.Collections;
using System;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Applies a slowness debuff when nearby, and dashes away upon being attacked
    /// </summary>
    public class WraithAttacker : AttackerBase<WraithParams>
    {
        public WraithAttacker(Enemy enemy, WraithParams parameters) : base(enemy, parameters)
        {
            dashCurve = parameters.DashCurve;
            enemy.Damageable.OnDamage += DashAway;
        }

        private readonly AnimationCurve dashCurve;
        public override void Destroy()
        {
            //i'm a bit concerned that this might not always work since it didn't on the phantoms for some unknown reason
            me.Damageable.OnDamage -= DashAway;
            if(currentSlowness != Guid.Empty) PlayerStats.MoveSpeed.RemoveModifier(currentSlowness);
        }

        private IAttackable target;
        public override void Update(IAttackable target, float deltaTime)
        {
            this.target = target;
            CalculateSlownessStacks();
        }

        private Guid currentSlowness;
        private bool hasIgnored = false;
        private int previousStacks = 0;
        private void CalculateSlownessStacks()
        {
            if(target is not PlayerAttackable)
            {
                if(!hasIgnored)
                {
                    hasIgnored = true;
                    if(currentSlowness != Guid.Empty)
                        PlayerStats.MoveSpeed.RemoveModifier(currentSlowness);
                }
                return;
            }

            Vector2 myPosition = me.transform.position;
            Vector2 targetPosition = target.Position;
            float distance = Vector2.Distance(myPosition, targetPosition);

            //Uses the same logic as the phantom
            int stacks;
            if(distance > 6.0f) stacks = 0;
            else if(distance > 4.0f) stacks = 1;
            else if(distance > 2.0f) stacks = 2;
            else stacks = 3;

            if(stacks != previousStacks)
            {
                if(currentSlowness != Guid.Empty) PlayerStats.MoveSpeed.RemoveModifier(currentSlowness);
                currentSlowness = stacks == 0 ? Guid.Empty : PlayerStats.MoveSpeed.AddModifier(speed => Mathf.Max(0f, speed - 6f * stacks));
            }

            previousStacks = stacks;
        }

        private bool isDashing = false;
        private void DashAway(int damage)
        {
            if(!isDashing) me.StartCoroutine(Dash());
        }

        private IEnumerator Dash()
        {
            isDashing = true;

            Vector2 myPosition = me.transform.position;
            Vector2 deltaPosition = target.Position - myPosition;
            Vector2 retreatDirection = -12f * deltaPosition.normalized;

            me.Flipper.enabled = false;
            me.PermitForces(0.5f, 2f);
            me.Body.AddForce(retreatDirection, ForceMode2D.Impulse);
            me.Damageable.HealthBar.ForceHide();

            for(float elapsed = 0f; elapsed < 0.5f; elapsed += Time.deltaTime)
            {
                me.SetAlpha(dashCurve.Evaluate(2f * elapsed));
                yield return null;
            }

            me.SetAlpha(1f);
            me.Flipper.enabled = true;
            me.Damageable.HealthBar.Refresh(me.Damageable.HealthPercent);
            isDashing = false;
        }
    }
}

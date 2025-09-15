using System.Collections;
using UnityEngine;

namespace Sigmoid.Enemies
{
    public class DroidAttacker : AttackerBase<DroidParams>
    {
        public DroidAttacker(Enemy enemy, DroidParams parameters) : base(enemy, parameters)
        {
            targeting = (ProtectiveTargeting) me.BaseTargeting;
            shieldMax = parameters.ShieldHealth;
            shieldDuration = parameters.ShieldDuration;
            shieldCooldown = parameters.ShieldCooldown;
            me.Damageable.OnDamage += TakeDamage;
        }

        private readonly ProtectiveTargeting targeting;
        private readonly int shieldMax;
        private readonly float shieldDuration;
        private readonly float shieldCooldown;

        private int shieldHealth;
        private float expiryTimer;
        private float cooldownTimer;

        public override void Update(IAttackable target, float deltaTime)
        {
            expiryTimer -= deltaTime;
            cooldownTimer -= deltaTime;

            if(expiryTimer < 0f) KillShield();
        }

        private void ActivateShield()
        {
            targeting.IsProtecting = true;
            shieldHealth = shieldMax;
            expiryTimer = shieldDuration;
        }

        private void KillShield()
        {
            targeting.IsProtecting = false;
            cooldownTimer = shieldCooldown;
        }

        private void TakeDamage(int damage)
        {
            if(targeting.IsProtecting) return;
            if(targeting.CanProtect && cooldownTimer < 0f) ActivateShield();
        }

        public override void Destroy() => me.Damageable.OnDamage -= TakeDamage;
    }
}

using Sigmoid.Projectiles;
using Sigmoid.Enemies;
using Sigmoid.Players;
using Sigmoid.Weapons;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// A type of reaction that holds reference to a ProjectilePool
    /// </summary>
    public abstract class ProjectileReaction : Reaction
    {
        public abstract Projectile Projectile { get; }
        public abstract float Cooldown { get; }
        protected float cooldown;
        public override void Update(float deltaTime) => cooldown -= deltaTime;

        private ProjectilePool pool;
        protected ProjectilePool Pool => pool == null ? pool = ProjectileManager.Instance.Get(Projectile) : pool;
    }

    public class WindReaction : SpawnReaction<DetonatedPool, DetonatedEffect>
    {
        public override DetonatedPool Pool => ReactionPool.Instance.CloudPool;
        public override float Cooldown => 0.2f;
    }

    public class DiamondReaction : ProjectileReaction
    {
        public override Projectile Projectile => ReactionPool.Instance.DiamondProjectile;
        public override float Cooldown => 0.5f;

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            if(cooldown > 0f) return;
            cooldown = Cooldown;

            LinearProjectile projectile = (LinearProjectile) Pool.Fetch();
            projectile.Initialise(Player.Instance, Pool, HitMask.Enemies, 2f);

            Vector2 direction = hit.projectile.Body.velocity.normalized;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, -direction);
            projectile.Throw(hit.point, rotation, direction, 0f);
        }
    }

    public class GlassReaction : ProjectileReaction
    {
        public override Projectile Projectile => ReactionPool.Instance.GlassProjectile;
        public override float Cooldown => 0.5f;

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            if(cooldown > 0f) return;
            cooldown = Cooldown;

            for(int i = 0; i < 3; i++)
            {
                Thorn projectile = (Thorn) Pool.Fetch();
                Vector2 randomForce = new Vector2(Random.Range(-4f, 4f), Random.Range(3f, 4f));
                projectile.Initialise(Pool, hit.point, randomForce);
            }
        }
    }

    public class RubberReaction : ProjectileReaction
    {
        public override Projectile Projectile => null;
        public override float Cooldown => 0.4f;

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            if(cooldown > 0f) return;
            cooldown = Cooldown;

            Potion oldPotion = (Potion) hit.projectile;
            Weapon weapon1 = WeaponManager.Instance.Weapons[0];

            Potion potion = weapon1.Fetch().Initialise(weapon1, oldPotion.Chemical, oldPotion.DamageMultiplier);
			potion.Launch(hit.point, hit.point + 2f * hit.projectile.Body.velocity.normalized);
            potion.SetImmunityPeriod(0.33f);
        }
    }

    public class ElectricityReaction : ProjectileReaction
    {
        public override Projectile Projectile => ReactionPool.Instance.LightningBolt;
        public override float Cooldown => 1.0f;

        public override void React(DamageableAttackable source, ProjectileHit hit, float damageMultiplier)
        {
            if(cooldown > 0f) return;
            cooldown = Cooldown;

            ElectricBolt bolt = (ElectricBolt) Pool.Fetch();
            bolt.Initialise(Pool, source);
        }
    }
}

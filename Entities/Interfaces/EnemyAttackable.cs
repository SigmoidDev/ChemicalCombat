using Sigmoid.Upgrading;
using Sigmoid.Weapons;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Modifies damage taken based on an enemy's weaknesses/resistances
    /// </summary>
    public class EnemyAttackable : DamageableAttackable
    {
        [field: SerializeField] public Enemy Enemy { get; private set; }
        public override Vector2 Velocity
        {
            get => Enemy.Velocity;
            set => Enemy.Velocity = value;
        }

        public override float ModifyDamage(float original, DamageContext context) => original
        * Enemy.EnemyType.GetTypeEffectiveness(context.type)
        * Enemy.EnemyType.GetCategoryEffectiveness(context.category)
        * Enemy.Damageable.BuffReceiver.DamageTaken
        * ((Perks.Has(Perk.Efficient) && Damageable.IsHealthy) ? 1.25f : 1f);

        public override void OnDamage(DamageContext finalContext) => WeaponManager.Instance.RecordDamage((int)finalContext.damage);
    }
}

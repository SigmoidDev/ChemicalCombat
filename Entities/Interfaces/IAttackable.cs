using Sigmoid.Upgrading;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Describes any object, entity or player that can be hit by an attack
    /// </summary>
	public interface IAttackable
	{
        public void ReceiveAttack(DamageContext context);
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
	}

    /// <summary>
    /// A type of Attackable for enemies and target dummies
    /// </summary>
    public abstract class DamageableAttackable : MonoBehaviour, IAttackable
    {
        [field: SerializeField] public Damageable Damageable { get; private set; }
        public Vector2 Position
        {
            get => transform.parent.position;
            set => transform.parent.position = value;
        }
        public abstract Vector2 Velocity { get; set; }

        public virtual void ReceiveAttack(DamageContext context)
        {
            float damage = ModifyDamage(context.damage, context);
            if(damage <= 0f) return;

            bool wasCrit = 100f * Random.value < PlayerStats.CritChance;
            if(wasCrit) damage *= 1.5f;
            damage *= PerkHandler.Instance.ComboBonus;

            DamageContext modifiedContext = new DamageContext(Mathf.Ceil(damage), context.type, context.category, context.source);
            Damageable.Damage(modifiedContext);
            OnDamage(modifiedContext);
        }

        public virtual float ModifyDamage(float original, DamageContext context) => original;
        public virtual void OnDamage(DamageContext finalContext){}
    }

    [System.Flags]
    public enum HitMask
    {
        None = 0,
        Players = 1,
        Enemies = 2,
        Objects = 4
    }
}

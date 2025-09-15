using System.Collections.Generic;
using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Upgrading;
using Sigmoid.Enemies;
using Sigmoid.Cameras;
using Sigmoid.Players;
using Sigmoid.Audio;
using Sigmoid.Buffs;
using UnityEngine;

namespace Sigmoid.Reactions
{
    /// <summary>
    /// Specifically a damage-dealing explosion that may or may not inflict DoTs
    /// </summary>
    public class Explosion : DetonatedEffect
    {
        [SerializeField] protected List<ScriptableAudio> sounds;
		[SerializeField] protected List<DotType> dots;
        [SerializeField] protected DamageType type;
		[SerializeField] protected float duration;
		[SerializeField] protected float radius;
        [SerializeField] protected float power;
		[SerializeField] protected float damage;

        private float damageMultiplier;
        protected float EffectiveDamage => damage * damageMultiplier * (Perks.Has(Perk.Incendiary) ? 1.25f : 1f);
		protected override float EffectiveRadius => radius * PlayerStats.ExplosionRadius;
        protected float FalloffBase => Perks.Has(Perk.Incendiary) ? 1.5f : 2.0f;

        public override void Initialise(DetonatedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
            base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            transform.localScale = PlayerStats.ExplosionRadius * Vector2.one;
            this.damageMultiplier = damageMultiplier;
        }

        protected override void Effect(IAttackable attackable)
        {
            if((hitMask & HitMask.Enemies) != 0 && attackable is DamageableAttackable damageable)
            {
                float distance = Vector2.Distance(damageable.Position, transform.position);
                float calculatedDamage = EffectiveDamage * MathsHelper.DistanceFalloff(distance, EffectiveRadius, FalloffBase);

                DamageContext context = new DamageContext(calculatedDamage, type, DamageCategory.Explosion, owner);
                damageable.ReceiveAttack(context);

                foreach(DotType dot in dots) damageable.Damageable.DotReceiver.InflictDot(dot);
            }
            else if(((hitMask & HitMask.Players) != 0 && attackable is PlayerAttackable)
            || ((hitMask & HitMask.Objects) != 0 && attackable is ObjectAttackable))
            {
                if(Vector2.Distance(attackable.Position, transform.position) > radius) return;

                DamageContext context = new DamageContext(1, type, DamageCategory.Explosion, owner);
                attackable.ReceiveAttack(context);
            }
        }

        protected override IEnumerator OnDetonated()
        {
            foreach(ScriptableAudio sound in sounds)
                AudioManager.Instance.Play(sound, transform.position, AudioChannel.Sound);

            float distancePlayer = Vector2.Distance(MainCamera.CameraPosition, transform.position);
			MainCamera.ShakeScreen(duration * 1.5f, power * MathsHelper.DistanceFalloff(distancePlayer, EffectiveRadius * 1.5f, 3f), 0.98f);

			yield return new WaitForSeconds(duration);
			pool.Release(this);
        }
    }
}

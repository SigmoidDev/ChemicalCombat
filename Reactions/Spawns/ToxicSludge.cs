using Sigmoid.Reactions;
using Sigmoid.Players;
using Sigmoid.Effects;
using UnityEngine;

namespace Sigmoid.Enemies
{
	public class ToxicSludge : SpawnedEffect
	{
        [SerializeField] private FadeEffect fadeEffect;

        //cancels out the effects of EffectDuration
        protected override float Lifetime => 3f / PlayerStats.EffectDuration;
        public override void Initialise(SpawnedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
            base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            fadeEffect.Initialise(transform.position);
        }

        private void OnTriggerEnter2D(Collider2D other)
		{
			if(other.gameObject.layer != LayerMask.NameToLayer("Hitboxes")) return;
			if(!other.TryGetComponent(out PlayerAttackable attackable)) return;
			attackable.ReceiveAttack(new DamageContext(1f, DamageType.Toxic, DamageCategory.DoT, owner));
		}

        public override void OnEnter(DamageableAttackable damageable){}
        public override void OnExit(DamageableAttackable damageable){}
	}
}

using Sigmoid.Enemies;
using Sigmoid.Effects;
using Sigmoid.Buffs;
using UnityEngine;

namespace Sigmoid.Reactions
{
	public class AcidPool : SpawnedEffect
	{
        [SerializeField] private FadeEffect fadeEffect;

        protected override float Lifetime => 4f;
        public override void Initialise(SpawnedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
            base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            fadeEffect.Initialise(transform.position);
        }

        public override void OnEnter(DamageableAttackable damageable) => damageable.Damageable.DotReceiver.InflictDot(DotType.Dissolving);
        public override void OnExit(DamageableAttackable damageable) => damageable.Damageable.DotReceiver.RemoveDot(DotType.Dissolving, 1f);
	}
}

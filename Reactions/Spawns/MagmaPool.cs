using Sigmoid.Enemies;
using Sigmoid.Effects;
using Sigmoid.Buffs;
using UnityEngine;

namespace Sigmoid.Reactions
{
	public class MagmaPool : SpawnedEffect
	{
        [SerializeField] private FadeEffect spriteFade;
        [SerializeField] private FadeEffect shadowFade;

        protected override float Lifetime => 5f;
        public override void Initialise(SpawnedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
            base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            spriteFade.Initialise(transform.position);
            shadowFade.Initialise(transform.position);
        }

        public override void OnEnter(DamageableAttackable damageable) => damageable.Damageable.DotReceiver.InflictDot(DotType.Burning);
        public override void OnExit(DamageableAttackable damageable) => damageable.Damageable.DotReceiver.RemoveDot(DotType.Burning, 2f);
	}
}

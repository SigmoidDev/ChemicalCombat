using Sigmoid.Players;
using Sigmoid.Enemies;
using Sigmoid.Buffs;
using UnityEngine;

namespace Sigmoid.Reactions
{
	public class PoisonCloud : SpawnedEffect
	{
        [SerializeField] private ParticleSystem particles;

        protected override float Lifetime => 3f;
        public override void Initialise(SpawnedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
            base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            ParticleSystem.MainModule main = particles.main;
            main.duration = Lifetime * PlayerStats.EffectDuration + 1f;
            particles.Play();
        }

        public override void OnEnter(DamageableAttackable damageable) => damageable.Damageable.DotReceiver.InflictDot(DotType.Poisoned);
        public override void OnExit(DamageableAttackable damageable) => damageable.Damageable.DotReceiver.RemoveDot(DotType.Poisoned, 1f);
    }
}

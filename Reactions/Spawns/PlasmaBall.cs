using System.Collections.Generic;
using Sigmoid.Effects;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Reactions
{
	public class PlasmaBall : SpawnedEffect
	{
        [SerializeField] private Rigidbody2D body;
		[SerializeField] private FadeEffect fadeEffect;
		[SerializeField] private ZapPool zapPool;

        protected override float Lifetime => 5f;
        public override void Initialise(SpawnedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
            activeTargets = new HashSet<DamageableAttackable>();
            base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            fadeEffect.Initialise(transform.position);
        }

        public void SetMoving(Vector2 startVelocity) => body.velocity = startVelocity;

        private HashSet<DamageableAttackable> activeTargets;
        public override void OnEnter(DamageableAttackable damageable) => activeTargets.Add(damageable);
        public override void OnExit(DamageableAttackable damageable) => activeTargets.Remove(damageable);

        private float zapTimer;
        private void Update()
        {
            zapTimer -= Time.deltaTime;
            if(zapTimer > 0f) return;
            zapTimer = 0.5f;

            foreach(DamageableAttackable damageable in activeTargets)
            {
                ZapLine line = zapPool.Fetch();
                line.Initialise(zapPool, transform.position, damageable.Position + 0.5f * Vector2.up);

                DamageContext context = new DamageContext(2, DamageType.Light, DamageCategory.Blunt, owner);
                damageable.ReceiveAttack(context);
                damageable.Damageable.DotReceiver.InflictDot(Buffs.DotType.Burning, 1f, 1);
            }
        }
    }
}

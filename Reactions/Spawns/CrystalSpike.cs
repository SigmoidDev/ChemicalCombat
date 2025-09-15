using System.Collections;
using Sigmoid.Enemies;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Reactions
{
	public class CrystalSpike : SpawnedEffect
	{
        private static WaitForSeconds _waitForSeconds0_1 = new WaitForSeconds(0.1f);
        [SerializeField] private Animator animator;
		[SerializeField] private LayerMask layerMask;
		private readonly Collider2D[] buffer = new Collider2D[40];

        protected override float Lifetime => 1.5f;
        public override void Initialise(SpawnedPool pool, Vector2 point, IDamageSource owner, HitMask hitMask, float damageMultiplier)
        {
            base.Initialise(pool, point, owner, hitMask, damageMultiplier);
            animator.Play("Crystallise");

            int numHits = Physics2D.OverlapCircleNonAlloc(transform.position, 1.4f, buffer, layerMask);
			for(int i = 0; i < numHits; i++)
			{
                if(!buffer[i].TryGetComponent(out IAttackable attackable)) continue;
				StartCoroutine(HitDelayed(attackable));
			}
        }

        private IEnumerator HitDelayed(IAttackable attackable)
        {
            yield return _waitForSeconds0_1;
            if((hitMask & HitMask.Enemies) != 0 && attackable is DamageableAttackable damageable)
            {
                DamageContext physicalDamage = new DamageContext(6, DamageType.Physical, DamageCategory.Blunt, owner);
                DamageContext lightDamage = new DamageContext(6, DamageType.Light, DamageCategory.Blunt, owner);
                damageable.ReceiveAttack(physicalDamage);
                damageable.ReceiveAttack(lightDamage);
            }
            else if(((hitMask & HitMask.Players) != 0 && attackable is PlayerAttackable)
            || ((hitMask & HitMask.Objects) != 0 && attackable is ObjectAttackable))
            {
                DamageContext context = new DamageContext(1, DamageType.Physical, DamageCategory.Blunt, owner);
                attackable.ReceiveAttack(context);
            }
        }

        public override void OnEnter(DamageableAttackable damageable){}
        public override void OnExit(DamageableAttackable damageable){}
	}
}

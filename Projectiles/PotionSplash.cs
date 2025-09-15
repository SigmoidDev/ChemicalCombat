using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Upgrading;
using Sigmoid.Chemicals;
using Sigmoid.Weapons;
using Sigmoid.Players;
using Sigmoid.Enemies;
using UnityEngine;

namespace Sigmoid.Projectiles
{
	public class PotionSplash : MonoBehaviour
	{
        private static WaitForSeconds _waitForSeconds1_5 = new WaitForSeconds(1.5f);
        private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
        [SerializeField] private ParticleSystem system;
        private Weapon weapon;
        private Potion potion;

		public void Initialise(Vector2 position, Weapon weapon, Potion potion)
        {
            transform.position = position;
            transform.localScale = 0.5f * PlayerStats.SplashRadius * Vector2.one;

            this.weapon = weapon;
            this.potion = potion;
            SetColourGradient(potion.Chemical);
            StartCoroutine(PlayAnimation(Perks.Has(Perk.Lingering)));
        }

        /// <summary>
        /// Sets the particle system to use the correct colouring for the chemical provided
        /// </summary>
        /// <param name="chemical"></param>
        private void SetColourGradient(Chemical chemical)
        {
            ScriptableChemical info = ChemicalManager.Get(chemical);
            ParticleSystem.MainModule module = system.main;
            ParticleSystem.MinMaxGradient minMax = module.startColor;

            minMax.colorMin = info.colours[1];
            minMax.colorMax = info.colours[3];
            module.startColor = minMax;
        }

        private readonly Collider2D[] buffer = new Collider2D[40];
        private IEnumerator PlayAnimation(bool shouldLinger)
        {
            bool isVolatile = Perks.Has(Perk.Volatile);

            int numSplashes = shouldLinger ? 2 : 1;
            for(int j = 0; j < numSplashes; j++)
            {
                system.Play();
                int hits = Physics2D.OverlapCircleNonAlloc(transform.position, PlayerStats.SplashRadius * 0.5f, buffer, WeaponManager.Instance.HitboxMask);
                for(int i = 0; i < hits; i++)
                {
                    if(!buffer[i].TryGetComponent(out IAttackable attackable) || attackable is PlayerAttackable) continue;

                    ProjectileHit info = new ProjectileHit(potion, transform.position, buffer[i].ClosestPoint(potion.transform.position), potion.Displacement);
                    DamageContext context = new DamageContext(PlayerStats.BaseDamage * potion.DamageMultiplier, DamageType.Physical, DamageCategory.Blunt, potion.Source);

                    attackable.ReceiveAttack(context);

                    if(attackable is not DamageableAttackable damageable) continue;
                    damageable.Damageable.ChemicalReactor.InflictChemical(potion.Chemical, (reaction) =>
                    {
                        if(reaction == null) return;
                        reaction.React(damageable, info, potion.DamageMultiplier);
                        weapon.React(info, reaction);
                    });

                    if(isVolatile && Random.value < 0.25f)
                    {
                        Chemical bonus = ChemicalManager.GetRandom();
                        damageable.Damageable.ChemicalReactor.InflictChemical(bonus, (reaction) =>
                        {
                            if(reaction == null) return;
                            reaction.React(damageable, info, potion.DamageMultiplier);
                            weapon.React(info, reaction);
                        });
                    }

                    if(attackable is not DummyAttackable dummy) continue;
                    float distance = Vector2.Distance(transform.position, attackable.Position);
                    float falloff = MathsHelper.DistanceFalloff(distance / Mathf.Sqrt(0.5f * PlayerStats.SplashRadius), 3.0f, 8.0f);
                    float torque = MathsHelper.ForceToTorque(dummy.Dummy.Body, 0.5f * falloff * info.velocity.normalized, info.point);
                    dummy.Dummy.Body.AddTorque(torque);
                }

                if(shouldLinger) yield return _waitForSeconds0_5;
            }

            yield return _waitForSeconds1_5;
            weapon.Pool.Release(this);
        }
	}
}

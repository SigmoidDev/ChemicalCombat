using System.Collections.Generic;
using System.Collections;
using Sigmoid.Players;
using Sigmoid.Buffs;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Allows Damageables to take damage from various damage over time effects
    /// </summary>
	public class DotReceiver : MonoBehaviour
	{
		[SerializeField] private Damageable damageable;

        private Dictionary<DotType, DotStatus> dots;
        private float effectiveness;

        private void Awake() => ResetDoTs();
        public void ResetDoTs(ScriptableEnemy enemy = null)
        {
            dots = new Dictionary<DotType, DotStatus>
			{
				{ DotType.Burning, new DotStatus(0, 0f) },
				{ DotType.Corroding, new DotStatus(0, 0f) },
				{ DotType.Dissolving, new DotStatus(0, 0f) },
				{ DotType.Poisoned, new DotStatus(0, 0f) }
			};
            effectiveness = enemy != null ? enemy.GetCategoryEffectiveness(DamageCategory.DoT) : 1f;
        }

        /// <summary>
        /// Inflicts n stacks of a given DoT, set to expire after some duration (indefinite by default)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="duration"></param>
        /// <param name="number"></param>
        public void InflictDot(DotType type, float duration = 0f, int number = 1)
        {
            //Can't apply the dot if this has been deleted
            if(!gameObject.activeInHierarchy) return;

            dots[type].stacks += number;
			dots[type].cooldown = 0f;

            //Duration of 0s corresponds to indefinite, so avoid that
            if(duration > 0f) RemoveDot(type, duration, number);
        }

        /// <summary>
        /// Removes a DoT after a set number of seconds, provided the enemy is still alive
        /// </summary>
        /// <param name="type"></param>
        /// <param name="after"></param>
        /// <param name="number"></param>
        public void RemoveDot(DotType type, float after, int number = 1)
        {
            if(!gameObject.activeInHierarchy) return;

            StartCoroutine(RemoveDot(type, after));
            IEnumerator RemoveDot(DotType type, float delay)
            {
                yield return new WaitForSeconds(delay);
                if(!gameObject.activeInHierarchy) yield break;
                dots[type].stacks -= number;
            }
        }

        /// <summary>
        /// Iterates over all active DoTs and deals damage depending on the number of stacks
        /// </summary>
		private void Update()
		{
            foreach(KeyValuePair<DotType, DotStatus> pair in dots)
            {
                DotType dot = pair.Key;
                DotStatus status = pair.Value;

                status.cooldown -= Time.deltaTime;
				if(status.stacks > 0 && status.cooldown <= 0f)
				{
					ScriptableDot type = DotManager.Get(dot);
					status.cooldown = type.interval / (effectiveness * PlayerStats.DoTRate);

					int damage = type.damage * status.stacks;
                    DamageContext context = new DamageContext(damage, type.type, DamageCategory.DoT, Player.Instance);

                    float flashDuration = 0.12f;
                    if(status.cooldown < 0.16f)
                        flashDuration = 0.75f * status.cooldown;

                    FlashData flash = new FlashData(type.colour, flashDuration);
                    damageable.Damage(context, flash);
				}
            }
		}

        /// <summary>
        /// Provides encapsulated access to the DoT dictionary
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(System.Action<DotType, DotStatus> action)
        {
            foreach(KeyValuePair<DotType, DotStatus> pair in dots)
                action?.Invoke(pair.Key, pair.Value);
        }
	}

    public class DotStatus
	{
		public int stacks;
		public float cooldown;

		public DotStatus(int stacks, float cooldown)
		{
			this.stacks = stacks;
			this.cooldown = cooldown;
		}
	}
}

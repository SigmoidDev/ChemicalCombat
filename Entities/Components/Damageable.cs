using System;
using Sigmoid.Buffs;
using UnityEngine;

namespace Sigmoid.Enemies
{
	public class Damageable : MonoBehaviour
	{
        [SerializeField] private SpriteRenderer sprite;

		[field: SerializeField] public HealthBar HealthBar { get; private set; }
		[field: SerializeField] public ChemicalReactor ChemicalReactor { get; private set; }
		[field: SerializeField] public DotReceiver DotReceiver { get; private set; }
		[field: SerializeField] public BuffReceiver BuffReceiver { get; private set; }
		[field: SerializeField] public HitFlash HitFlash { get; private set; }

		private int maxHealth;
        public bool IsHealthy => health == maxHealth;

		private int health;
        public int Health => health;
        public float HealthPercent => (float) health / maxHealth;

		private bool alive;
		public bool IsDead => !alive;

		public event Action<int> OnDamage;
		public event Action OnDeath;

		private void Awake() => Initialise(1);
        public void Initialise(int health)
        {
			maxHealth = health;
            this.health = health;
			alive = true;

            sprite.color = Color.white;
            if(HealthBar != null) HealthBar.ForceHide();
            ChemicalReactor.Initialise();
        }

        public void Damage(DamageContext damage, FlashData flash = null)
		{
            if(!gameObject.activeInHierarchy) return;
			HitFlash.Flash(flash);

			int roundedDamage = (int) damage.damage;
			health -= roundedDamage;

			if(alive && health <= 0)
			{
				alive = false;
				OnDeath?.Invoke();
			}

			if(HealthBar != null) HealthBar.Refresh(HealthPercent);
			OnDamage?.Invoke(roundedDamage);
		}
	}
}

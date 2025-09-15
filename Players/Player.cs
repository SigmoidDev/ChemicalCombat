using System.Collections.Generic;
using Sigmoid.Utilities;
using Sigmoid.Upgrading;
using Sigmoid.Cameras;
using Sigmoid.Enemies;
using Sigmoid.Weapons;
using Sigmoid.Audio;
using Sigmoid.Game;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Players
{
	public class Player : Singleton<Player>, IDamageSource
	{
        public string DisplayName => "Player";

		[field: SerializeField] public SpriteRenderer Sprite { get; private set; }
		[field: SerializeField] public Animator Animator { get; private set; }
		[field: SerializeField] public CapsuleCollider2D Collider { get; private set; }
		[field: SerializeField] public Movement Movement { get; private set; }
		[field: SerializeField] public Interactor Interactor { get; private set; }
		[field: SerializeField] public HitFlash HitFlash { get; private set; }
		[field: SerializeField] public PlayerAttackable Attackable { get; private set; }

        [SerializeField] private AudioPlayer hurtSound;
        [SerializeField] private AudioPlayer healSound;

		public int Health { get; private set; }
        public IDamageSource LastDamager { get; private set; }
        public event System.Action<DamageContext, bool> OnHit;
        public event System.Action<int> OnHealthChanged;
        public event System.Action OnDeath;

		private float timeSinceHit;
		public bool IsInvulnerable => timeSinceHit <= PlayerStats.ImmunityPeriod || (Movement.IsSliding && Perks.Has(Perk.Athletic));

		public bool IsAlive { get; private set; }
		private void Awake()
		{
			Health = PlayerStats.MaxHealth;
			IsAlive = true;
		}

        private Vector2 lastPosition;
        private Vector2 averageVelocity;
        public Vector2 Velocity => averageVelocity;
        private readonly Queue<Vector2> velocitySamples = new Queue<Vector2>();
        private const int NUM_SAMPLES = 30;

        private void Update()
        {
            timeSinceHit += Time.deltaTime;
            if(PlayerUI.InMenu) return;

            Vector2 currentVelocity = ((Vector2) transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            velocitySamples.Enqueue(currentVelocity);
            if(velocitySamples.Count > NUM_SAMPLES)
                velocitySamples.Dequeue();

            averageVelocity = Vector2.zero;
            foreach(Vector2 sample in velocitySamples)
                averageVelocity += sample;

            //prevents NaNs if no samples are taken
            averageVelocity = velocitySamples.Count == 0 ? Vector2.zero : averageVelocity / velocitySamples.Count;
        }

        /// <summary>
        /// Heals the player by a certain number of health points (1 by default)
        /// </summary>
        /// <param name="amount"></param>
		public void Heal(int amount = 1)
		{
            Health = Mathf.Min(Health + amount, PlayerStats.MaxHealth);
            OnHealthChanged?.Invoke(Health);
			healSound.Play();
		}

        /// <summary>
        /// Damages the player, providing an optional source for achievement purposes, and calls OnDeath if health reaches 0
        /// </summary>
        /// <param name="damager"></param>
		public void TakeDamage(DamageContext context)
		{
			if(IsInvulnerable || !IsAlive) return;

            float random = Random.value * 100f;
            if(random < PlayerStats.DodgeChance)
            {
                OnHit?.Invoke(context, false);
                return;
            }

			Health -= (int) context.damage;
            OnHit?.Invoke(context, true);
            OnHealthChanged?.Invoke(Health);
            hurtSound.Play();

            LastDamager = context.source;
			timeSinceHit = 0f;

			MainCamera.FlashScreen(0.5f, ScreenFlash.HurtStartColour, ScreenFlash.HurtEndColour);
			HitFlash.Flash();

            if(IsAlive && Health <= 0)
            {
                OnDeath?.Invoke();
                IsAlive = false;

                Animator.Play("Die");
                Movement.Body.velocity = Vector2.zero;

                if(SceneLoader.Instance.CurrentScene != GameScene.Tutorial)
                    WeaponManager.Instance.gameObject.SetActive(false);
            }
		}

        /// <summary>
        /// Respawns the player
        /// </summary>
        public void Revive()
        {
            Health = PlayerStats.MaxHealth;
			IsAlive = true;

            Animator.Play("Idle");
        }
    }
}

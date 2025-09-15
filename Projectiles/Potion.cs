using Sigmoid.Utilities;
using Sigmoid.Chemicals;
using Sigmoid.Enemies;
using Sigmoid.Players;
using Sigmoid.Weapons;
using Sigmoid.Effects;
using Sigmoid.Audio;
using UnityEngine;
using Sigmoid.Upgrading;

namespace Sigmoid.Projectiles
{
    public class Potion : Projectile
    {
        public override IDamageSource Source => Player.Instance;

        [SerializeField] private ScriptableAudio breakSound;
		[field: SerializeField] public PaletteSwapper Swapper { get; private set; }
        public Weapon Weapon { get; private set; }
		public Chemical Chemical { get; private set; }
        public float DamageMultiplier { get; private set; }

		public Potion Initialise(Weapon weapon, Chemical chemical, float damageMultiplier)
		{
			Weapon = weapon;
			Chemical = chemical;
            immunity = 0f;
            DamageMultiplier = damageMultiplier;

			Colourise();
			return this;
		}

        public void Colourise()
        {
            ScriptableChemical info = ChemicalManager.Get(Chemical);
            Color[] colours = new Color[]
            {
                info.colours[1],
                info.colours[2],
                info.colours[3]
            };
            Swapper.UpdateLUT(Swapper.Originals, colours);
        }

        private float lifetime;
        private float immunity;
        private bool used;

        public void SetImmunityPeriod(float duration) => immunity = duration;

        private void Update()
        {
            Body.velocity -= new Vector2(0f, GRAVITY * Time.deltaTime);
            lifetime -= Time.deltaTime;
            immunity -= Time.deltaTime;
            if(lifetime <= 0f) Splash();
        }

		private const float GRAVITY = 10f;

        private Vector2 displacement;
        public Vector2 Displacement => displacement;

        /// <summary>
        /// See https://www.desmos.com/calculator/u4flocuids for some of the maths, although I'm not actually sure how I ended up with this
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
		public void Launch(Vector2 from, Vector2 to)
        {
			transform.SetPositionAndRotation(from, Quaternion.identity);
			displacement = to - from;

            float peakHeight = Perks.Has(Perk.Organised) ? 0.5f : 1f;
            float sqGravity = Mathf.Sqrt(GRAVITY);
            float peak = peakHeight * Vector2.Distance(from, to) * 0.1f;
			float height = Mathf.Max(0f, displacement.y) + peak;
			float initialY = Mathf.Sqrt(2f * height);

            float discriminant = MathsHelper.SafeRoot(2f * (height - displacement.y));
            float factor = displacement.x / (initialY + discriminant);
            float initialX = sqGravity * factor;
            initialY *= sqGravity;

            used = false;
            lifetime = displacement.x / initialX;
            Body.AddTorque(MathsHelper.SafeRoot(displacement.x * 0.33f) * Random.Range(0.8f, 1.2f));
			Body.AddForce(new Vector2(initialX, initialY), ForceMode2D.Impulse);
        }

        public override void Collide(Collider2D other){}
        public override void Hit(IAttackable attackable)
        {
            if(immunity <= 0f && attackable is not PlayerAttackable) Splash();
        }

        public void Splash()
        {
            if(used) return;
            used = true;

            AudioManager.Instance.Play(breakSound, transform.position, AudioChannel.Sound);

            PotionSplash splash = Weapon.Pool.Fetch();
            splash.Initialise(transform.position, Weapon, this);
            Weapon.Release(this);
        }
    }
}

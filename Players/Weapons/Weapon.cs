using System.Collections.Generic;
using System.Collections;
using System;
using Sigmoid.Projectiles;
using Sigmoid.Utilities;
using Sigmoid.Upgrading;
using Sigmoid.Reactions;
using Sigmoid.Chemicals;
using Sigmoid.Players;
using Sigmoid.Effects;
using Sigmoid.Mining;
using Sigmoid.Audio;
using Sigmoid.Game;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Weapons
{
	public class Weapon : ObjectPool<Potion>
	{
		[field: SerializeField] public ScriptableWeapon Type { get; private set; }
		[field: SerializeField] public SplashPool Pool { get; private set; }
        [SerializeField] private AudioPlayer throwSound;

        [SerializeField] private Key key;
		public bool IsPressed { get; private set; }

        private void Update()
        {
            cooldown -= Time.deltaTime;

            //this system prevents you from throwing a potion when trying to interact
            KeyCode keybind = Options.Keybinds[key];
            if(Input.GetKeyDown(keybind) && (!Player.Instance.Interactor.IsHovering ||
            (keybind != Options.Keybinds[Key.Interact] && keybind != KeyCode.Mouse1)))
                IsPressed = true;

            if(IsPressed && Input.GetKeyUp(keybind))
                IsPressed = false;
        }

        [SerializeField] private List<Chemical> magazine;
		public List<Chemical> Magazine => magazine;
        private Dictionary<Chemical, int> reloadCost;

        /// <summary>
        /// Gets called when the magazine is changed, which happens when a potion is modified in the bench
        /// </summary>
		public event Action<List<Chemical>> OnMagazineChanged;
		public void ChangeMagazine(List<Chemical> magazine)
		{
			this.magazine = magazine;
			OnMagazineChanged?.Invoke(magazine);
            UpdateRenderer();

            reloadCost = new Dictionary<Chemical, int>();
            foreach(Chemical chemical in magazine)
            {
                if(reloadCost.ContainsKey(chemical)) reloadCost[chemical]++;
                else reloadCost[chemical] = 1;
            }
		}

        /// <summary>
        /// Fetches the next chemical in the magazine, or returns None
        /// </summary>
        /// <returns></returns>
		private Chemical GetNext()
		{
			if(current >= magazine.Count) return Chemical.None;
			return magazine[current++];
		}



        private void Start()
        {
			ChangeMagazine(magazine);
            UpdateRenderer();
            SceneLoader.Instance.OnSceneLoaded += OnSceneChange;
        }
        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneLoaded -= OnSceneChange;
        }
        private void OnSceneChange(GameScene scene)
        {
            Reload();
            ChangeMagazine(magazine);
            UpdateRenderer();
        }

        /// <summary>
        /// Updates the colour of the held potion to that of the current element
        /// </summary>
        public void UpdateRenderer()
		{
            bool isEmpty = current >= magazine.Count || magazine.Count == 0;
			Chemical next = isEmpty ? Chemical.None : magazine[current];

			ScriptableChemical info = ChemicalManager.Get(next);
			Color[] colours = new Color[]
			{
				info.colours[1],
				info.colours[2],
				info.colours[3]
			};
			Swapper.UpdateLUT(Swapper.Originals, colours);
		}


		public Vector2 SpawnPosition => Sprite.transform.position;

		private float cooldown;
		public event Action<int> OnFire;

        /// <summary>
        /// Throws a potion towards the cursor
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="mousePos"></param>
        /// <returns></returns>
		public Potion Fire(Vector2 spawnPos, Vector2 mousePos)
		{
			if(cooldown > 0f) return null;
			cooldown = Type.FireRate * PlayerStats.ThrowRate;

			Chemical chemical = GetNext();
			if(chemical == Chemical.None)
			{
				OnFire?.Invoke(-1);
				Reload();
				return null;
			}

			OnFire?.Invoke(current);
            Inventory.Instance.UseStock(chemical, 1);
            throwSound.Play();

            if(!Perks.Has(Perk.Organised))
            {
                Vector2 inaccuracy = new Vector2(
                    0.5f * UnityEngine.Random.Range(-1f, 1f),
                    0.5f * UnityEngine.Random.Range(-1f, 1f)
                );
                mousePos += inaccuracy;
            }

			Potion potion = Fetch().Initialise(this, chemical, GetDamageMultiplier(chemical));
			potion.Launch(spawnPos, mousePos);

			if(current == magazine.Count) Reload();
			UpdateRenderer();
			return potion;
		}

        /// <summary>
        /// Adds 25% if this is the last potion and Rhythmic is unlocked<br/>
        /// Adds 10% for every matching element if Saturated is unlocked
        /// </summary>
        /// <param name="thrownChemical"></param>
        /// <returns></returns>
        private float GetDamageMultiplier(Chemical thrownChemical)
        {
            float multiplier = 1f;
            if(Perks.Has(Perk.Rhythmic) && current == magazine.Count) multiplier *= 1.25f;
            if(Perks.Has(Perk.Saturated))
            {
                int count = 0;
                foreach(Chemical chemical in WeaponManager.Instance.Weapons[0].Magazine)
                    if(chemical == thrownChemical) count++;
                foreach(Chemical chemical in WeaponManager.Instance.Weapons[1].Magazine)
                    if(chemical == thrownChemical) count++;

                multiplier *= 1f + 0.1f * count;
            }
            return multiplier;
        }



		private int current;
		private bool isReloading;
		public bool IsReloading => isReloading;

        /// <summary>
        /// Called when reloading is fully complete
        /// </summary>
		public event Action OnReload;
        /// <summary>
        /// Called every frame with the fraction by which the reloading is completed
        /// </summary>
		public event Action<float> OnTimer;

        /// <summary>
        /// Reloads the potion if not already reloading, constantly calling OnTimer to update the UI
        /// </summary>
        public void Reload()
        {
            if(isReloading) return;
            StartCoroutine(CReload());

            IEnumerator CReload()
            {
                isReloading = true;

                float timeElapsed = 0f;
                float reloadSpeed = Type.ReloadSpeed * PlayerStats.ReloadSpeed;
                while(timeElapsed < reloadSpeed)
                {
                    float fraction = timeElapsed / reloadSpeed;
                    OnTimer?.Invoke(fraction);

                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                OnTimer?.Invoke(1f);

                isReloading = false;
                current = 0;
                UpdateRenderer();
                OnReload?.Invoke();
            }
        }



        /// <summary>
        /// Called when a potion thrown by this weapon causes a reaction with any enemy
        /// </summary>
        public event Action<ProjectileHit, Reaction> OnReact;
        public void React(ProjectileHit hit, Reaction reaction) => OnReact?.Invoke(hit, reaction);



		private bool isActive;
		public bool IsActive => isActive;
		public void Activate() => isActive = true;
		public void Deactivate() => isActive = false;

        /// <summary>
        /// Lerps the weapon's rotation to match the cursor
        /// </summary>
        /// <param name="mouseAngle"></param>
		public void UpdateTransform(float mouseAngle)
		{
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, mouseAngle);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * Type.Responsiveness);
			Sprite.flipY = mouseAngle > 90f || mouseAngle < -90f;
		}

		[field: SerializeField] private SpriteRenderer Sprite;
		[field: SerializeField] private PaletteSwapper Swapper;
    }
}

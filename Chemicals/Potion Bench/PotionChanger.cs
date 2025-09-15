using System.Collections.Generic;
using Sigmoid.Chemicals;
using Sigmoid.Weapons;
using Sigmoid.Players;
using Sigmoid.Effects;
using Sigmoid.Audio;
using Sigmoid.Game;
using UnityEngine.UI;
using UnityEngine;

namespace Sigmoid.UI
{
    /// <summary>
    /// Allows for the altering of one potion's magazine
    /// </summary>
	public class PotionChanger : MonoBehaviour
	{
        /// <summary>
        /// The index of the associated weapon (since it's in another scene)
        /// </summary>
        [SerializeField] private int weaponID;
		private Weapon linkedWeapon;

		[SerializeField] private PaletteSwapper swapper;
		[SerializeField] private Image bottle;
		[SerializeField] private Image liquid;
		[SerializeField] private Sprite[] fillLevels;
        [SerializeField] private AudioPlayer sloshSound;
        [SerializeField] private AudioPlayer clearSound;

        public event System.Action<List<Chemical>> OnModify;
		private List<Chemical> concoction;

		private void Start()
		{
            linkedWeapon = WeaponManager.Instance.Weapons[weaponID];
			concoction = linkedWeapon.Magazine;
			Recolour();

            SceneLoader.Instance.OnSceneLoaded += RefreshAtHome; //unsubscribed manually in OnDestroy
            OnModify?.Invoke(concoction);
		}

        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneLoaded -= RefreshAtHome;
        }

        private void RefreshAtHome(GameScene scene)
        {
            if(scene == GameScene.Home) OnModify?.Invoke(concoction);
        }

		public void Clear()
		{
            clearSound.Play();
			concoction = new List<Chemical>();
            OnModify?.Invoke(concoction);
			Recolour();
            Apply();
		}

        /// <summary>
        /// Attempts to add a chemical to the potion if it is not already full
        /// </summary>
        /// <param name="addition"></param>
		public void Add(Chemical addition)
		{
            if(concoction.Count >= PlayerStats.PotionSize) return;
            sloshSound.Play();
			concoction.Add(addition);
            OnModify?.Invoke(concoction);
			Recolour();
            Apply();
		}

        /// <summary>
        /// Recolours the potion to the average of the constituent chemicals in its composition
        /// </summary>
		private void Recolour()
		{
			if(UpdateSprite()) return;

			Color[] colours = new Color[5];
			foreach(Chemical chemical in concoction)
			{
				ScriptableChemical info = ChemicalManager.Get(chemical);
				for(int i = 0; i < 5; i++)
				{
					colours[i] += info.colours[i + 1] / concoction.Count;
				}
			}

			swapper.UpdateLUT(swapper.Originals, colours);
		}

        /// <summary>
        /// Updates the sprite used by the liquid according to the fill level
        /// </summary>
        /// <returns></returns>
		private bool UpdateSprite()
		{
            int level = Mathf.Min(concoction.Count, 5);
			liquid.sprite = fillLevels[level];
			return concoction.Count == 0;
		}

        /// <summary>
        /// Actually updates the weapon, as opposed to just the UI
        /// </summary>
        public void Apply() => linkedWeapon.ChangeMagazine(concoction);



        private bool isHovering;
		public bool IsHovering => isHovering;
        public void Hover() => isHovering = true;
        public void Unhover() => isHovering = false;

        private static readonly Color HoverColor = new Color(0.900f, 0.900f, 0.900f, 1.000f);
        private void Update() => bottle.color = Color.Lerp(bottle.color, IsHovering ? HoverColor : Color.white, 16f * Time.unscaledDeltaTime);
    }
}

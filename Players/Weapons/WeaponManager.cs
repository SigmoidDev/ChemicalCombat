using System.Collections.Generic;
using Sigmoid.Projectiles;
using Sigmoid.Utilities;
using Sigmoid.Reactions;
using Sigmoid.Chemicals;
using Sigmoid.Players;
using Sigmoid.Cameras;
using Sigmoid.UI;
using UnityEngine;

namespace Sigmoid.Weapons
{
    /// <summary>
    /// Holds references to all of the player's weapons<br/>
    /// (I originally planned to have more than just two, so this is quite overengineered)
    /// </summary>
	public class WeaponManager : Singleton<WeaponManager>
	{
		[field: SerializeField] public List<Weapon> Weapons { get; private set; }
		[field: SerializeField] public LayerMask GroundMask { get; private set; }
		[field: SerializeField] public LayerMask HitboxMask { get; private set; }

        public event System.Action<ProjectileHit, Reaction> OnReact;
        private void Awake()
        {
            timesUsed = new Dictionary<Chemical, int>();
            foreach(Weapon weapon in Weapons)
                weapon.OnReact += (hit, reaction) => OnReact?.Invoke(hit, reaction); //unsubscribed when this is unloaded
        }

		private void Update()
		{
			if(PlayerUI.InMenu || !Player.Instance.IsAlive) return;

			float mouseAngle = MathsHelper.GetMouseAngle();
			foreach(Weapon weapon in Weapons)
			{
				weapon.UpdateTransform(mouseAngle);
                if(weapon.IsReloading) continue;

				if(weapon.IsPressed && weapon.Magazine.Count > 0)
				{
					Vector2 mousePosition = MainCamera.MousePosition;
					Potion thrownPotion = weapon.Fire(weapon.SpawnPosition, mousePosition);
                    if(thrownPotion != null) UseChemical(thrownPotion.Chemical);
				}

                if(Input.GetKeyDown(Options.Keybinds[Key.Reload]))
                    weapon.Reload();
			}
		}

        public int TotalDamage { get; private set; }
        public int HighestDamage { get; private set; }

        /// <summary>
        /// Records the final damage dealt to any enemy for achievement and tracking purposes
        /// </summary>
        /// <param name="amount"></param>
        public void RecordDamage(int amount)
        {
            TotalDamage += amount;
            if(amount > HighestDamage)
                HighestDamage = amount;
        }

        private Dictionary<Chemical, int> timesUsed;
        private void UseChemical(Chemical chemical)
        {
            if(timesUsed.ContainsKey(chemical)) timesUsed[chemical]++;
            else timesUsed[chemical] = 1;
        }

        /// <summary>
        /// Finds how many times a given element was used in a potion
        /// </summary>
        /// <param name="chemical"></param>
        /// <returns></returns>
        public int GetTimesUsed(Chemical chemical) => timesUsed.TryGetValue(chemical, out int amount) ? amount : 0;

        /// <summary>
        /// Runs a find maximum on the timesUsed dictionary, returning None if a potion has never been thrown
        /// </summary>
        /// <returns></returns>
        public Chemical GetMostUsedChemical()
        {
            Chemical mostUsed = Chemical.None;
            int mostTimes = 0;

            foreach(KeyValuePair<Chemical, int> usagePair in timesUsed)
            {
                if(usagePair.Value > mostTimes)
                {
                    mostUsed = usagePair.Key;
                    mostTimes = usagePair.Value;
                }
            }

            return mostUsed;
        }
    }
}

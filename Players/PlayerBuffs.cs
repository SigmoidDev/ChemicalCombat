using System.Collections.Generic;
using System.Linq;
using Sigmoid.Projectiles;
using Sigmoid.Utilities;
using Sigmoid.Reactions;
using Sigmoid.Weapons;
using Sigmoid.Enemies;
using Sigmoid.Effects;
using Sigmoid.Game;

namespace Sigmoid.Players
{
    /// <summary>
    /// Handles temporary buffs that have custom effects (not directly affecting a stat)<br/>
    /// Note that this class, despite being called PlayerBuffs, also handles debuffs
    /// </summary>
	public class PlayerBuffs : Singleton<PlayerBuffs>
	{
        #region Midas
        private bool hasMidas = false;
        public static bool HasMidas => Instance.hasMidas;

		public System.Guid GrantMidasBuff()
        {
            hasMidas = true;
            return System.Guid.Empty;
        }
        public void RemoveMidasBuff() => hasMidas = false;
        #endregion

        #region Terrified
        private int terrifiedStacks;
        public int TerrifiedStacks => terrifiedStacks;
        private Dictionary<PhantomAttacker, int> terrifiedDictionary;
        public void UpdateTerrifiedStacks(PhantomAttacker phantom, int number)
        {
            terrifiedDictionary[phantom] = number;

            terrifiedStacks = 0;
            foreach(KeyValuePair<PhantomAttacker, int> stacks in terrifiedDictionary.ToList())
                if(stacks.Key != null && stacks.Key.IsEnabled)
                    terrifiedStacks += stacks.Value;

            TerrifiedVignette.Instance.UpdateAlphas(terrifiedStacks);
        }

        public void RemoveTerrifiedStacks(PhantomAttacker phantom)
        {
            if(!terrifiedDictionary.ContainsKey(phantom)) return;

            terrifiedStacks -= terrifiedDictionary[phantom];
            terrifiedDictionary.Remove(phantom);
            TerrifiedVignette.Instance.UpdateAlphas(terrifiedStacks);
        }
        #endregion

        private void Awake()
        {
            terrifiedDictionary = new Dictionary<PhantomAttacker, int>();
            WeaponManager.Instance.OnReact += OnReact; //unsubscribed automatically on unload of Player scene
            SceneLoader.Instance.OnSceneLoaded += RefreshStacks; //unsubscribed manually below
        }

        private void OnDestroy()
        {
            if(!SceneLoader.InstanceExists) return;
            SceneLoader.Instance.OnSceneLoaded -= RefreshStacks;
        }

        /// <summary>
        /// Ensures that Terrified stacks are not kept across floors in case of bugs
        /// </summary>
        /// <param name="scene"></param>
        private void RefreshStacks(GameScene scene)
        {
            terrifiedDictionary = new Dictionary<PhantomAttacker, int>();
            TerrifiedVignette.Instance.UpdateAlphas(0);
        }

        private void OnReact(ProjectileHit hit, Reaction reaction)
        {
            if(HasMidas && SceneLoader.Instance.CurrentScene == GameScene.Labyrinth)
                CollectableSpawner.Instance.DropCoins(hit.point, 1);
        }
	}
}

using System.Collections.Generic;
using Sigmoid.Projectiles;
using Sigmoid.Utilities;
using Sigmoid.Weapons;
using Sigmoid.Game;

namespace Sigmoid.Reactions
{
	public class ReactionTracker : Singleton<ReactionTracker>
	{
		private Dictionary<Reaction, int> reactionsUsed;

        private void Awake()
        {
            reactionsUsed = new Dictionary<Reaction, int>();
            SceneLoader.Instance.OnSceneLoaded += AddWeaponEvent; //immediately unsubscribed on call
        }

        private void AddWeaponEvent(GameScene scene)
        {
            WeaponManager.Instance.OnReact += OnReact; //unsubscribed when Player scene is unloaded
            SceneLoader.Instance.OnSceneLoaded -= AddWeaponEvent;
        }

        private void OnReact(ProjectileHit hit, Reaction reaction)
        {
            if(reactionsUsed.ContainsKey(reaction)) reactionsUsed[reaction]++;
            reactionsUsed[reaction] = 1;
        }

        public static int GetTimesUsed(Reaction reaction) => Instance.reactionsUsed.TryGetValue(reaction, out int amount) ? amount : 0;
        public static int UniqueReactions => Instance.reactionsUsed.Count;
	}
}

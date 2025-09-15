using System.Collections.Generic;
using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Chemicals;
using Sigmoid.Players;
using Sigmoid.Cameras;
using Sigmoid.Game;
using UnityEngine;
using Sigmoid.Weapons;

namespace Sigmoid.Tutorial
{
	public class TutorialManager : Singleton<TutorialManager>
	{
        private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
        private static WaitForSeconds _waitForSeconds0_3 = new WaitForSeconds(0.3f);

        [SerializeField] private PlayerTeleporter teleporter;
        [SerializeField] private List<TutorialCheckpoint> checkpoints;
        [SerializeField] private EnemyZone zone1;
        [SerializeField] private EnemyZone zone2;
        [SerializeField] private CoinZone zone3;

		private void Awake()
        {
            ChemicalManager.Lock(Chemical.Helium);
            ChemicalManager.Lock(Chemical.Carbon);
            ChemicalManager.Lock(Chemical.Nitrogen);
            ChemicalManager.Lock(Chemical.Chlorine);

            teleporter.OnTeleport += OnTeleport;
            Player.Instance.OnDeath += OnDeath;
        }

        /// <summary>
        /// Called when the player teleports from the A section to the B section of the tutorial
        /// </summary>
        private void OnTeleport()
        {
            ChemicalManager.Unlock(Chemical.Helium);
            ChemicalManager.Unlock(Chemical.Carbon);
            ChemicalManager.Unlock(Chemical.Nitrogen);
            ChemicalManager.Unlock(Chemical.Chlorine);

            //Fully heals the player and brings them to exactly 80 coins to buy one upgrade
            Player.Instance.Heal(10);
            CoinManager.Earn(80 - CoinManager.Instance.Coins);
        }

        /// <summary>
        /// Ensures that the player doesn't actually die, but is instead teleported to a safe location
        /// </summary>
        private void OnDeath() => StartCoroutine(RevivePlayer());
        private IEnumerator RevivePlayer()
        {
            yield return _waitForSeconds0_5;
            MainCamera.FadeOut(0.3f);
            yield return _waitForSeconds0_3;

            Vector2 respawnPosition = Vector2.zero;
            for(int i = checkpoints.Count - 1; i >= 0; i--)
            {
                if(checkpoints[i].IsReached)
                {
                    respawnPosition = checkpoints[i].RespawnPosition;
                    break;
                }
            }

            zone1.ResetRoom();
            zone2.ResetRoom();
            zone3.ResetRoom();

            Player.Instance.transform.position = respawnPosition;
            Player.Instance.Revive();
            Player.Instance.Heal(1);

            WeaponManager.Instance.Weapons[0].Reload();
            WeaponManager.Instance.Weapons[1].Reload();

            MainCamera.ResetScreenColour();
            MainCamera.CircleReveal(0.6f, 0.2f);
        }
	}
}

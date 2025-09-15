using UnityEngine;
using Steamworks;

namespace Sigmoid.Utilities
{
	public class SteamManager : Singleton<SteamManager>
	{
        private bool isConnected;
        #if UNITY_STANDALONE
        private void Start() => isConnected = SteamAPI.Init();
        #endif

        public string Username => isConnected ? SteamFriends.GetPersonaName() : "User" + Random.Range(1000, 10000).ToString();
    }
}

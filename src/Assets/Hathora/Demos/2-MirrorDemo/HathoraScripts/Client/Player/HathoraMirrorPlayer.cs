// Created by dylan@hathora.dev

using Hathora.Demos.Shared.Scripts.Client.Player;
using Mirror;
using UnityEngine;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Client.Player
{
    /// <summary>
    /// Helpers for the runtime-spawned networked Player GameObject.
    /// This example uses FishNet.
    /// </summary>
    public class HathoraMirrorPlayer : NetworkBehaviour
    {
        [SerializeField]
        private HathoraNetPlayerUI playerUi;
        
        [SerializeField]
        private GameObject ownerObjWrapper;
        
        #region Init
        /// <summary>
        /// Better to use this instead of Start, in most situations.
        /// <summary>Called AFTER OnStartNetwork</summary>
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            
            Debug.Log($"[HathoraMirrorPlayer] OnStartClient: IsOwner? {base.isOwned}");
            owningClientStarted();
        }

        private void owningClientStarted()
        {
            ownerObjWrapper.gameObject.SetActive(true);
            playerUi.OnConnected(
                base.netId.ToString(),
                NetworkManager.singleton.numPlayers); // Includes self
            
            NetworkSpawnLogs();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            Debug.Log("[HathoraMirrorPlayer] OnStopClient");
        }

        /// <summary>Called only once, AFTER OnStopClient</summary>
        public override void OnStopLocalPlayer()
        {
            base.OnStopLocalPlayer();
            Debug.Log("[HathoraMirrorPlayer] OnStopNetwork");
        }

        private void NetworkSpawnLogs()
        {
            Debug.Log($"[HathoraMirrorPlayer] OnNetworkSpawn; " +
                $"id: {base.netId}, " +
                $"name: `{gameObject.name}`");
            
            if (base.isClient && base.isServer)
                Debug.Log("[HathoraMirrorPlayer] OnNetworkSpawn called on host (server+client)");
            else if (base.isServerOnly)
                Debug.Log("[HathoraMirrorPlayer] OnNetworkSpawn called on server");
            else if (base.isClient)
                Debug.Log("[HathoraMirrorPlayer] OnNetworkSpawn called on client");
        }
        #endregion // Init
        

    }
}

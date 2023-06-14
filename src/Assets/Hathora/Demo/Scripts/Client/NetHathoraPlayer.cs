// Created by dylan@hathora.dev

using FishNet;
using FishNet.Object;
using LiteNetLib;
using UnityEngine;

namespace Hathora.Demo.Scripts.Client
{
    /// <summary>
    /// Helpers for the runtime-spawned networked Player GameObject.
    /// This example uses FishNet.
    /// </summary>
    public class NetHathoraPlayer : NetworkBehaviour
    {
        [SerializeField]
        private GameObject ownerObjWrapper;
        
        #region Init
        /// <summary>Called BEFORE OnStartClient</summary>
        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            Debug.Log($"[NetHathoraPlayer] OnStartNetwork");
        }

        /// <summary>
        /// Better to use this instead of Start, in most situations.
        /// <summary>Called AFTER OnStartNetwork</summary>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
            
            Debug.Log($"[NetHathoraPlayer] OnStartClient: IsOwner? {base.IsOwner}");
            if (!base.IsOwner)
                return;
            
            ownerObjWrapper.gameObject.SetActive(true);
            NetworkSpawnLogs();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            Debug.Log("[NetHathoraPlayer] OnStopClient");
        }
        
        /// <summary>Called only once, AFTER OnStopClient</summary>
        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            Debug.Log("[NetHathoraPlayer] OnStopNetwork");
        }

        private void NetworkSpawnLogs()
        {
            Debug.Log($"[NetHathoraPlayer] OnNetworkSpawn, id==={NetworkObject.ObjectId}");
            
            if (base.IsHost)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on host (server+client)");
            else if (base.IsServerOnly)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on server");
            else if (base.IsClient)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on client");
        }
        #endregion // Init
        

    }
}

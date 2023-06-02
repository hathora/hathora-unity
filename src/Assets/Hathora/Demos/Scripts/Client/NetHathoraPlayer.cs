// Created by dylan@hathora.dev

using UnityEngine;

namespace Hathora.Demos.Scripts.Client
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
        /// <summary>
        /// Better to use this instead of Start, in most situations.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!base.IsOwner)
                return;
            
            ownerObjWrapper.gameObject.SetActive(true);
            NetworkSpawnLogs();
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

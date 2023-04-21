// Created by dylan@hathora.dev

using Hathora.Net.Server;
using Unity.Netcode;
using UnityEngine;

namespace Hathora.Utils
{
    /// <summary>
    /// Helpers for the NetworkPlayer. Since NetworkPlayer spawns dynamically
    /// </summary>
    public class HathoraPlayer : NetworkBehaviour
    {
        private void Start()
        {
            NetworkSpawnLogs();
        }
        
        private void NetworkSpawnLogs()
        {
            Debug.Log($"[RpcTest] OnNetworkSpawn, id==={NetworkObjectId}");
            
            if (IsServer && IsClient)
                Debug.Log("OnNetworkSpawn called on host (server+client)");
            else if (IsServer)
                Debug.Log("OnNetworkSpawn called on server");
            else if (IsClient)
                Debug.Log("OnNetworkSpawn called on client");
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // TODO?
        }
    }
}

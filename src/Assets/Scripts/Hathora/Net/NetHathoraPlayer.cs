// Created by dylan@hathora.dev

using FishNet.Object;
using UnityEngine;

namespace Hathora.Net
{
    /// <summary>
    /// Helpers for the NetworkPlayer. Since NetworkPlayer spawns dynamically
    /// </summary>
    public class NetHathoraPlayer : NetworkBehaviour
    {
        private void Start() =>
            NetworkSpawnLogs();

        private void NetworkSpawnLogs()
        {
            Debug.Log($"[HathoraPlayer] OnNetworkSpawn, id==={NetworkObject.ObjectId}");
            
            if (base.IsHost)
                Debug.Log("[HathoraPlayer] OnNetworkSpawn called on host (server+client)");
            else if (base.IsServer)
                Debug.Log("[HathoraPlayer] OnNetworkSpawn called on server");
            else if (base.IsClient)
                Debug.Log("[HathoraPlayer] OnNetworkSpawn called on client");
        }
    }
}

// Created by dylan@hathora.dev

using Hathora.Net.Common;
using Unity.Netcode;
using UnityEngine;

namespace Hathora.Net.Server
{
    /// <summary>
    /// Server calls Client via [ClientRpc].
    /// Supports singleton via Singleton.
    /// </summary>
    public class NetServerMgr : NetMgrBase
    {
        public static NetServerMgr Singleton;

        private void Start() => setSingleton();
        
        private void setSingleton()
        {
            if (Singleton != null)
                Destroy(gameObject);

            Singleton = this;
        }

        public void HostAsServer()
        {
            Debug.Log("[NetServerMgr] @ HostAsServer - Starting...");
            s_NetMgr.StartServer();
            NetUi.ToggleLobbyUi(show:false, NetCommonMgr.NetMode.Server);
        }

        /// <summary>
        /// Send a ping to the server => then pong back from server to client.
        /// - Generally called to trigger NetServerMgr.TestServerToClientRpc().
        /// </summary>
        /// <param name="numTimesRpcd"></param>
        /// <param name="srcNetObjId"></param>
        // [ServerRpc(RequireOwnership = true)]
        [ServerRpc]
        public void TestServerRpc(int numTimesRpcd, ulong srcNetObjId)
        {
            Debug.Log("[NetClientMgr] TestServerPing: " +
                $"Server Received RPC #{numTimesRpcd} on NetObj #{srcNetObjId}");
            
            s_ClientMgr.TestClientRpc(numTimesRpcd, srcNetObjId);
        }
        
        [ServerRpc]
        public void RequestOwnershipServerRpc(ulong networkObjectId, ServerRpcParams rpcParams = default)
        {
            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects
                .TryGetValue(networkObjectId, out NetworkObject networkObject))
            {
                return;
            }
        
            // TODO: Validation middleware
            networkObject.ChangeOwnership(rpcParams.Receive.SenderClientId);
        }
    }
}

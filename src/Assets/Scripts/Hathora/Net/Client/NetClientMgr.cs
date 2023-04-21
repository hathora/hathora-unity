// Created by dylan@hathora.dev

using Hathora.Net.Common;
using Unity.Netcode;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// Client calls server via [ServerRpc]
    /// </summary>
    public class NetClientMgr : NetMgrBase
    {
        public static NetClientMgr Singleton;

        private void Start() => setSingleton();
        
        private void setSingleton()
        {
            if (Singleton != null)
                Destroy(gameObject);

            Singleton = this;
        }
        
        public void JoinAsClient()
        {
            Debug.Log("[NetServerMgr] @ HostAsServer - Finding Server...");
            s_NetMgr.StartClient();
            NetUi.ToggleLobbyUi(show:false, NetCommonMgr.NetMode.Client);
        }
        
        /// <summary>
        /// Send a ping from client to server => then pong back from server to client.
        /// - Generally called to trigger NetServerMgr.TestServerToClientRpc().
        /// </summary>
        /// <param name="numTimesRpcd"></param>
        /// <param name="srcNetObjId"></param>
        [ServerRpc(RequireOwnership = true)]
        public void TestClientToServerRpc(int numTimesRpcd, ulong srcNetObjId)
        {
            Debug.Log($"[PingRpcTest] TestClientToServerPing: " +
                $"Server Received RPC #{numTimesRpcd} on NetObj #{srcNetObjId}");
            
            s_ServerMgr.TestServerToClientRpc(numTimesRpcd, srcNetObjId);
        }
    }
}

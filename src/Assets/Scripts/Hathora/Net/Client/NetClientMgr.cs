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
        /// Send a pong back from server to client.
        /// - Generally called after NetClientMgr.TestServerPing().
        /// </summary>
        /// <param name="numTimesRpcd"></param>
        /// <param name="srcNetObjId"></param>
        [ClientRpc]
        public void TestClientRpc(int numTimesRpcd, ulong srcNetObjId)
        {
            Debug.Log("[NetServerMgr] TestClientRpc: " +
                $"Client Received RPC #{numTimesRpcd} on NetObj #{srcNetObjId}");
        }
    }
}

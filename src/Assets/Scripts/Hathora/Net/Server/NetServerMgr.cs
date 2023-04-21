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
        /// Send a pong back from server to client.
        /// - Generally called after NetClientMgr.TestClientToServerPing().
        /// </summary>
        /// <param name="numTimesRpcd"></param>
        /// <param name="srcNetObjId"></param>
        [ClientRpc]
        public void TestServerToClientRpc(int numTimesRpcd, ulong srcNetObjId)
        {
            Debug.Log("[PingRpcTest] TestServerToClientPong: " +
                $"Client Received RPC #{numTimesRpcd} on NetObj #{srcNetObjId}");
        }
    }
}

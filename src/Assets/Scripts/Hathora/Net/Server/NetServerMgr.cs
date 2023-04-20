// Created by dylan@hathora.dev

using Hathora.Net.Common;
using Unity.Netcode;
using UnityEngine;

namespace Hathora.Net.Server
{
    /// <summary>
    /// Server calls Client via [ClientRpc].
    /// Supports singleton via s_NetServerMgr.
    /// </summary>
    public class NetServerMgr : NetMgrBase
    {
        public static NetServerMgr s_NetServerMgr;

        private void Start() => setSingleton();
        
        private void setSingleton()
        {
            if (s_NetServerMgr != null)
                Destroy(gameObject);

            s_NetServerMgr = this;
        }

        public void HostAsServer()
        {
            NetMgr.StartServer();
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

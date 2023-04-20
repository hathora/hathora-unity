// Created by dylan@hathora.dev

using Hathora.Net.Common;
using Hathora.Net.Server;
using Unity.Netcode;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// Client calls server via [ServerRpc]
    /// </summary>
    public class NetClientMgr : NetMgrBase
    {
        [SerializeField]
        private NetServerMgr serverMgr;

        public void JoinAsClient()
        {
            NetMgr.StartClient();
            NetUi.ToggleLobbyUi(show:false, NetCommonMgr.NetMode.Client);
        }
        
        /// <summary>
        /// Send a ping from client to server => then pong back from server to client.
        /// - Generally called to trigger NetServerMgr.TestServerToClientRpc().
        /// </summary>
        /// <param name="numTimesRpcd"></param>
        /// <param name="srcNetObjId"></param>
        [ServerRpc]
        public void TestClientToServerRpc(int numTimesRpcd, ulong srcNetObjId)
        {
            Debug.Log($"[PingRpcTest] TestClientToServerPing: " +
                $"Server Received RPC #{numTimesRpcd} on NetObj #{srcNetObjId}");
            
            serverMgr.TestServerToClientRpc(numTimesRpcd, srcNetObjId);
        }
    }
}

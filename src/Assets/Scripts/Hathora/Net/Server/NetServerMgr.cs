// Created by dylan@hathora.dev

using FishNet;
using FishNet.Object;
using Hathora.Net.Common;
using UnityEngine;

namespace Hathora.Net.Server
{
    /// <summary>
    /// Server calls Client via [ObserversRpc].
    /// Supports singleton via Singleton.
    /// </summary>
    public class NetServerMgr : NetBehaviourBase
    {
        public static NetServerMgr Singleton;

        private void Awake() => 
            setSingleton();
        
        private void setSingleton()
        {
            if (Singleton != null)
                Destroy(gameObject);

            Singleton = this;
        }

        public void HostAsServer()
        {
            Debug.Log("[NetServerMgr] @ HostAsServer - Starting...");
            InstanceFinder.ServerManager.StartConnection();
            s_netUi.ToggleLobbyUi(show:false, NetCommonMgr.NetMode.Server);
        }

        /// <summary>
        /// Send a msg to the server from an observer.
        /// </summary>
        /// <param name="msg">Arbitrary string</param>
        // [ServerRpc(RequireOwnership = true)]
        [ServerRpc]
        public void SendMsgServerRpc(string msg)
        {
            // Send a msg back
            Debug.Log($"[NetServerMgr] SendMsgServerRpc: Received msg on server (from observed client) == '{msg}'");
            s_ClientMgr.ReceiveMsgObserverRpc(msg);
        }
    }
}

// Created by dylan@hathora.dev

using FishNet;
using FishNet.Object;
using Hathora.Net.Common;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// Client calls server via [ServerRpc]
    /// </summary>
    public class NetClientMgr : NetBehaviourBase
    {
        public static NetClientMgr Singleton;

        private void Awake() => 
            setSingleton();
        
        private void setSingleton()
        {
            if (Singleton != null)
                Destroy(gameObject);

            Singleton = this;
        }
        
        public void JoinAsClient()
        {
            Debug.Log("[NetServerMgr] @ HostAsServer - Finding Server...");
            
            InstanceFinder.ClientManager.StartConnection();
            s_netUi.ToggleLobbyUi(show:false, NetCommonMgr.NetMode.Client);
        }
        
        /// <summary>
        /// Receive a msg from the server.
        /// </summary>
        /// <param name="msg">Arbitrary string</param>
        [ObserversRpc]
        public void ReceiveMsgObserverRpc(string msg)
        {
            Debug.Log($"[NetClientMgr] SendMsgServerRpc: Received msg on observed client (from server) == '{msg}'");
        }
    }
}

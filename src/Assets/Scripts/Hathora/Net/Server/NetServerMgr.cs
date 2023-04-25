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
        }


    }
}

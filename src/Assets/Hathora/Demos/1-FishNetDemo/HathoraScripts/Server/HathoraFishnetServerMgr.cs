// Created by dylan@hathora.dev

using FishNet;
using FishNet.Transporting;
using Hathora.Core.Scripts.Runtime.Server;
using UnityEngine;

namespace Hathora.Demos._1_FishNetDemo.HathoraScripts.Server
{
    /// <summary>
    /// Child of HathoraServerMgrBase to handle FishNet-specific runtime server logic.
    /// </summary>
    public class HathoraFishnetServerMgr : HathoraServerMgrBase
    {
        public static HathoraServerMgrBase Singleton { get; private set; }

        /// <summary>Shortcuts to the selected Transport instance</summary>
        private static Transport transport
        {
            get => InstanceFinder.TransportManager.Transport;
            set => InstanceFinder.TransportManager.Transport = value;
        }

        #region Init
        protected override void OnAwake()
        {
            Debug.Log("[HathoraFishnetServerMgr] OnAwake");
            base.OnAwake();
            setSingleton();
        }
    
        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraServerMgrBase.setSingleton] Error: " +
                    "setSingleton: Destroying dupe");
            
                Destroy(gameObject);
                return;
            }
    
            Singleton = this;
        }
        #endregion // Init

        
        #region FishNet-Specific Server Handling
        // TODO
        #endregion // FishNet-Specific Server Handling
    }
}

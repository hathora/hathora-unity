// Created by dylan@hathora.dev

using FishNet;
using FishNet.Transporting;
using Hathora.Core.Scripts.Runtime.Server;
using UnityEngine;

namespace Hathora.Demos._1_FishNetDemo.HathoraScripts.Server
{
    /// <summary>
    /// Child of HathoraServerMgrBase to handle FishNet-specific runtime server logic.
    /// On virtual base.OnAwake, if we're deloyed to a Hathora server, well get the Process info.
    /// </summary>
    public class HathoraFishnetServerMgr : HathoraServerMgrBase
    {
        public static HathoraServerMgrBase Singleton { get; private set; }

        /// <summary>Shortcuts to the selected Transport instance</summary>
        private static Transport transport =>
            InstanceFinder.TransportManager.Transport;

        #region Init
        protected override void SetSingleton()
        {
            base.SetSingleton();

            if (Singleton != null)
            {
                Debug.LogError("[HathoraFishnetServerMgr.setSingleton] Error: " +
                    "setSingleton: Destroying dupe");
            
                Destroy(gameObject);
                return;
            }
    
            Singleton = this;
        }
        #endregion // Init
    }
}

// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Client;
using Hathora.Core.Scripts.Runtime.Server.Models;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Server
{
    /// <summary>
    /// Inits and centralizes all Hathora Server [runtime] API wrappers.
    /// 
    /// Unlike HathoraClientMgrBase, we don't need a parent since Server is lower-level
    /// than Client (eg: No UI, Session or net code specific to a platform).
    /// TODO: If this gets more complex, make it an abstract base class; parity with Client.
    /// </summary>
    public class HathoraServerMgr : MonoBehaviour
    {
        #region Serialized Fields
        [Header("(!) Top menu: Hathora/ServerConfigFinder")]
        [SerializeField]
        private HathoraServerConfig hathoraServerConfig;
        
        [Header("API Wrappers for Hathora SDK")]
        [SerializeField]
        private ServerApiContainer serverApis;
        #endregion // Serialized Fields
        
        public static HathoraServerMgr Singleton { get; private set; }
        
        
        #region Init
        private void Awake() => setSingleton();

        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraServerMgrsetSingleton.] Error: " +
                    "setSingleton: Destroying dupe");
                
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }

        private void Start()
        {
            validateReqs();
            initApis(_hathoraSdkConfig: null); // Base will create this
        }

        private void validateReqs()
        {
            if (hathoraServerConfig == null)
            {
                Debug.LogError("[HathoraServerMgr] !HathoraServerConfig; " +
                    $"Serialize to {gameObject.name}.{nameof(HathoraServerMgr)}");
            }
        }
        
        /// <summary>
        /// Init all Server [runtime] API wrappers. Uses serialized HathoraServerConfig.
        /// </summary>
        /// <param name="_hathoraSdkConfig">We'll automatically create this, if empty</param>
        private void initApis(Configuration _hathoraSdkConfig = null)
        {
            if (serverApis.ServerProcessApi != null)
                serverApis.ServerProcessApi.Init(hathoraServerConfig, _hathoraSdkConfig);
        }
        #endregion // Init
    }
}

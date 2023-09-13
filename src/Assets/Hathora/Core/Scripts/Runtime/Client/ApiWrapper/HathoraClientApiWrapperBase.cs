// Created by dylan@hathora.dev

using Hathora.Core.Scripts.Runtime.Client.Config;
using Hathora.Core.Scripts.Runtime.Common.Models;
using HathoraSdk;
using Newtonsoft.Json;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Client.ApiWrapper
{
    /// <summary>
    /// High-level base API wrapper for the low-level Hathora SDK APIs.
    /// - Only Client API wrappers may inherit from this.
    /// * Caches SDK Config and HathoraClientConfig for API use. 
    /// * Try/catches async API calls and [Base] automatically handlles API Exceptions.
    /// * Due to code autogen, the SDK exposes too much: This simplifies and minimally exposes.
    /// * Due to code autogen, the SDK sometimes have nuances: This provides fixes/workarounds.
    /// * Call Init() to pass HathoraClientConfig + Hathora SDK Config (see HathoraClientMgr).
    /// * Does not handle UI (see HathoraClientMgrUi).
    /// * Does not handle Session caching (see HathoraClientSession).
    /// </summary>
    public abstract class HathoraClientApiWrapperBase : MonoBehaviour, IHathoraApiBase
    {
        public SDKConfig HathoraSdkConfig { get; set; }
        protected HathoraClientConfig HathoraClientConfig { get; private set; }
        
        // Shortcuts
        public string AppId => HathoraClientConfig.AppId;


        #region Init
        /// <summary>
        /// Init anytime before calling an API. Client calls use V1 auth token.
        /// </summary>
        /// <param name="_hathoraClientConfig">
        /// Find via Unity editor (alongside the server config) via top menu: Hathora/Configuration
        /// </param>
        /// <param name="_hathoraSdkConfig">SDKConfig that we pass to Hathora API calls</param>
        public virtual void Init(
            HathoraClientConfig _hathoraClientConfig,
            SDKConfig _hathoraSdkConfig = null)
        {
            this.HathoraClientConfig = _hathoraClientConfig;
            this.HathoraSdkConfig = _hathoraSdkConfig ?? GenerateSdkConfig();
        }
        
        public SDKConfig GenerateSdkConfig() => new();
        #endregion // Init
        
        
        #region Utils
        public string ToJson<T>(T Obj, bool prettify = true)
        {
            Formatting formatting = prettify ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(Obj, formatting);
        }
        #endregion // Utils
        
    }
}

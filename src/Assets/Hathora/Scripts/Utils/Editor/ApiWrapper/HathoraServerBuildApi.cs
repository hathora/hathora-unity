// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Scripts.Net.Server;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ApiWrapper
{
    public class HathoraServerBuildApi : HathoraServerApiBase
    {
        private BuildV1Api buildApi;
        
        public override void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig)
        {
            Debug.Log("[HathoraServerBuildApi] Initializing API...");
            base.Init(_hathoraSdkConfig, _hathoraServerConfig);
            this.buildApi = new BuildV1Api(_hathoraSdkConfig);
        }
        
        
        
        #region Server Build Async Hathora SDK Calls
        
        #endregion // Server Build Async Hathora SDK Calls
    }
}

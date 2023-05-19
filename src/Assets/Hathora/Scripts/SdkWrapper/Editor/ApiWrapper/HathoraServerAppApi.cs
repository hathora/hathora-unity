// Created by dylan@hathora.dev

using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Debug = UnityEngine.Debug;

namespace Hathora.Scripts.SdkWrapper.Editor.ApiWrapper
{
    public class HathoraServerAppApi : HathoraServerApiBase
    {
        private readonly AppV1Api appApi;

        
        /// <summary>
        /// </summary>
        /// <param name="_netHathoraConfig"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public HathoraServerAppApi(
            NetHathoraConfig _netHathoraConfig,
            Configuration _hathoraSdkConfig = null)
            : base(_netHathoraConfig, _hathoraSdkConfig)
        {
            Debug.Log("[HathoraServerAppApi] Initializing API...");
            this.appApi = new AppV1Api(base.HathoraSdkConfig);
        }
        
        
        #region Server App Async Hathora SDK Calls
        /// <summary>
        /// Wrapper for `CreateAppAsync` to upload and app a cloud app to Hathora.
        /// </summary>
        /// <returns>Returns App on success</returns>
        public async Task<List<ApplicationWithDeployment>> GetAppsAsync()
        {
            List<ApplicationWithDeployment> getAppsResult; 
            try
            {  
                getAppsResult = await appApi.GetAppsAsync();
            }
            catch (ApiException apiErr)
            {
                HandleServerApiException(
                    nameof(HathoraServerAppApi),
                    nameof(GetAppsAsync), 
                    apiErr);
                return null;
            }

            Debug.Log($"[HathoraServerAppApi] num: '{getAppsResult?.Count}'");

            return getAppsResult;
        }
        #endregion // Server App Async Hathora SDK Calls
    }
}

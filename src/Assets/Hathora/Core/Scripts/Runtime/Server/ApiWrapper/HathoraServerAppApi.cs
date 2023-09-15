// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HathoraSdk;
using HathoraSdk.Models.Operations;
using HathoraSdk.Models.Shared;
using HathoraSdk.Utils;
using Debug = UnityEngine.Debug;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
    /// <summary>
    /// Handles Application API calls to Hathora Server.
    /// - Passes API key from HathoraServerConfig to SDK
    /// - Passes Auth0 (Dev Token) from hathoraServerConfig to SDK
    /// - API Docs | https://hathora.dev/api#tag/AppV1
    /// </summary>
    public class HathoraServerAppApi : HathoraServerApiWrapperBase
    {
        private readonly AppV1SDK appApi;

        
        /// <summary>
        /// </summary>
        /// <param name="_hathoraServerConfig"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public HathoraServerAppApi(
            HathoraServerConfig _hathoraServerConfig,
            SDKConfig _hathoraSdkConfig = null)
            : base(_hathoraServerConfig, _hathoraSdkConfig)
        { 
            Debug.Log("[HathoraServerAppApi] Initializing API..."); 
            
            // TODO: Overloading VxSDK constructor with nulls, for now, until we know how to properly construct
            SpeakeasyHttpClient httpClient = null;
            string serverUrl = null;
            this.appApi = new AppV1SDK(
                httpClient,
                httpClient, 
                serverUrl,
                HathoraSdkConfig);
        }
        
        
        #region Server App Async Hathora SDK Calls
        /// <summary>
        /// Wrapper for `CreateAppAsync` to upload and app a cloud app to Hathora.
        /// </summary>
        /// <param name="_cancelToken">TODO: This may be implemented in the future</param>
        /// <returns>Returns App on success</returns>
        public async Task<List<ApplicationWithDeployment>> GetAppsAsync(
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerAppApi)}.{nameof(GetAppsAsync)}]";

            // Get response async => 
            GetAppsResponse getAppsResponse = null;
            
            try
            {
                getAppsResponse = await appApi.GetAppsAsync(
                    new GetAppsSecurity { Auth0 = base.HathoraDevToken } // TODO: Redundant - already has Auth0 from constructor via SDKConfig.HathoraDevToken
                ); 
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(appApi.GetAppsAsync)} => Error: {e.Message}");
                return null; // fail
            }

            // Get inner response to return -> Log/Validate
            List<ApplicationWithDeployment> applicationWithDeployment = getAppsResponse.ApplicationWithDeployments;
            Debug.Log($"{logPrefix} num: '{applicationWithDeployment?.Count ?? 0}'");
            
            return applicationWithDeployment;
        }
        #endregion // Server App Async Hathora SDK Calls
    }
}

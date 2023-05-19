// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Models;
using Hathora.Scripts.Utils;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Editor.ApiWrapper
{
    public class HathoraServerAppApi : HathoraServerApiBase
    {
        private readonly AppV1Api appApi;

        
        public HathoraServerAppApi(
            Configuration _hathoraSdkConfig, 
            NetHathoraConfig _netHathoraConfig)
            : base(_hathoraSdkConfig, _netHathoraConfig)
        {
            Debug.Log("[HathoraServerAppApi] Initializing API...");
            this.appApi = new AppV1Api(_hathoraSdkConfig);
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
                    nameof(HathoraServerBuildApi),
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

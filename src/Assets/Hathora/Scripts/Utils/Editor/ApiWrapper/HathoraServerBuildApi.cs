// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
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
        /// <summary>
        /// Wrapper for `CreateBuildAsync` to upload and deploy a cloud build to Hathora.
        /// </summary>
        /// <returns>Returns AuthResult on success</returns>
        public async Task<Build> DeployBuildToHathora()
        {
            Build cloudBuildResult;
            try
            {
                cloudBuildResult = await buildApi.CreateBuildAsync(hathoraServerConfig.AppId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraServerBuildApi]**ERR @ DeployBuildToHathora " +
                    $"(CreateBuildAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null;
            }
            
            Debug.Log($"[HathoraServerBuildApi] " +
                $"Status: '{cloudBuildResult?.Status}', " +
                $"BuildId: '{cloudBuildResult?.BuildId}'");

            // if (!isAuthed)
            //     return null;

            return cloudBuildResult;
        }
        #endregion // Server Build Async Hathora SDK Calls
    }
}

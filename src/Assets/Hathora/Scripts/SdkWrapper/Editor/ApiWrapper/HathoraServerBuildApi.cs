// Created by dylan@hathora.dev

using System;
using System.IO;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Server;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Editor.ApiWrapper
{
    public class HathoraServerBuildApi : HathoraServerApiBase
    {
        private readonly BuildV1Api buildApi;
        
        public HathoraServerBuildApi(
            Configuration _hathoraSdkConfig, 
            NetHathoraConfig _netHathoraConfig) 
            : base(_hathoraSdkConfig, _netHathoraConfig)
        {
            Debug.Log("[HathoraServerBuildApi] Initializing API...");
            this.buildApi = new BuildV1Api(_hathoraSdkConfig);
        }
        
        
        
        #region Server Build Async Hathora SDK Calls
        /// <summary>
        /// Wrapper for `CreateBuildAsync` to request an cloud build (tarball upload).
        /// </summary>
        /// <returns>Returns Build on success >> Pass this info to RunCloudBuildAsync()</returns>
        public async Task<Build> CreateBuildAsync()
        {
            Build createCloudBuildResult;
            
            try
            {
                createCloudBuildResult = await buildApi.CreateBuildAsync(
                    NetHathoraConfig.AppId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraServerBuildApi.RunCloudBuildAsync]" +
                    $"**ERR (CreateBuildAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null;
            }

            Debug.Log($"[HathoraServerBuildApi.RunCloudBuildAsync] result == " +
                $"BuildId: '{createCloudBuildResult.BuildId}, " +
                $"Status: {createCloudBuildResult.Status}");

            return createCloudBuildResult;
        }
        
        /// <summary>
        /// Wrapper for `RunBuildAsync` to upload the tarball after calling 
        /// </summary>
        /// <param name="_buildId"></param>
        /// <param name="tarball"></param>
        /// <returns>Returns byte[] on success</returns>
        public async Task<byte[]> RunCloudBuildAsync(double _buildId, Stream tarball)
        {
            byte[] cloudRunBuildResultByteArr;
            
            try
            {
                cloudRunBuildResultByteArr = await buildApi.RunBuildAsync(
                    NetHathoraConfig.AppId,
                    _buildId,
                    tarball);
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraServerBuildApi.RunCloudBuildAsync]" +
                    $"**ERR (RunBuildAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null;
            }

            Debug.Log($"[HathoraServerBuildApi.RunCloudBuildAsync] result == " +
                $"isSuccess? '{cloudRunBuildResultByteArr is { Length: > 0 }}");

            return cloudRunBuildResultByteArr;
        }
        #endregion // Server Build Async Hathora SDK Calls
    }
}

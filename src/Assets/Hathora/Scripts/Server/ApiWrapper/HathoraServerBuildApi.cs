// Created by dylan@hathora.dev

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Scripts.Sdk.hathora_cloud_sdks.csharp.src.Hathora.Cloud.Sdk.Api;
using Hathora.Scripts.Sdk.hathora_cloud_sdks.csharp.src.Hathora.Cloud.Sdk.Client;
using Hathora.Scripts.Sdk.hathora_cloud_sdks.csharp.src.Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Scripts.Server.ApiWrapper
{
    public class HathoraServerBuildApi : HathoraServerApiBase
    {
        private readonly BuildV1Api buildApi;

        
        /// <summary>
        /// </summary>
        /// <param name="_netHathoraConfig"></param>
        /// <param name="_hathoraSdkConfig">
        /// Set in base as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public HathoraServerBuildApi(
            NetHathoraConfig _netHathoraConfig,
            Configuration _hathoraSdkConfig = null) 
            : base(_netHathoraConfig, _hathoraSdkConfig)
        {
            Debug.Log("[HathoraServerBuildApi] Initializing API...");
            this.buildApi = new BuildV1Api(base.HathoraSdkConfig);
        }
        
        
        
        #region Server Build Async Hathora SDK Calls
        /// <summary>
        /// Wrapper for `CreateBuildAsync` to request an cloud build (_tarball upload).
        /// </summary>
        /// <param name="_cancelToken"></param>
        /// <returns>Returns Build on success >> Pass this info to RunCloudBuildAsync()</returns>
        public async Task<Build> CreateBuildAsync(CancellationToken _cancelToken = default)
        {
            Build createCloudBuildResult;
            
            try
            {
                createCloudBuildResult = await buildApi.CreateBuildAsync(
                    NetHathoraConfig.HathoraCoreOpts.AppId,
                    _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleServerApiException(
                    nameof(HathoraServerBuildApi),
                    nameof(CreateBuildAsync), 
                    apiException);
                return null;
            }

            Debug.Log($"[HathoraServerBuildApi.RunCloudBuildAsync] result == " +
                $"BuildId: '{createCloudBuildResult.BuildId}, " +
                $"Status: {createCloudBuildResult.Status}");

            return createCloudBuildResult;
        }

        /// <summary>
        /// Wrapper for `RunBuildAsync` to upload the _tarball after calling 
        /// </summary>
        /// <param name="_buildId"></param>
        /// <param name="_tarball"></param>
        /// <param name="_cancelToken"></param>
        /// <returns>Returns byte[] on success</returns>
        public async Task<byte[]> RunCloudBuildAsync(
            double _buildId, 
            Stream _tarball,
            CancellationToken _cancelToken = default)
        {
            byte[] cloudRunBuildResultByteArr;
            
            try
            {
                cloudRunBuildResultByteArr = await buildApi.RunBuildAsync(
                    NetHathoraConfig.HathoraCoreOpts.AppId,
                    _buildId,
                    _tarball,
                    _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleServerApiException(
                    nameof(HathoraServerBuildApi),
                    nameof(RunCloudBuildAsync), 
                    apiException);
                return null;
            }

            Debug.Log($"[HathoraServerBuildApi.RunCloudBuildAsync] result == " +
                $"isSuccess? '{cloudRunBuildResultByteArr is { Length: > 0 }}");

            return cloudRunBuildResultByteArr;
        }
        #endregion // Server Build Async Hathora SDK Calls
    }
}

// Created by dylan@hathora.dev

using System;
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
    /// Operations to get data on active and stopped processes.
    /// API Docs | https://hathora.dev/api#tag/ProcessesV1 
    /// </summary>
    public class HathoraServerProcessApi : HathoraServerApiWrapperBase
    {
        private ProcessesV1SDK processesApi;

        /// <summary>
        /// </summary>
        /// <param name="_hathoraServerConfig"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public HathoraServerProcessApi(
            HathoraServerConfig _hathoraServerConfig,
            SDKConfig _hathoraSdkConfig = null)
            : base(_hathoraServerConfig, _hathoraSdkConfig)
        { 
            Debug.Log("[HathoraServerProcessApi] Initializing API..."); 
            
            // TODO: Overloading VxSDK constructor with nulls, for now, until we know how to properly construct
            SpeakeasyHttpClient httpClient = null;
            string serverUrl = null;
            this.processesApi = new ProcessesV1SDK(
                httpClient,
                httpClient, 
                serverUrl,
                HathoraSdkConfig);
        }
        
        
        #region Server Process Async Hathora SDK Calls
        /// <summary>
        /// `GetProcessInfo` wrapper: Get details for an existing process using appId and processId.
        /// API Doc | https://hathora.dev/api#tag/ProcessesV1/operation/GetProcessInfo 
        /// </summary>
        /// <param name="_processId">
        /// The process running the Room; find it in web console or GetRunningProcesses().
        /// </param>
        /// <param name="_returnNullOnStoppedProcess">If the Process stopped (no Rooms inside), just return null</param>
        /// <param name="_cancelToken"></param>
        /// <returns>Returns Process on success</returns>
        public async Task<Process> GetProcessInfoAsync(
            string _processId,
            bool _returnNullOnStoppedProcess = true,
            CancellationToken _cancelToken = default)
        {
            const string logPrefix = "[HathoraServerProcessApi.GetProcessInfoAsync]";
            Debug.Log($"{logPrefix} <color=yellow>processId: {_processId}</color>");
            
            GetProcessInfoResponse getProcessInfoResponse = null;
            
            try
            {
                GetProcessInfoSecurity security;
                GetProcessInfoRequest request;
                
                getProcessInfoResponse = await processesApi.GetProcessInfoAsync(
                    AppId,
                    _processId,
                    _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} LoginAnonymousAsync => Error: {e.Message}");
                return null; // fail
            }

            Debug.Log($"{logPrefix} Success: <color=yellow>" +
                $"{nameof(getProcessInfoResponse)}: {ToJson(getProcessInfoResponse)}</color>");

            Process process = getProcessInfoResponse.Process; 
            if (process?.StoppingAt != null)
            {
                Debug.LogError($"{logPrefix} Got Process info, but reported <color=orange>Stopped</color> " +
                    $"(returnNullOnStoppedProcess=={_returnNullOnStoppedProcess})");
                return null;
            }

            return process;
        }
        #endregion // Server Process Async Hathora SDK Calls
    }
}

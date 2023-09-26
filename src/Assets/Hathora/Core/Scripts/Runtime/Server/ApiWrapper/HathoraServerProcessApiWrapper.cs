// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using HathoraCloud;
using HathoraCloud.Models.Operations;
using HathoraCloud.Models.Shared;
using Debug = UnityEngine.Debug;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
    /// <summary>
    /// Operations to get data on active and stopped processes.
    /// Processes Concept | https://hathora.dev/docs/concepts/hathora-entities#process
    /// API Docs | https://hathora.dev/api#tag/ProcessesV1 
    /// </summary>
    public class HathoraServerProcessApiWrapper : HathoraServerApiWrapperBase
    {
        protected ProcessesV1SDK ProcessesApi { get; }

        public HathoraServerProcessApiWrapper(
            HathoraCloudSDK _hathoraSdk,
            HathoraServerConfig _hathoraServerConfig)
            : base(_hathoraSdk, _hathoraServerConfig)
        {
            Debug.Log($"[{nameof(HathoraServerProcessApiWrapper)}.Constructor] " +
                "Initializing Server API...");
            
            this.ProcessesApi = _hathoraSdk.ProcessesV1 as ProcessesV1SDK;
        }
        
        
        #region Server Process Async Hathora SDK Calls
        /// <summary>
        /// `GetProcessInfo` wrapper: Get details for an existing process using appId and processId.
        /// API Doc | https://hathora.dev/api#tag/ProcessesV1/operation/GetProcessInfo 
        /// </summary>
        /// <param name="_processId">
        /// The process running the Room; find it in web console or GetRunningProcesses().
        /// </param>
        /// <param name="_returnNullOnStoppedProcess">
        /// If the Process stopped (no Rooms inside), just return null.
        /// - This makes the validation process easier if you want all-or-nothing.
        /// </param>
        /// <param name="_cancelToken">TODO</param>
        /// <returns>Returns Process on success</returns>
        public async Task<Process> GetProcessInfoAsync(
            string _processId,
            bool _returnNullOnStoppedProcess = true,
            CancellationToken _cancelToken = default)
        {
            const string logPrefix = "[HathoraServerProcessApiWrapper.GetProcessInfoAsync]";
            Debug.Log($"{logPrefix} <color=yellow>processId: {_processId}</color>");
            
            // Process request
            GetProcessInfoRequest getProcessInfoRequest = new()
            {
                ProcessId = _processId,
            };
            
            // Get response async =>
            GetProcessInfoResponse getProcessInfoResponse = null;
            
            try
            {
                getProcessInfoResponse = await ProcessesApi.GetProcessInfoAsync(
                    getProcessInfoRequest);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(ProcessesApi.GetProcessInfoAsync)} => Error: {e.Message}");
                return null; // fail
            }

            // Process result
            Debug.Log($"{logPrefix} Success: <color=yellow>{nameof(getProcessInfoResponse.Process)}: {ToJson(getProcessInfoResponse.Process)}</color>");

            Process process = getProcessInfoResponse.Process;

            bool isStoppedProcess = process?.StoppingAt != null; 
            if (isStoppedProcess)
            {
                Debug.LogError($"{logPrefix} Got Process info, but reported <color=orange>Stopped</color> " +
                    $"(returnNullOnStoppedProcess=={_returnNullOnStoppedProcess})");

                if (_returnNullOnStoppedProcess)
                    return null;
            }

            getProcessInfoResponse.RawResponse?.Dispose(); // Prevent mem leaks
            return process;
        }
        #endregion // Server Process Async Hathora SDK Calls
    }
}

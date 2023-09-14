// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Client.Config;
using HathoraSdk;
using HathoraSdk.Utils;
using HathoraSdk.Models.Shared;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Client.ApiWrapper
{
    /// <summary>
    /// High-level API wrapper for the low-level Hathora SDK's Room API.
    /// * Caches SDK Config and HathoraClientConfig for API use. 
    /// * Try/catches async API calls and [Base] automatically handlles API Exceptions.
    /// * Due to code autogen, the SDK exposes too much: This simplifies and minimally exposes.
    /// * Due to code autogen, the SDK sometimes have nuances: This provides fixes/workarounds.
    /// * Call Init() to pass HathoraClientConfig + Hathora SDK Config (see HathoraClientMgr).
    /// * Does not handle UI (see HathoraClientMgrUi).
    /// * Does not handle Session caching (see HathoraClientSession).
    /// </summary>
    public class HathoraClientRoomApi : HathoraClientApiWrapperBase
    {
        private RoomV2SDK roomApi;

        /// <summary>
        /// </summary>
        /// <param name="_hathoraClientConfig"></param>
        /// <param name="_hathoraSdkConfig"></param>
        public override void Init(
            HathoraClientConfig _hathoraClientConfig, 
            SDKConfig _hathoraSdkConfig = null)
        {
            Debug.Log($"[{nameof(HathoraClientRoomApi)}] Initializing API...");
            
            base.Init(_hathoraClientConfig, _hathoraSdkConfig);
            
            // TODO: Overloading VxSDK constructor with nulls, for now, until we know how to properly construct
            SpeakeasyHttpClient httpClient = null;
            string serverUrl = null;
            this.roomApi = new RoomV2SDK(
                httpClient,
                httpClient, 
                serverUrl,
                HathoraSdkConfig);
        }


        #region Client Room Async Hathora SDK Calls
        /// <summary>
        /// Gets connection info, like ip:port.
        /// (!) We'll poll until we have an `Active` Status: Be sure to await!
        /// </summary>
        /// <param name="_roomId">Get this from NetHathoraClientLobbyApi join/create</param>
        /// <param name="_pollIntervalSecs"></param>
        /// <param name="_pollTimeoutSecs"></param>
        /// <param name="_cancelToken"></param>
        /// <returns>Room on success</returns>
        public async Task<ConnectionInfoV2> ClientGetConnectionInfoAsync(
            string _roomId, 
            int _pollIntervalSecs = 1, 
            int _pollTimeoutSecs = 15,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraClientRoomApi)}.{nameof(ClientGetConnectionInfoAsync)}]";
            
            // Prep request
            HathoraSdk.Models.Operations.GetConnectionInfoRequest getConnectionInfoRequest = new()
            {
                RoomId = _roomId,
            };

            // Poll until we get the `Active` status.
            int pollSecondsTicked; // Duration to be logged later
            HathoraSdk.Models.Operations.GetConnectionInfoResponse getConnectionInfoResponse = null;
            
            for (pollSecondsTicked = 0; pollSecondsTicked < _pollTimeoutSecs; pollSecondsTicked++)
            {
                _cancelToken.ThrowIfCancellationRequested();
                
                try
                {
                    getConnectionInfoResponse = await roomApi.GetConnectionInfoAsync(getConnectionInfoRequest);
                }
                catch(Exception e)
                {
                    Debug.LogError($"{logPrefix} ");
                    return null; // fail
                }

                if (getConnectionInfoResponse.ConnectionInfoV2?.Status == ConnectionInfoV2Status.Active)
                    break;
                
                await Task.Delay(TimeSpan.FromSeconds(_pollIntervalSecs), _cancelToken);
            }

            // -----------------------------------------
            // We're done polling -- sucess or timeout?
            ConnectionInfoV2 connectionInfo = getConnectionInfoResponse?.ConnectionInfoV2;

            if (connectionInfo?.Status != ConnectionInfoV2Status.Active)
            {
                Debug.LogError($"{logPrefix} Error: Timed out");
                return null;
            }

            // Success
            Debug.Log($"{logPrefix} Success (after {pollSecondsTicked}s polling): <color=yellow>" +
                $"{nameof(getConnectionInfoResponse)}: {ToJson(getConnectionInfoResponse)}</color>");

            return connectionInfo;
        }
        #endregion // Client Room Async Hathora SDK Calls
    }
}

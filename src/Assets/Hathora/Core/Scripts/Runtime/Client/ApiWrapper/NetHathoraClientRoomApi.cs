// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Client.Config;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Client.ApiWrapper
{
    /// <summary>
    /// * Call Init() to pass UserConfig/instances.
    /// * Does not handle UI.
    /// * Does not handle Session caching.
    /// </summary>
    public class NetHathoraClientRoomApi : NetHathoraClientApiBase
    {
        private RoomV2Api roomApi;

        /// <summary>
        /// </summary>
        /// <param name="_hathoraClientConfig"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public override void Init(
            HathoraClientConfig _hathoraClientConfig, 
            Configuration _hathoraSdkConfig = null)
        {
            Debug.Log("[NetHathoraClientRoomApi] Initializing API...");
            base.Init(_hathoraClientConfig, _hathoraSdkConfig);
            this.roomApi = new RoomV2Api(base.HathoraSdkConfig);
        }


        #region Client Room Async Hathora SDK Calls
        /// <summary>
        /// Gets connection info, like ip:port.
        /// (!) We'll poll until we have an `Active` Status: Be sure to await!
        /// </summary>
        /// <param name="roomId">Get this from NetHathoraClientLobbyApi join/create</param>
        /// <param name="pollIntervalSecs"></param>
        /// <param name="pollTimeoutSecs"></param>
        /// <param name="_cancelToken"></param>
        /// <returns>Room on success</returns>
        public async Task<ConnectionInfoV2> ClientGetConnectionInfoAsync(
            string roomId, 
            int pollIntervalSecs = 1, 
            int pollTimeoutSecs = 15,
            CancellationToken _cancelToken = default)
        {
            // Poll until we get the `Active` status.
            int pollSecondsTicked; // Duration to be logged later
            ConnectionInfoV2 connectionInfoResponse = null;
            
            for (pollSecondsTicked = 0; pollSecondsTicked < pollTimeoutSecs; pollSecondsTicked++)
            {
                _cancelToken.ThrowIfCancellationRequested();
                
                try
                {
                    connectionInfoResponse = await roomApi.GetConnectionInfoAsync(
                        HathoraClientConfig.AppId, 
                        roomId,
                        _cancelToken);
                }
                catch(ApiException apiException)
                {
                    HandleClientApiException(
                        nameof(NetHathoraClientRoomApi),
                        nameof(ClientGetConnectionInfoAsync), 
                        apiException);
                    return null; // fail
                }

                
                if (connectionInfoResponse.Status == ConnectionInfoV2.StatusEnum.Active)
                    break;
                
                await Task.Delay(TimeSpan.FromSeconds(pollIntervalSecs), _cancelToken);
            }

            // -----------------------------------------
            // We're done polling -- sucess or timeout?
            if (connectionInfoResponse?.Status != ConnectionInfoV2.StatusEnum.Active)
            {
                Debug.LogError("[NetHathoraClientAuthApi.ClientGetConnectionInfoAsync] " +
                    "Error: Timed out");
                return null;
            }

            // Success
            Debug.Log($"[NetHathoraClientRoomApi.ClientGetConnectionInfoAsync] Success " +
                $"(after {pollSecondsTicked}s polling): <color=yellow>" +
                $"connectionInfoResponse: {connectionInfoResponse.ToJson()}</color>");

            return connectionInfoResponse;
        }
        #endregion // Client Room Async Hathora SDK Calls
    }
}

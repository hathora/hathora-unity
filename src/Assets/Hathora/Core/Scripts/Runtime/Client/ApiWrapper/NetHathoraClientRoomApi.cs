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
    /// </summary>
    public class NetHathoraClientRoomApi : NetHathoraClientApiBase
    {
        private RoomV2Api roomApi;

        /// <summary>
        /// </summary>
        /// <param name="_hathoraClientConfig"></param>
        /// <param name="_netSession"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public override void Init(
            HathoraClientConfig _hathoraClientConfig, 
            NetSession _netSession,
            Configuration _hathoraSdkConfig = null)
        {
            Debug.Log("[NetHathoraClientRoomApi] Initializing API...");
            base.Init(_hathoraClientConfig, _netSession, _hathoraSdkConfig);
            this.roomApi = new RoomV2Api(base.HathoraSdkConfig);
        }


        #region Client Room Async Hathora SDK Calls
        /// <summary>
        /// Gets connection info, like ip:port. Caches ConnectionInfo in NetSession.
        /// (!) We'll poll until we have an `Active` Status: Be sure to await!
        /// </summary>
        /// <param name="roomId">Get this from NetHathoraClientLobbyApi join/create</param>
        /// <param name="initPollTimerSecs"></param>
        /// <param name="pollTimeoutSecs"></param>
        /// <param name="_cancelToken"></param>
        /// <returns>Room on success</returns>
        public async Task<ConnectionInfoV2> ClientGetConnectionInfoAsync(
            string roomId, 
            float initPollTimerSecs = 0.1f, 
            float pollTimeoutSecs = 15f,
            CancellationToken _cancelToken = default)
        {
            float pollTimerTickedSecs = 0;
            
            // Poll until we get the `Active` status.
            ConnectionInfoV2 connectionInfoResponse = null;

            for (pollTimerTickedSecs = 0; pollTimerTickedSecs < pollTimeoutSecs; pollTimerTickedSecs++)
            {
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
                
                await Task.Delay(TimeSpan.FromSeconds(initPollTimerSecs), _cancelToken);
            }

            // -----------------------------------------
            // We're done polling -- sucess or timeout?
            if (connectionInfoResponse?.Status != ConnectionInfoV2.StatusEnum.Active)
            {
                Debug.LogError("[NetHathoraClientAuthApi]**ERR @ ClientGetConnectionInfoAsync: Timed out");
                return null;
            }

            // Success
            Debug.Log($"[NetHathoraClientRoomApi.ClientGetConnectionInfoAsync] Success " +
                $"(after {pollTimerTickedSecs}s polling): <color=yellow>" +
                $"connectionInfoResponse: {connectionInfoResponse.ToJson()}</color>");

            NetSession.ServerInfo = connectionInfoResponse;
            return connectionInfoResponse;
        }
        #endregion // Client Room Async Hathora SDK Calls
    }
}

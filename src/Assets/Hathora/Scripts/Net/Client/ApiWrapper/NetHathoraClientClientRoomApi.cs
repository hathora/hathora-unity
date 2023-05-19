// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using UnityEngine;

namespace Hathora.Scripts.Net.Client.ApiWrapper
{
    /// <summary>
    /// * Call Init() to pass UserConfig/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientClientRoomApi : NetHathoraClientApiBase
    {
        private RoomV2Api roomApi;

        /// <summary>
        /// </summary>
        /// <param name="_netHathoraConfig"></param>
        /// <param name="_netSession"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public override void Init(
            NetHathoraConfig _netHathoraConfig, 
            NetSession _netSession,
            Configuration _hathoraSdkConfig = null)
        {
            Debug.Log("[NetHathoraClientClientRoomApi] Initializing API...");
            base.Init(_netHathoraConfig, _netSession, _hathoraSdkConfig);
            this.roomApi = new RoomV2Api(base.HathoraSdkConfig);
        }


        #region Client Room Async Hathora SDK Calls
        /// <summary>
        /// Gets connection info, like ip:port. Caches ConnectionInfo in NetSession.
        /// (!) We'll poll until we have an `Active` Status: Be sure to await!
        /// </summary>
        /// <param name="roomId">Get this from NetHathoraClientClientLobbyApi join/create</param>
        /// <param name="initPollTimerSecs"></param>
        /// <param name="pollTimeoutSecs"></param>
        /// <returns>Room on success</returns>
        public async Task<ConnectionInfoV2> ClientGetConnectionInfoAsync(
            string roomId, 
            float initPollTimerSecs = 0.1f, 
            float pollTimeoutSecs = 10f)
        {
            float pollTimerTickedSecs = 0;
            
            // Poll until we get the `Active` status.
            ConnectionInfoV2 connectionInfoResponse = null;

            for (pollTimerTickedSecs = 0; pollTimerTickedSecs < pollTimeoutSecs; pollTimerTickedSecs++)
            {
                try
                {
                    connectionInfoResponse = await roomApi.GetConnectionInfoAsync(
                        NetHathoraConfig.HathoraCoreOpts.AppId, 
                        roomId);
                }
                catch(ApiException apiException)
                {
                    HandleClientApiException(
                        nameof(NetHathoraClientClientRoomApi),
                        nameof(ClientGetConnectionInfoAsync), 
                        apiException);
                    return null; // fail
                }

                
                if (connectionInfoResponse.Status == ConnectionInfoV2.StatusEnum.Active)
                    break;
                
                await Task.Delay(TimeSpan.FromSeconds(initPollTimerSecs));
            }

            // -----------------------------------------
            // We're done polling -- sucess or timeout?
            if (connectionInfoResponse?.Status != ConnectionInfoV2.StatusEnum.Active)
            {
                Debug.LogError("[NetHathoraClientClientAuthApi]**ERR @ ClientGetConnectionInfoAsync: Timed out");
                return null;
            }

            // Success
            Debug.Log($"[NetHathoraClientClientRoomApi] ClientGetConnectionInfoAsync => " +
                $"status: {connectionInfoResponse.Status}, duration: {pollTimerTickedSecs}s");

            NetSession.ServerInfo = connectionInfoResponse;
            return connectionInfoResponse;
        }
        #endregion // Client Room Async Hathora SDK Calls
    }
}

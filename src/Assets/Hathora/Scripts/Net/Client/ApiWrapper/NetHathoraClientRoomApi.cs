// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.Net.Server;
using UnityEngine;

namespace Hathora.Scripts.Net.Client.ApiWrapper
{
    /// <summary>
    /// * Call Init() to pass UserConfig/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientRoomApi : NetHathoraApiBase
    {
        private RoomV2Api roomApi;

        public override void Init(
            Configuration _hathoraSdkConfig, 
            NetHathoraConfig _netHathoraConfig, 
            NetSession _netSession)
        {
            Debug.Log("[NetHathoraClientRoomApi] Initializing API...");
            base.Init(_hathoraSdkConfig, _netHathoraConfig, _netSession);
            this.roomApi = new RoomV2Api(_hathoraSdkConfig);
        }


        #region Client Room Async Hathora SDK Calls
        /// <summary>
        /// Gets connection info, like ip:port. Caches ConnectionInfo in NetSession.
        /// (!) We'll poll until we have an `Active` Status: Be sure to await!
        /// </summary>
        /// <param name="roomId">Get this from NetHathoraClientLobbyApi join/create</param>
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
                    connectionInfoResponse = await roomApi.GetConnectionInfoAsync(NetHathoraConfig.AppId, roomId);
                }
                catch(Exception e)
                {
                    Debug.LogError($"[NetHathoraClientRoomApi] ClientGetConnectionInfoAsync poll err: {e.Message}");
                    await Task.FromException<Exception>(e);
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
                Debug.LogError("[NetHathoraClientAuthApi]**ERR @ ClientGetConnectionInfoAsync: Timed out");
                return null;
            }

            // Success
            Debug.Log($"[NetHathoraClientRoomApi] ClientGetConnectionInfoAsync => " +
                $"status: {connectionInfoResponse.Status}, duration: {pollTimerTickedSecs}s");

            NetSession.ServerInfo = connectionInfoResponse;
            return connectionInfoResponse;
        }
        #endregion // Client Room Async Hathora SDK Calls
    }
}

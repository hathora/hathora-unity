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
        private RoomV1Api roomApi;

        public override void Init(
            Configuration _hathoraSdkConfig, 
            NetHathoraConfig _netHathoraConfig, 
            NetSession _netSession)
        {
            Debug.Log("[NetHathoraClientRoomApi] Initializing API...");
            base.Init(_hathoraSdkConfig, _netHathoraConfig, _netSession);
            this.roomApi = new RoomV1Api(_hathoraSdkConfig);
        }


        #region Client Room Async Hathora SDK Calls
        /// <summary>
        /// Gets connection info, like ip:port. Caches ActiveConnectionInfo in NetSession.
        /// (!) We'll poll until we have an active connection: Be sure to await!
        /// - If the GetStartingConnectionInfo() parses, the `status` is probably !Active, including only status.
        /// - If the GetActiveConnectionInfo() parses, the `status` is probably Active (and includes ip:port).
        /// </summary>
        /// <param name="roomId">Get this from NetHathoraClientLobbyApi join/create</param>
        /// <param name="initPollTimerSecs"></param>
        /// <param name="pollTimeoutSecs"></param>
        /// <returns>Room on success</returns>
        public async Task<ActiveConnectionInfo> ClientGetConnectionInfoAsync(
            string roomId, 
            float initPollTimerSecs = 0.1f, 
            float pollTimeoutSecs = 10f)
        {
            float pollTimerTickedSecs = 0;
            
            // Poll until we get the active connection info.
            ActiveConnectionInfo activeConnectionInfo = null;

            for (pollTimerTickedSecs = 0; pollTimerTickedSecs < pollTimeoutSecs; pollTimerTickedSecs++)
            {
                ConnectionInfo conInfoUnionWrapper = null;
                
                try
                {
                    // Try getting an active connection wrapper - should not throw until we attempt to get the Union obj later
                    conInfoUnionWrapper = await roomApi.GetConnectionInfoAsync(NetHathoraConfig.AppId, roomId);
                }
                catch(Exception e)
                {
                    Debug.LogError($"[NetHathoraClientRoomApi] ClientGetConnectionInfoAsync poll err: {e.Message}");
                    await Task.FromException<Exception>(e);
                    return null; // fail
                }

                // Check if status is active -- since this is a Union, it'll throw err on null.
                try
                {
                    activeConnectionInfo = conInfoUnionWrapper.GetActiveConnectionInfo(); // Throws if null
                }
                catch(Exception e)
                {
                    Debug.LogWarning($"[NetHathoraClientRoomApi] Throw on conInfoUnionWrapper.GetActiveConnectionInfo, " +
                        $"but somewhat *expected*: {e.Message}");
                }
                
                if (activeConnectionInfo != null)
                    break;
                
                await Task.Delay(TimeSpan.FromSeconds(initPollTimerSecs));
            }

            // -----------------------------------------
            // We're done polling -- sucess or timeout?
            if (activeConnectionInfo == null)
            {
                Debug.LogError("[NetHathoraClientAuthApi]**ERR @ ClientGetConnectionInfoAsync: Timed out");
                return null;
            }

            // Success
            Debug.Log($"[NetHathoraClientRoomApi] ClientGetConnectionInfoAsync => " +
                $"status: {activeConnectionInfo.Status}, duration: {pollTimerTickedSecs}s");

            NetSession.ServerInfo = activeConnectionInfo;
            return activeConnectionInfo;
        }
        #endregion // Client Room Async Hathora SDK Calls
    }
}

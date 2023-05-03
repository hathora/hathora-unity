// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Client.Models;
using Hathora.Net.Server;
using JetBrains.Annotations;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// * Call Init() to pass config/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientRoom : NetHathoraApiBase
    {
        private RoomV1Api roomApi;

        public override void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _playerSession)
        {
            Debug.Log("[NetHathoraClientRoom] Initializing API...");
            base.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
            this.roomApi = new RoomV1Api(_hathoraSdkConfig);
        }


        #region Client Room Async Hathora SDK Calls
        /// <summary>
        /// Gets connection info, like ip:port.
        /// (!) We'll poll until we have an active connection: Be sure to await!
        /// - If the GetStartingConnectionInfo() parses, the `status` is probably !Active, including only status.
        /// - If the GetActiveConnectionInfo() parses, the `status` is probably Active (and includes ip:port).
        /// </summary>
        /// <param name="roomId">Get this from NetHathoraClientLobby join/create</param>
        /// <param name="initPollTimerSecs"></param>
        /// <param name="pollTimeoutSecs"></param>
        /// <returns>Room on success</returns>
        public async Task<ActiveConnectionInfo> ClientGetConnectionInfoAsync(
            string roomId, 
            float initPollTimerSecs = 0.1f, 
            float pollTimeoutSecs = 10f)
        {
            if (!base.IsClient)
                return null; // fail

            float pollTimerTickedSecs = 0;
            ActiveConnectionInfo activeConnectionInfo = null;
            
            try
            {
                // Poll until we get the active connection info.
                ConnectionInfo conInfoUnionWrapper = null;
                for (pollTimerTickedSecs = 0; pollTimerTickedSecs < pollTimeoutSecs; pollTimerTickedSecs++)
                {
                    try
                    {
                        // Try getting an active connection -- throws if null, but we'll try again until timeout.
                        conInfoUnionWrapper = await roomApi.GetConnectionInfoAsync(hathoraServerConfig.AppId, roomId);
                        activeConnectionInfo = conInfoUnionWrapper.GetActiveConnectionInfo(); // Throws if null
                        if (activeConnectionInfo != null)
                            break;
                    
                        await Task.Delay(TimeSpan.FromSeconds(initPollTimerSecs));
                    }
                    catch (Exception e)
                    {
                        // Let's report the status, but keep polling.
                        StartingConnectionInfo startConInfo = conInfoUnionWrapper?.GetStartingConnectionInfo();
                        Debug.Log("[NetHathoraClientRoom] ClientGetConnectionInfoAsync: " +
                            $"status: '{startConInfo?.Status}', iPollTimer: {pollTimerTickedSecs}/{pollTimeoutSecs}");
                    }
                }

                // We're done polling -- sucess or timeout?
                if (activeConnectionInfo == null)
                {
                    Debug.LogError("[NetHathoraClientAuth]**ERR @ ClientGetConnectionInfoAsync: Timed out");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClientRoom]**ERR @ ClientGetConnectionInfoAsync " +
                    $"(GetConnectionInfoAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null; // fail
            }

            // Success
            Debug.Log($"[NetHathoraClientRoom] ClientGetConnectionInfoAsync => " +
                $"status: {activeConnectionInfo.Status}, duration: {pollTimerTickedSecs}s");
            
            return activeConnectionInfo;
        }
        #endregion // Client Room Async Hathora SDK Calls
    }
}

// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Client.Config;
using HathoraSdk;
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
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public override void Init(
            HathoraClientConfig _hathoraClientConfig) 
            // Configuration _hathoraSdkConfig = null)
        {
            Debug.Log($"[{nameof(HathoraClientRoomApi)}] Initializing API...");
            
            // TODO: `Configuration` is missing in the new SDK - cleanup, if permanently gone.
            // base.Init(_hathoraClientConfig, _hathoraSdkConfig);
            // this.roomApi = new RoomV2SDK(base.HathoraSdkConfig);
            
            base.Init(_hathoraClientConfig);
            
            // TODO: Manually init w/out constructor, or add constructor support to model
            this.roomApi = new RoomV2SDK();
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
            string logPrefix = $"[{nameof(HathoraClientRoomApi)}.{nameof(ClientGetConnectionInfoAsync)}]";
            
            // Poll until we get the `Active` status.
            int pollSecondsTicked; // Duration to be logged later
            ConnectionInfoV2 connectionInfoResponse = null;
            
            // TODO: `StatusEnum` is missing in the new SDK - Find what to check for Active, instead; cleanup, if permanently gone.
            for (pollSecondsTicked = 0; pollSecondsTicked < pollTimeoutSecs; pollSecondsTicked++)
            {
                _cancelToken.ThrowIfCancellationRequested();
                
                try
                {
                    // TODO: The old SDK passed `AppId` -- how does the new SDK handle this if we don't pass AppId and don't init with a Sdk Configuration?
                    // TODO: Manually init w/out constructor, or add constructor support to model
                    connectionInfoResponse = await roomApi.GetConnectionInfoAsync(
                        HathoraClientConfig.AppId, 
                        roomId,
                        _cancelToken);
                }
                catch(Exception e)
                {
                    Debug.LogError($"{logPrefix} ");
                    return null; // fail
                }

                
                // // TODO: `StatusEnum` to check for `Active` status no longer exists in the new SDK - how to check for status?
                // if (connectionInfoResponse.Status == ConnectionInfoV2.StatusEnum.Active)
                //     break;
                
                await Task.Delay(TimeSpan.FromSeconds(pollIntervalSecs), _cancelToken);
            }

            // -----------------------------------------
            // We're done polling -- sucess or timeout?
            // // TODO: `StatusEnum` is missing in the new SDK - Find what to check for Active, instead; cleanup, if permanently gone.
            // if (connectionInfoResponse?.Status != ConnectionInfoV2.StatusEnum.Active)
            // {
            //     Debug.LogError($"{logPrefix} Error: Timed out");
            //     return null;
            // }

            // Success
            
            // TODO: `ToJson()` no longer exists in request/response models, but should soon make a return?
            // Debug.Log($"{logPrefix} Success (after {pollSecondsTicked}s polling): <color=yellow>" +
            //     $"connectionInfoResponse: {connectionInfoResponse.ToJson()}</color>");
            Debug.Log($"{logPrefix} Success (after {pollSecondsTicked}s polling)");

            return connectionInfoResponse;
        }
        #endregion // Client Room Async Hathora SDK Calls
    }
}

// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Models;
using Hathora.Scripts.Utils;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Editor.ApiWrapper
{
    public class HathoraServerRoomApi : HathoraServerApiBase
    {
        private readonly RoomV2Api roomApi;
        private HathoraLobbyRoomOpts roomOpts => NetHathoraConfig.HathoraLobbyRoomOpts;

        
        /// <summary>
        /// </summary>
        /// <param name="_netHathoraConfig"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public HathoraServerRoomApi( 
            NetHathoraConfig _netHathoraConfig,
            Configuration _hathoraSdkConfig = null)
            : base(_netHathoraConfig, _hathoraSdkConfig)
        {
            Debug.Log("[HathoraServerRoomApi] Initializing API...");
            this.roomApi = new RoomV2Api(base.HathoraSdkConfig);
        }
        
        
        #region Server Room Async Hathora SDK Calls
        /// <summary>
        /// Wrapper for `CreateRoomAsync` to upload and room a cloud room to Hathora.
        /// </summary>
        /// <returns>Returns Room on success</returns>
        public async Task<ConnectionInfoV2> CreateRoomAsync(
            string roomId,
            CancellationToken _cancelToken = default)
        {
            CreateRoomRequest createRoomReq = null;
            try
            {
                createRoomReq = new CreateRoomRequest(roomOpts.HathoraRegion);
                // createRoomReq.AdditionalProperties = // TODO  
                
                Debug.Log("[HathoraServerRoom.CreateRoomAsync] " +
                    $"roomConfig == <color=yellow>{createRoomReq.ToJson()}</color>");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }

            ConnectionInfoV2 createRoomResult;
            try
            {
                createRoomResult = await roomApi.CreateRoomAsync(
                    AppId,
                    createRoomReq,
                    roomId,
                    _cancelToken);
            }
            catch (ApiException apiErr)
            {
                HandleServerApiException(
                    nameof(HathoraServerRoomApi),
                    nameof(CreateRoomAsync), 
                    apiErr);
                return null;
            }

            Debug.Log($"[HathoraServerRoomApi] Success - " +
                $"RoomId: '{createRoomResult?.RoomId}', " +
                $"Status: '{createRoomResult?.Status}', " +
                $"ExposedPort: '{createRoomResult?.ExposedPort}");

            return createRoomResult;
        }
        #endregion // Server Room Async Hathora SDK Calls

    }
}

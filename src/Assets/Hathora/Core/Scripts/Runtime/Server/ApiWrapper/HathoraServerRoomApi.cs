// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Server.Models;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
    public class HathoraServerRoomApi : HathoraServerApiBase
    {
        private readonly RoomV2Api roomApi;
        private HathoraLobbyRoomOpts roomOpts => HathoraServerConfig.HathoraLobbyRoomOpts;

        
        /// <summary>
        /// </summary>
        /// <param name="_hathoraServerConfig"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public HathoraServerRoomApi( 
            HathoraServerConfig _hathoraServerConfig,
            Configuration _hathoraSdkConfig = null)
            : base(_hathoraServerConfig, _hathoraSdkConfig)
        {
            Debug.Log("[HathoraServerRoomApi] Initializing API...");
            this.roomApi = new RoomV2Api(base.HathoraSdkConfig);
        }
        
        
        #region Server Room Async Hathora SDK Calls
        /// <summary>
        /// Wrapper for `CreateRoomAsync` to upload and room a cloud room to Hathora.
        /// You generally want to call once to get the roomId, then poll GetRoomInfoAsync() 
        /// (with that roomId) until status is Active.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns>Returns Room on success</returns>
        public async Task<ConnectionInfoV2> CreateRoomAsync(
            string roomId = null,
            CancellationToken _cancelToken = default)
        {
            CreateRoomRequest createRoomReq = null;
            try
            {
                createRoomReq = new CreateRoomRequest(roomOpts.HathoraRegion);
                
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
            catch (TaskCanceledException)
            {
                // The user explicitly cancelled, or the Task timed out
                Debug.Log("[HathoraServerRoomApi.GetRoomInfoAsync] Task cancelled");
                return null;
            }
            catch (ApiException apiErr)
            {
                // HTTP err from Hathora Cloud
                HandleServerApiException(
                    nameof(HathoraServerRoomApi),
                    nameof(CreateRoomAsync), 
                    apiErr);
                return null;
            }

            Debug.Log($"[HathoraServerRoomApi] Success: " +
                $"<color=yellow>{createRoomResult.ToJson()}</color>");

            return createRoomResult;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="_roomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<Room> GetRoomInfoAsync(
            string _roomId, 
            CancellationToken _cancelToken = default)
        {
            Room getRoomInfoResult;

            try
            {
                getRoomInfoResult = await roomApi.GetRoomInfoAsync(
                    AppId,
                    _roomId,
                    _cancelToken);
            }
            catch (TaskCanceledException)
            {
                // The user explicitly cancelled, or the Task timed out
                Debug.Log("[HathoraServerRoomApi.GetRoomInfoAsync] Task cancelled");
                return null;
            }
            catch (ApiException apiErr)
            {
                // HTTP err from Hathora Cloud
                HandleServerApiException(
                    nameof(HathoraServerRoomApi),
                    nameof(GetRoomInfoAsync), 
                    apiErr);
                return null;
            }

            Debug.Log($"[HathoraServerRoomApi] Success: " +
                $"<color=yellow>{getRoomInfoResult.ToJson()}</color>");

            return getRoomInfoResult;
        }

        /// <summary>This shouldn't take long, so we poll once per second.</summary>
        /// <param name="_roomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<Room> PollGetRoomUntilActiveAsync(
            string _roomId,
            CancellationToken _cancelToken = default)
        {
            Room room = null;
            int attemptNum = 0;
            
            while (room is not { Status: RoomStatus.Active })
            {
                // Validate
                _cancelToken.ThrowIfCancellationRequested();

                if (room?.Status == RoomStatus.Destroyed)
                {
                    Debug.LogError("[HathoraConfigPostAuthBodyRoomUI.pollUntilCreatedRoom] " +
                        "Room was destroyed.");
                    return null;
                }
                
                if (room?.Status == RoomStatus.Suspended)
                {
                    Debug.LogError("[HathoraConfigPostAuthBodyRoomUI.pollUntilCreatedRoom] " +
                        "Room was suspended.");
                    return null;
                }
                
                // Try again
                attemptNum++;

                Debug.Log("[HathoraConfigPostAuthBodyRoomUI.pollUntilCreatedRoom] " +
                    $"Attempt #{attemptNum} ...");
                
                room = await GetRoomInfoAsync(
                    _roomId,
                    _cancelToken: _cancelToken);
                
                await Task.Delay(TimeSpan.FromSeconds(1), _cancelToken);
            }

            return room;
        }
        #endregion // Server Room Async Hathora SDK Calls
    }
}

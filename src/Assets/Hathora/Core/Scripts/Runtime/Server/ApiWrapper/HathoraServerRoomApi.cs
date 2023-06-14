// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Server.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
    public class HathoraServerRoomApi : HathoraServerApiBase
    {
        private readonly RoomV2Api roomApi;
        private HathoraLobbyRoomOpts roomOpts => HathoraServerConfig.HathoraLobbyRoomOpts;
        public bool IsPollingForActiveRoom { get; private set; }
        
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
        /// This is a HIGH-LEVEL func that does multiple things:
        /// </summary>
        /// 
        /// <list type="number">
        /// <item>Wrapper for `CreateRoomAwaitActiveAsync` to create a new room in Hathora</item>
        /// <item>Poll GetRoomInfoAsync(_customCreateRoomId): takes 5~10s, until Status is Active</item>
        /// <item>Once Active Status, get new ConnectionInfoV2 to get ExposedPort (connection info)</item>
        /// </list>
        /// 
        /// <param name="_customCreateRoomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns>both Room + [ACTIVE]ConnectionInfoV2 (ValueTuple) on success</returns>
        public async Task<(Room room, ConnectionInfoV2 connInfo)> CreateRoomAwaitActiveAsync(
            string _customCreateRoomId = null,
            CancellationToken _cancelToken = default)
        {
            string createdRoomId = await CreateRoomAsync(_customCreateRoomId, _cancelToken);

            // Once Room Status is Active, we can refresh the ConnectionInfo with ExposedPort
            Room activeRoom = null;
            try
            {
                activeRoom = await PollGetRoomUntilActiveAsync(
                    createdRoomId, 
                    _cancelToken);
                
                Assert.IsTrue(activeRoom?.Status == RoomStatus.Active, 
                    "activeRoom !Active status");
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraServerRoomApi.CreateRoomAwaitActiveAsync]: " +
                    $"{e.Message} (Check console.hathora.dev logs for +info)");
                throw;
            }


            ConnectionInfoV2 activeConnectionInfo = null;
            try
            {
                activeConnectionInfo = await GetConnectionInfoAsync(
                    activeRoom.RoomId,
                    _cancelToken);
                
                Assert.IsTrue(activeConnectionInfo?.Status == ConnectionInfoV2.StatusEnum.Active, 
                    "activeConnectionInfo !Active status (expected Active since room is Active");
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraServerRoomApi.CreateRoomAwaitActiveAsync]: " +
                    $"{e.Message} (Check console.hathora.dev logs for +info)");
                throw;
            }
            
            

            Debug.Log($"[HathoraServerRoomApi.CreateRoomAwaitActiveAsync] Success: " +
                $"<color=yellow>activeConnectionInfo: {activeConnectionInfo.ToJson()}</color>");

            return (activeRoom, activeConnectionInfo);
        }

        /// <summary>
        /// Wrapper for `CreateRoomAwaitActiveAsync` to create a new room in Hathora.
        /// </summary>
        /// <param name="_customCreateRoomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        private async Task<string> CreateRoomAsync(
            string _customCreateRoomId = null,
            CancellationToken _cancelToken = default)
        {
            // Prep request data
            CreateRoomRequest createRoomReq = null;
            try
            {
                createRoomReq = new CreateRoomRequest(roomOpts.HathoraRegion);
                
                Debug.Log("[HathoraServerRoom.CreateRoomAwaitActiveAsync] " +
                    $"<color=yellow>roomConfig: {createRoomReq.ToJson()}</color>");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }

            // Request call async =>
            ConnectionInfoV2 createRoomResultWithNullPort;
            try
            {
                // BUG: ExposedPort prop will always be null here; prop should be removed for CreateRoom.
                // To get the ExposedPort, we need to poll until Room Status is Active
                createRoomResultWithNullPort = await roomApi.CreateRoomAsync(
                    AppId,
                    createRoomReq,
                    _customCreateRoomId,
                    _cancelToken);
            }
            catch (TaskCanceledException)
            {
                // The user explicitly cancelled, or the Task timed out
                Debug.Log("[HathoraServerRoomApi.CreateRoomAsync] Task cancelled");
                return null;
            }
            catch (ApiException apiErr)
            {
                // HTTP err from Hathora Cloud
                HandleServerApiException(
                    nameof(HathoraServerRoomApi),
                    nameof(CreateRoomAwaitActiveAsync), 
                    apiErr);
                return null;
            }
            
            Debug.Log($"[HathoraServerRoomApi.CreateRoomAsync] Success: <color=yellow>" +
                $"createRoomResultWithNullPort: {createRoomResultWithNullPort.ToJson()}</color>");

            // Everything else in this result object is currently irrelevant except the RoomId
            return createRoomResultWithNullPort.RoomId;
        }
        
        /// <summary>
        /// When you get the result, check Status for Active.
        /// (!) If !Active, getting the ConnectionInfoV2 will result in !ExposedPort.
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
                $"<color=yellow>getRoomInfoResult: {getRoomInfoResult.ToJson()}</color>");

            return getRoomInfoResult;
        }

        /// <summary>
        /// (!) If the Room you created has a Status if !Active, the
        /// Result `ExposedPort` prop here will be null.
        /// </summary>
        /// <param name="_roomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<ConnectionInfoV2> GetConnectionInfoAsync(
            string _roomId, 
            CancellationToken _cancelToken = default)
        {
            ConnectionInfoV2 getConnectionInfoResult;

            try
            {
                getConnectionInfoResult = await roomApi.GetConnectionInfoAsync(
                    AppId,
                    _roomId,
                    _cancelToken);
            }
            catch (TaskCanceledException)
            {
                // The user explicitly cancelled, or the Task timed out
                Debug.Log("[HathoraServerRoomApi.GetConnectionInfoAsync] Task cancelled");
                return null;
            }
            catch (ApiException apiErr)
            {
                // HTTP err from Hathora Cloud
                HandleServerApiException(
                    nameof(HathoraServerRoomApi),
                    nameof(GetConnectionInfoAsync), 
                    apiErr);
                return null;
            }
            
            bool isActiveWithExposedPort = 
                getConnectionInfoResult.Status == ConnectionInfoV2.StatusEnum.Active && 
                getConnectionInfoResult.ExposedPort != null;

            Debug.Log($"[HathoraServerRoomApi] Success: " +
                $"isActiveWithExposedPort? {isActiveWithExposedPort}, " +
                $"<color=yellow>getConnectionInfoResult: {getConnectionInfoResult.ToJson()}</color>");

            return getConnectionInfoResult;
        }

        // ------------------------------------------
        // Utils >>
        
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
            IsPollingForActiveRoom = true;
            
            while (room is not { Status: RoomStatus.Active })
            {
                // Validate
                if (_cancelToken.IsCancellationRequested)
                {
                    IsPollingForActiveRoom = false;
                    _cancelToken.ThrowIfCancellationRequested();
                }

                if (room?.Status == RoomStatus.Destroyed)
                {
                    Debug.LogError("[HathoraConfigPostAuthBodyRoomUI.PollGetRoomUntilActiveAsync] " +
                        "Room was destroyed.");
                    
                    IsPollingForActiveRoom = false;
                    return null;
                }
                
                if (room?.Status == RoomStatus.Suspended)
                {
                    Debug.LogError("[HathoraConfigPostAuthBodyRoomUI.PollGetRoomUntilActiveAsync] " +
                        "Room was suspended.");

                    IsPollingForActiveRoom = false;
                    return null;
                }
                
                // Try again
                attemptNum++;

                Debug.Log("[HathoraConfigPostAuthBodyRoomUI.PollGetRoomUntilActiveAsync] " +
                    $"Attempt #{attemptNum} ...");
                
                room = await GetRoomInfoAsync(
                    _roomId,
                    _cancelToken: _cancelToken);
                
                await Task.Delay(TimeSpan.FromSeconds(1), _cancelToken);
            }
            
            Debug.Log($"[HathoraConfigPostAuthBodyRoomUI.PollGetRoomUntilActiveAsync] " +
                $"Done: <color=yellow>room: {room.ToJson()}</color>");

            IsPollingForActiveRoom = false;
            return room;
        }
        #endregion // Server Room Async Hathora SDK Calls
    }
}

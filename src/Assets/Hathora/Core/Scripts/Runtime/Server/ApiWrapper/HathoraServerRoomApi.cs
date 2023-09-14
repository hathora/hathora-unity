// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Server.Models;
using HathoraSdk;
using HathoraSdk.Models.Operations;
using HathoraSdk.Models.Shared;
using HathoraSdk.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using CreateRoomRequest = HathoraSdk.Models.Shared.CreateRoomRequest;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
    public class HathoraServerRoomApi : HathoraServerApiWrapperBase
    {
        private readonly RoomV2SDK roomApi;
        private HathoraLobbyRoomOpts roomOpts => HathoraServerConfig.HathoraLobbyRoomOpts;
        public bool IsPollingForActiveConnInfo { get; private set; }
        
        /// <summary>
        /// </summary>
        /// <param name="_hathoraServerConfig"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public HathoraServerRoomApi( 
            HathoraServerConfig _hathoraServerConfig,
            SDKConfig _hathoraSdkConfig = null)
            : base(_hathoraServerConfig, _hathoraSdkConfig)
        {
            Debug.Log("[HathoraServerRoomApi] Initializing API...");
            
            // TODO: Overloading VxSDK constructor with nulls, for now, until we know how to properly construct
            SpeakeasyHttpClient httpClient = null;
            string serverUrl = null;
            this.roomApi = new RoomV2SDK(
                httpClient,
                httpClient, 
                serverUrl,
                HathoraSdkConfig);
        }
        
        
        #region Server Room Async Hathora SDK Calls
        /// <summary>
        /// This is a HIGH-LEVEL func that does multiple things:
        /// </summary>
        /// 
        /// <list type="number">
        /// <item>Wrapper for `CreateRoomAwaitActiveAsync` to create a new room in Hathora</item>
        /// <item>Poll GetConnectionInfoV2Async(roomId): takes ~5s, until Status is `Active`</item>
        /// <item>Once Active Status, get Room info</item>
        /// </list>
        /// 
        /// <param name="_customCreateRoomId"></param>
        /// <param name="_cancelToken">TODO</param>
        /// <returns>both Room + [ACTIVE]ConnectionInfoV2 (ValueTuple) on success</returns>
        public async Task<(Room room, ConnectionInfoV2 connInfo)> CreateRoomAwaitActiveAsync(
            string _customCreateRoomId = null,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerRoomApi)}.{nameof(CreateRoomAwaitActiveAsync)}]";
            
            // (1/3) Create Room
            string newlyCreatedRoomId = null;
            try
            {
                newlyCreatedRoomId = await CreateRoomAsync(_customCreateRoomId, _cancelToken);
            }
            catch (TaskCanceledException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} CreateRoomAsync => Error: {e.Message}");
                throw;
            }
            
            // ----------
            // (2/3) Poll until `Active` connection Status (or timeout)
            ConnectionInfoV2 activeConnectionInfo = null;

            try
            {
                activeConnectionInfo = await PollConnectionInfoUntilActiveAsync(
                    newlyCreatedRoomId,
                    _cancelToken);

                Assert.IsTrue(activeConnectionInfo?.Status == ConnectionInfoV2Status.Active,
                    $"{logPrefix} {nameof(activeConnectionInfo)} Expected `Active` status, since Room is Active");
            }
            catch (TaskCanceledException e)
            {
                Debug.Log($"{logPrefix} Cancelled");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(PollConnectionInfoUntilActiveAsync)} => Error: {e.Message} " +
                    "(Check console.hathora.dev logs for +info)");
                throw;
            }
            
            // ----------
            // (3/3) Once Connection Status is `Active`, get Room info
            Room activeRoom = null;
            try
            {
                activeRoom = await GetRoomInfoAsync(
                    newlyCreatedRoomId, 
                    _cancelToken);
                
                Assert.IsTrue(activeRoom?.Status == RoomStatus.Active, 
                    $"{logPrefix} Expected activeRoom !Active status");
            }
            catch (TaskCanceledException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(GetRoomInfoAsync)} => Error: {e.Message} " +
                    "(Check console.hathora.dev logs for +info)");
                throw;
            }

            // ----------
            // Success
            Debug.Log($"{logPrefix} Success: <color=yellow>" +
                $"{nameof(activeConnectionInfo)}: {ToJson(activeConnectionInfo)}</color>");

            return (activeRoom, activeConnectionInfo);
        }

        /// <summary>
        /// Wrapper for `CreateRoomAwaitActiveAsync` to create a new room in Hathora.
        /// </summary>
        /// <param name="createRoomReq">Region, RoomConfig</param>
        /// <param name="_cancelToken">TODO</param>
        /// <returns></returns>
        private async Task<string> CreateRoomAsync(
            CreateRoomRequest createRoomReq,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerRoomApi)}.{nameof(CreateRoomAsync)}]";
            
            // Prep request data
            Debug.Log($"{logPrefix} <color=yellow>" +
                $"{nameof(createRoomReq)}: {ToJson(createRoomReq)}</color>");

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
                Debug.Log($"{logPrefix} Task cancelled");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(roomApi.CreateRoomAsync)} => Error: {e.Message}");
                return null; // fail
            }
            
            Debug.Log($"{logPrefix} Success: <color=yellow>{nameof(createRoomResultWithNullPort)}: " +
                $"{ToJson(createRoomResultWithNullPort)}</color>");

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
            string logPrefix = $"[{nameof(HathoraServerRoomApi)}.{nameof(GetRoomInfoAsync)}]";
            
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
                Debug.Log($"{logPrefix} Task cancelled");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(roomApi.GetRoomInfoAsync)} => Error: {e.Message}");
                return null; // fail
            }

            Debug.Log($"{logPrefix} Success: <color=yellow>" +
                $"{nameof(getRoomInfoResult)}: {ToJson(getRoomInfoResult)}</color>");

            return getRoomInfoResult;
        }
        
        /// <summary>
        /// Get all active rooms for a given process using `appId + `processId`.
        /// API Doc | https://hathora.dev/api#tag/RoomV2/operation/GetActiveRoomsForProcess 
        /// </summary>
        /// <param name="_processId">
        /// System generated unique identifier to a runtime instance of your game server.
        /// Example: "cbfcddd2-0006-43ae-996c-995fff7bed2e"
        /// </param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<List<GetActiveRoomsForProcessResponse>> GetActiveRoomsForProcessAsync(
            string _processId, 
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[HathoraServerRoomApi].{nameof(GetActiveRoomsForProcessAsync)}]";
            List<GetActiveRoomsForProcessResponse> getActiveRoomsResultList = null;

            try
            {
                getActiveRoomsResultList = await roomApi.GetActiveRoomsForProcessAsync(
                    AppId,
                    _processId,
                    _cancelToken);
            }
            catch (TaskCanceledException)
            {
                // The user explicitly cancelled, or the Task timed out
                Debug.Log($"{logPrefix} Task cancelled");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(roomApi.GetActiveRoomsForProcessAsync)} => Error: {e.Message}");
                return null; // fail
            }

            Debug.Log($"{logPrefix} Success: <color=yellow>" +
                $"getActiveRoomsResultList count: {getActiveRoomsResultList.Count}</color>");

            if (getActiveRoomsResultList.Count > 0)
            {
                Debug.Log($"{logPrefix} Success: <color=yellow>" +
                    $"{nameof(getActiveRoomsResultList)}[0]: {base.ToJson(getActiveRoomsResultList[0])}</color>");
            }

            return getActiveRoomsResultList;
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
            string logPrefix = $"[{nameof(HathoraServerRoomApi)}.{nameof(GetConnectionInfoAsync)}]";
            
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
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(roomApi.GetConnectionInfoAsync)} => Error: {e.Message}");
                return null; // fail
            }
            
            bool isActiveWithExposedPort = 
                getConnectionInfoResult.Status == ConnectionInfoV2Status.Active && 
                getConnectionInfoResult.ExposedPort != null;

            Debug.Log($"{logPrefix} Success: <color=yellow>" +
                $"{nameof(isActiveWithExposedPort)}? {isActiveWithExposedPort}, " +
                $"{nameof(getConnectionInfoResult)}: {ToJson(getConnectionInfoResult)}</color>");

            return getConnectionInfoResult;
        }

        // ------------------------------------------
        // Utils >>
        
        /// <summary>
        /// ETA 5 seconds; poll once/second.
        /// </summary>
        /// <param name="_roomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<ConnectionInfoV2> PollConnectionInfoUntilActiveAsync(
            string _roomId,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerRoomApi)}.{nameof(PollConnectionInfoUntilActiveAsync)}]";
            
            ConnectionInfoV2 connectionInfo = null;
            int attemptNum = 0;
            IsPollingForActiveConnInfo = true;
            
            while (connectionInfo is not { Status: ConnectionInfoV2Status.Active })
            {
                // Check for cancel + await 1s
                if (_cancelToken.IsCancellationRequested)
                {
                    IsPollingForActiveConnInfo = false;
                    _cancelToken.ThrowIfCancellationRequested();
                }
                
                await Task.Delay(TimeSpan.FromSeconds(1), _cancelToken);
        
                // ---------------
                // Try again
                attemptNum++;
                Debug.Log($"{logPrefix} Attempt #{attemptNum} ...");
                
                connectionInfo = await GetConnectionInfoAsync(
                    _roomId,
                    _cancelToken: _cancelToken);
                
                // ---------------
                if (connectionInfo?.Status == ConnectionInfoV2Status.Active)
                    break; // Success
                
                // ---------------
                // Still !Active -- log, then try again
                Debug.LogWarning($"{logPrefix} Room !Active (yet) - attempting to poll again ...");
            }
            
            Debug.Log($"{logPrefix} Success: <color=yellow>" +
                $"{nameof(connectionInfo)}: {ToJson(connectionInfo)}</color>");
        
            IsPollingForActiveConnInfo = false;
            return connectionInfo;
        }
        #endregion // Server Room Async Hathora SDK Calls
    }
}

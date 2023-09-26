// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Common.Utils;
using HathoraCloud;
using HathoraCloud.Models.Operations;
using HathoraCloud.Models.Shared;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Common.ApiWrapper
{
    /// <summary>
    /// Common+Client API calls for Lobby.
    /// Operations to create and manage lobbies.
    /// Lobbies Concept | https://hathora.dev/docs/concepts/hathora-entities#lobby 
    /// API Docs | https://hathora.dev/api#tag/LobbyV2 
    /// </summary>
    public class HathoraLobbyApiWrapper : HathoraApiWrapperBase
    {
        protected LobbyV2SDK LobbyApi { get; }

        public HathoraLobbyApiWrapper(HathoraCloudSDK _hathoraSdk)
            : base(_hathoraSdk)
        {
            Debug.Log($"[{nameof(HathoraLobbyApiWrapper)}.Constructor] " +
                "Initializing Common API...");
            
            this.LobbyApi = _hathoraSdk.LobbyV2 as LobbyV2SDK;
        }


        #region Client Lobby Async Hathora SDK Calls
        /// <summary>
        /// Create a new Player Client Lobby.
        /// </summary>
        /// <param name="_playerAuthToken">
        /// Player Auth Token (likely from a cached session)</param>
        /// <param name="_lobbyVisibility"></param>
        /// <param name="_initConfigObj">
        /// Pass your own serializable model (or json string):
        /// - These are the initial values included in the Lobby.
        /// - Eg: `MaxNumPlayers`
        /// - Required, since a 'Lobby' without an InitConfig is just a 'Room'. 
        /// </param>
        /// <param name="_roomId">Null will auto-generate</param>
        /// <param name="_cancelToken"></param>
        /// <param name="_region">(!) Index starts at 1 (not 0)</param>
        /// <returns>Lobby on success</returns>
        public virtual async Task<Lobby> CreateLobbyAsync(
            string _playerAuthToken,
            object _initConfigObj,
            Region _region = HathoraUtils.DEFAULT_REGION,
            LobbyVisibility _lobbyVisibility = LobbyVisibility.Public,
            string _roomId = null,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraLobbyApiWrapper)}.{nameof(CreateLobbyAsync)}]";

            
            CreateLobbySecurity createLobbySecurity = new() { PlayerAuth = _playerAuthToken };
            CreateLobbyParams createLobbyParams = new()
            {
                Region = _region,
                Visibility = _lobbyVisibility,
                InitialConfig = _initConfigObj, // TODO: SDK `InitialConfig` should take a serializable <object>
            };
            
            CreateLobbyRequest createLobbyRequestWrapper = new()
            {
                CreateLobbyParams = createLobbyParams,
                RoomId = _roomId,
            };

            Debug.Log($"{logPrefix} <color=yellow>{nameof(createLobbyParams)}: " +
                $"{ToJson(createLobbyParams)}</color>");

            CreateLobbyResponse createLobbyResponse = null;

            try
            {
                createLobbyResponse = await LobbyApi.CreateLobbyAsync(createLobbySecurity, createLobbyRequestWrapper);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(LobbyApi.CreateLobbyAsync)} => Error: {e.Message}");
                return null; // fail
            }

            Debug.Log($"{logPrefix} <color=yellow>{nameof(createLobbyResponse)}: " +
                $"{ToJson(createLobbyResponse.Lobby)}</color>");
            
            createLobbyResponse.RawResponse?.Dispose(); // Prevent mem leaks
            return createLobbyResponse.Lobby;
        }

        /// <summary>
        /// Gets Lobby info, if we already know the _roomId.
        /// (!) Creating a room will also return Lobby info; you probably want to
        ///     do this if interested in *joining*.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public virtual async Task<Lobby> GetLobbyInfoAsync(
            string roomId,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraLobbyApiWrapper)}.{nameof(CreateLobbyAsync)}]";
            Debug.Log($"{logPrefix} <color=yellow>_roomId: {roomId}</color>");

            // Prep request
            GetLobbyInfoRequest getLobbyInfoRequest = new()
            {
                RoomId = roomId,
            };
            
            // Get response async =>
            GetLobbyInfoResponse lobbyInfoResponse = null;
            
            try
            {
                lobbyInfoResponse = await LobbyApi.GetLobbyInfoAsync(getLobbyInfoRequest);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(LobbyApi.GetLobbyInfoAsync)} => Error: {e.Message}");
                return null; // fail
            }
            
            if (lobbyInfoResponse.StatusCode == 404)
            {
                Debug.LogError($"{logPrefix} 404: {lobbyInfoResponse.GetLobbyInfo404ApplicationJSONString} - " +
                    "Tip: If a server made a Room without a lobby, instead use the Room api (rather than Lobby api)");
            }
            
            Debug.Log($"{logPrefix} Success: <color=yellow>{nameof(lobbyInfoResponse)}: " +
                $"{ToJson(lobbyInfoResponse.Lobby)}</color>");      
            
            lobbyInfoResponse.RawResponse?.Dispose(); // Prevent mem leaks
            return lobbyInfoResponse.Lobby;
        }

        /// <summary>
        /// Gets a list of active+public lobbies.
        /// </summary>
        /// <param name="_request">Leave Region null to return ALL Regions</param>
        /// <param name="_cancelToken">TODO</param>
        /// <returns></returns>
        public virtual async Task<List<Lobby>> ListPublicLobbiesAsync(
            ListActivePublicLobbiesRequest _request,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraLobbyApiWrapper)}.{nameof(ListPublicLobbiesAsync)}]";
            string regionLogStr = _request.Region == null ? "Any" : _request.Region.ToString();
            Debug.Log($"{logPrefix} <color=yellow>Getting public+active lobbies for " +
                $"'{(regionLogStr)}' region</color>");

            ListActivePublicLobbiesResponse activePublicLobbiesResponse = null;
            
            try
            {
                activePublicLobbiesResponse = await LobbyApi.ListActivePublicLobbiesAsync(_request);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(LobbyApi.ListActivePublicLobbiesAsync)} => Error: {e.Message}");
                return null; // fail
            }

            if (activePublicLobbiesResponse.StatusCode == 404)
            {
                Debug.LogError($"{logPrefix} 404: If a server made a Room without a lobby, " +
                    $"instead use the {nameof(HathoraRoomApiWrapper)} (not {nameof(HathoraLobbyApiWrapper)})");
            }

            Debug.Log($"{logPrefix} => numLobbiesFound: {activePublicLobbiesResponse.Lobbies?.Count ?? 0}");
            
            activePublicLobbiesResponse.RawResponse?.Dispose(); // Prevent mem leaks
            return activePublicLobbiesResponse.Lobbies;
        }
        #endregion // Client Lobby Async Hathora SDK Calls
    }
}

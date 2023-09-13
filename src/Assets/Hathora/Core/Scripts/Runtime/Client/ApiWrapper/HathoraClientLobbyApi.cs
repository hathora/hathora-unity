// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Client.Config;
using HathoraSdk;
using HathoraSdk.Models.Operations;
using HathoraSdk.Models.Shared;
using HathoraSdk.Utils;
using UnityEngine;
using CreateLobbyRequest = HathoraSdk.Models.Shared.CreateLobbyRequest;

namespace Hathora.Core.Scripts.Runtime.Client.ApiWrapper
{
    /// <summary>
    /// High-level API wrapper for the low-level Hathora SDK's Lobby API.
    /// * Caches SDK Config and HathoraClientConfig for API use. 
    /// * Try/catches async API calls and [Base] automatically handlles API Exceptions.
    /// * Due to code autogen, the SDK exposes too much: This simplifies and minimally exposes.
    /// * Due to code autogen, the SDK sometimes have nuances: This provides fixes/workarounds.
    /// * Call Init() to pass HathoraClientConfig + Hathora SDK Config (see HathoraClientMgr).
    /// * Does not handle UI (see HathoraClientMgrUi).
    /// * Does not handle Session caching (see HathoraClientSession).
    /// </summary>
    public class HathoraClientLobbyApi : HathoraClientApiWrapperBase
    {
        private LobbyV2SDK lobbyApi;

        /// <summary>Monobehaviour workaround for a constructor.</summary>
        /// <param name="_hathoraClientConfig"></param>
        /// <param name="_hathoraSdkConfig"></param>
        public override void Init(
            HathoraClientConfig _hathoraClientConfig,
            SDKConfig _hathoraSdkConfig = null)
        {
            Debug.Log($"[{nameof(HathoraClientLobbyApi)}] Initializing API...");
            base.Init(_hathoraClientConfig, _hathoraSdkConfig);
            
            // TODO: Overloading VxSDK constructor with nulls, for now, until we know how to properly construct
            SpeakeasyHttpClient httpClient = null;
            string serverUrl = null;
            this.lobbyApi = new LobbyV2SDK(
                httpClient,
                httpClient, 
                serverUrl,
                HathoraSdkConfig);
        }


        #region Client Lobby Async Hathora SDK Calls
        /// <summary>
        /// Create a new Player Client Lobby.
        /// </summary>
        /// <param name="_playerAuthToken">Player Auth Token (likely from a cached session)</param>
        /// <param name="lobbyVisibility"></param>
        /// <param name="_initConfigJsonStr"></param>
        /// <param name="roomId">Null will auto-generate</param>
        /// <param name="_cancelToken"></param>
        /// <param name="_region">(!) Index starts at 1 (not 0)</param>
        /// <returns>Lobby on success</returns>
        public async Task<Lobby> ClientCreateLobbyAsync(
            string _playerAuthToken,
            LobbyVisibility lobbyVisibility,
            Region _region = Region.WashingtonDC,
            string _initConfigJsonStr = "{}",
            string roomId = null,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraClientLobbyApi)}.{nameof(ClientCreateLobbyAsync)}]";
            
            CreateLobbyRequest createLobbyRequest = new(
                lobbyVisibility, 
                _initConfigJsonStr, 
                _region);

            Debug.Log($"{logPrefix} <color=yellow>{nameof(createLobbyRequest)}: " +
                $"{ToJson(createLobbyRequest)}</color>");

            CreateLobbyResponse createLobbyResponse = null;

            try
            {
                // TODO: The old SDK passed `AppId` -- how does the new SDK handle this if we don't pass AppId and don't init with a Sdk Configuration?
                // TODO: Manually init w/out constructor, or add constructor support to model
                createLobbyResponse = await lobbyApi.CreateLobbyAsync(
                    HathoraClientConfig.AppId,
                    _playerAuthToken, // Player token; not dev
                    createLobbyRequest,
                    roomId,
                    _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(lobbyApi.CreateLobbyAsync)} => Error: {e.Message}");
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientAuthApi.ClientCreateLobbyAsync] Success: " +
                $"<color=yellow>{nameof(createLobbyResponse)}: {ToJson(createLobbyResponse)}</color>");
            
            return createLobbyResponse.Lobby;
        }

        /// <summary>
        /// Gets Lobby info, if we already know the roomId.
        /// (!) Creating a room will also return Lobby info; you probably want to
        ///     do this if interested in *joining*.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<Lobby> ClientGetLobbyInfoAsync(
            string roomId,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraClientLobbyApi)}.{nameof(ClientCreateLobbyAsync)}]";
            Debug.Log($"{logPrefix} <color=yellow>roomId: {roomId}</color>");

            GetLobbyInfoResponse lobbyInfoResponse = null;
            
            try
            {
                // TODO: The old SDK passed `AppId` -- how does the new SDK handle this if we don't pass AppId and don't init with a Sdk Configuration?
                // TODO: Manually init w/out constructor, or add constructor support to model
                lobbyInfoResponse = await lobbyApi.GetLobbyInfoAsync(
                    HathoraClientConfig.AppId,
                    roomId,
                    _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(lobbyApi.GetLobbyInfoAsync)} => Error: {e.Message}");
                return null; // fail
            }
            
            if (lobbyInfoResponse.StatusCode == 404)
            {
                Debug.LogError($"{logPrefix} 404: {lobbyInfoResponse.GetLobbyInfo404ApplicationJSONString} - " +
                    "Tip: If a server made a Room without a lobby, instead use the Room api (rather than Lobby api)");
            }
            
            Debug.Log($"{logPrefix} Success: <color=yellow>" +
                $"{nameof(lobbyInfoResponse)}: {ToJson(lobbyInfoResponse)}</color>");      
            
            return lobbyInfoResponse.Lobby;
        }

        /// <summary>
        /// Gets a list of active+public lobbies.
        /// </summary>
        /// <param name="_request">Leave Region null to return ALL Regions</param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<List<Lobby>> ClientListPublicLobbiesAsync(
            ListActivePublicLobbiesRequest _request,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraClientLobbyApi)}.{nameof(ClientListPublicLobbiesAsync)}]";
            string regionLogStr = _request.Region == null ? "Any" : _request.Region.ToString();
            Debug.Log($"{logPrefix} <color=yellow>Getting public+active lobbies for " +
                $"'{(regionLogStr)}' region</color>");

            ListActivePublicLobbiesResponse activePublicLobbiesResponse = null;
            
            try
            {
                // TODO: The old SDK passed `AppId` -- how does the new SDK handle this if we don't pass AppId and don't init with a Sdk Configuration?
                // TODO: Manually init w/out constructor, or add constructor support to model
                activePublicLobbiesResponse = await lobbyApi.ListActivePublicLobbiesAsync(_request);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(lobbyApi.ListActivePublicLobbiesAsync)} => Error: {e.Message}");
                return null; // fail
            }

            if (activePublicLobbiesResponse.StatusCode == 404)
            {
                Debug.LogError($"{logPrefix} 404: If a server made a Room without a lobby, " +
                    $"instead use the {nameof(HathoraClientRoomApi)} (not {nameof(HathoraClientLobbyApi)})");
            }

            Debug.Log($"{logPrefix} => numLobbiesFound: {activePublicLobbiesResponse.Lobbies?.Count ?? 0}");
            return activePublicLobbiesResponse.Lobbies;
        }
        #endregion // Client Lobby Async Hathora SDK Calls
    }
}

// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Client.Config;
using HathoraSdk;
using HathoraSdk.Models.Shared;
using UnityEngine;

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

        /// <summary>
        /// </summary>
        /// <param name="_hathoraClientConfig"></param>
        public override void Init(
            HathoraClientConfig _hathoraClientConfig)
            // SDKConfig _hathoraSdkConfig = null)
        {
            Debug.Log($"[{nameof(HathoraClientLobbyApi)}] Initializing API...");
            
            // TODO: `Configuration` is missing in the new SDK - cleanup, if permanently gone.
            // base.Init(_hathoraClientConfig, _hathoraSdkConfig);
            // this.lobbyApi = new LobbyV2SDK(base.HathoraSdkConfig);
            
            base.Init(_hathoraClientConfig);
            
            // TODO: Manually init w/out constructor, or add constructor support to model
            this.lobbyApi = new LobbyV2SDK();
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
            CreateLobbyRequest.VisibilityEnum lobbyVisibility,
            Region _region = Region.WashingtonDC,
            string _initConfigJsonStr = "{}",
            string roomId = null,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraClientLobbyApi)}.{nameof(ClientCreateLobbyAsync)}]";
            
            CreateLobbyRequest request = new(
                lobbyVisibility, 
                _initConfigJsonStr, 
                _region);

            // TODO: `ToJson()` no longer exists in request/response models, but should soon make a return?
            // Debug.Log($"{logPrefix} <color=yellow>request: {request.ToJson()}</color>");
            Debug.Log($"{logPrefix} Start");

            Lobby lobby;

            try
            {
                // TODO: The old SDK passed `AppId` -- how does the new SDK handle this if we don't pass AppId and don't init with a Sdk Configuration?
                // TODO: Manually init w/out constructor, or add constructor support to model
                lobby = await lobbyApi.CreateLobbyAsync(
                    HathoraClientConfig.AppId,
                    _playerAuthToken, // Player token; not dev
                    request,
                    roomId,
                    _cancelToken);
            }
            catch (Exception e)
            {
                Debug.Log($"{logPrefix}");
            }
            catch (ApiException apiException)
            {
                HandleApiException(
                    nameof(HathoraClientLobbyApi),
                    nameof(ClientCreateLobbyAsync), 
                    apiException);
                return null; // fail
            }

            //// TODO: `ToJson()` no longer exists in request/response models, but should soon make a return?
            // Debug.Log($"[NetHathoraClientAuthApi.ClientCreateLobbyAsync] Success: " +
            //     $"<color=yellow>lobby: {lobby.ToJson()}</color>");
            Debug.Log($"{logPrefix} Success");
            
            return lobby;
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
            
            Lobby lobby;
            try
            {
                // TODO: The old SDK passed `AppId` -- how does the new SDK handle this if we don't pass AppId and don't init with a Sdk Configuration?
                // TODO: Manually init w/out constructor, or add constructor support to model
                lobby = await lobbyApi.GetLobbyInfoAsync(
                    HathoraClientConfig.AppId,
                    roomId,
                    _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleApiException(
                    nameof(HathoraClientLobbyApi),
                    nameof(ClientGetLobbyInfoAsync), 
                    apiException);
                
                if (apiException.ErrorCode == 404)
                {
                    Debug.LogError($"{logPrefix} 404 - Tip: If a server made a Room without a lobby, " +
                        "instead use the Room api (rather than Lobby api)");
                }
                
                return null; // fail
            }

            // TODO: `ToJson()` no longer exists in request/response models, but should soon make a return?
            // Debug.Log($"{logPrefix} Success: <color=yellow>lobby: {lobby.ToJson()}</color>");      
            Debug.Log($"{logPrefix} Success");      
            
            return lobby;
        }

        /// <summary>
        /// Gets a list of active+public lobbies.
        /// </summary>
        /// <param name="_region">Leave null to return ALL Regions</param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<List<Lobby>> ClientListPublicLobbiesAsync(
            Region? _region = null, // null == ALL regions
            CancellationToken _cancelToken = default)
        {
            string logsPrefix = $"[HathoraClientLobbyApi.{nameof(ClientListPublicLobbiesAsync)}]";
            string regionStr = _region == null ? "any" : _region.ToString();
            
            Debug.Log($"{logsPrefix} <color=yellow>Getting public+active lobbies for '{regionStr}' region</color>");
            
            List<Lobby> lobbies;
            try
            {
                // TODO: The old SDK passed `AppId` -- how does the new SDK handle this if we don't pass AppId and don't init with a Sdk Configuration?
                // TODO: Manually init w/out constructor, or add constructor support to model
                lobbies = await lobbyApi.ListActivePublicLobbiesAsync(
                    HathoraClientConfig.AppId,
                    region: _region,
                    _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleApiException(
                    nameof(HathoraClientLobbyApi),
                    nameof(ClientListPublicLobbiesAsync), 
                    apiException);
                
                if (apiException.ErrorCode == 404)
                    Debug.LogError($"{logsPrefix} 404: If a server made a Room without a lobby, " +
                        "instead use the Room api (rather than Lobby api)");
                
                return null; // fail
            }

            Debug.Log($"{logsPrefix} => numLobbiesFound: {lobbies?.Count ?? 0}");
            return lobbies;
        }
        #endregion // Client Lobby Async Hathora SDK Calls
    }
}

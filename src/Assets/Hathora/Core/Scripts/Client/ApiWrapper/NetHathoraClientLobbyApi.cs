// Created by dylan@hathora.dev

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Client.Config;
using UnityEngine;

namespace Hathora.Core.Scripts.Client.ApiWrapper
{
    /// <summary>
    /// * Call Init() to pass UserConfig/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientLobbyApi : NetHathoraClientApiBase
    {
        private LobbyV2Api lobbyApi;

        /// <summary>
        /// </summary>
        /// <param name="_hathoraClientConfig"></param>
        /// <param name="_netSession"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public override void Init(
            HathoraClientConfig _hathoraClientConfig,
            NetSession _netSession,
            Configuration _hathoraSdkConfig = null)
        {
            Debug.Log("[NetHathoraClientLobbyApi] Initializing API...");
            base.Init(_hathoraClientConfig, _netSession, _hathoraSdkConfig);
            this.lobbyApi = new LobbyV2Api(base.HathoraSdkConfig);
        }


        #region Client Lobby Async Hathora SDK Calls
        /// <summary>
        /// Create a new Player Client Lobby.
        /// </summary>
        /// <param name="lobbyVisibility"></param>
        /// <param name="_initConfigJsonStr"></param>
        /// <param name="_cancelToken"></param>
        /// <param name="_region"></param>
        /// <returns>Lobby on success</returns>
        public async Task<Lobby> ClientCreateLobbyAsync(
            CreateLobbyRequest.VisibilityEnum lobbyVisibility,
            Region _region,
            string _initConfigJsonStr = "{}",
            CancellationToken _cancelToken = default)
        {
            CreateLobbyRequest request = new(
                lobbyVisibility, 
                _initConfigJsonStr, 
                _region);

            Lobby lobby;
            try
            {
                lobby = await lobbyApi.CreateLobbyAsync(
                    HathoraClientConfig.AppId,
                    NetSession.PlayerAuthToken, // Player token; not dev
                    request,
                    cancellationToken: _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleClientApiException(
                    nameof(NetHathoraClientLobbyApi),
                    nameof(ClientCreateLobbyAsync), 
                    apiException);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientLobbyApi] ClientCreateLobbyAsync => roomId: {lobby.RoomId}");
            NetSession.Lobby = lobby;
            
            return lobby;
        }

        /// <summary>
        /// Gets Lobby info, if we already know the roomId.
        /// (!) Creating a room will also return Lobby info; you probably want to do this if interested in *joining*.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<Lobby> ClientGetLobbyInfoAsync(
            string roomId,
            CancellationToken _cancelToken = default)
        {
            Lobby lobby;
            try
            {
                lobby = await lobbyApi.GetLobbyInfoAsync(
                    HathoraClientConfig.AppId,
                    roomId,
                    _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleClientApiException(
                    nameof(NetHathoraClientLobbyApi),
                    nameof(ClientGetLobbyInfoAsync), 
                    apiException);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientLobbyApi] ClientGetLobbyInfoAsync => roomId: {lobby.RoomId}");
            NetSession.Lobby = lobby;
            
            return lobby;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<List<Lobby>> ClientListPublicLobbiesAsync(
            CancellationToken _cancelToken = default)
        {
            List<Lobby> lobbies;
            try
            {
                lobbies = await lobbyApi.ListActivePublicLobbiesAsync(
                    HathoraClientConfig.AppId,
                    HathoraClientConfig.FallbackRegion,
                    _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleClientApiException(
                    nameof(NetHathoraClientLobbyApi),
                    nameof(ClientListPublicLobbiesAsync), 
                    apiException);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientLobbyApi] ClientListPublicLobbiesAsync => " +
                $"numLobbiesFound: {lobbies?.Count ?? 0}");
            
            return lobbies;
        }
        #endregion // Client Lobby Async Hathora SDK Calls
    }
}
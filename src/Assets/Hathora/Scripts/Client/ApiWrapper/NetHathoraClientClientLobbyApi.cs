// Created by dylan@hathora.dev

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Scripts.Common;
using Hathora.Scripts.Sdk.hathora_cloud_sdks.csharp.src.Hathora.Cloud.Sdk.Api;
using Hathora.Scripts.Sdk.hathora_cloud_sdks.csharp.src.Hathora.Cloud.Sdk.Client;
using Hathora.Scripts.Sdk.hathora_cloud_sdks.csharp.src.Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Server.Config;
using UnityEngine;

namespace Hathora.Scripts.Client.ApiWrapper
{
    /// <summary>
    /// * Call Init() to pass UserConfig/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientClientLobbyApi : NetHathoraClientApiBase
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
            Debug.Log("[NetHathoraClientClientLobbyApi] Initializing API...");
            base.Init(_hathoraClientConfig, _netSession, _hathoraSdkConfig);
            this.lobbyApi = new LobbyV2Api(base.HathoraSdkConfig);
        }


        #region Client Lobby Async Hathora SDK Calls
        /// <summary>
        /// Create a new Player Client Lobby.
        /// </summary>
        /// <param name="lobbyVisibility"></param>
        /// <param name="_cancelToken"></param>
        /// <returns>Lobby on success</returns>
        public async Task<Lobby> ClientCreateLobbyAsync(
            CreateLobbyRequest.VisibilityEnum lobbyVisibility,
            string _initConfigJsonStr = "{}",
            CancellationToken _cancelToken = default)
        {
            CreateLobbyRequest request = new(
                lobbyVisibility, 
                _initConfigJsonStr, 
                HathoraClientConfig.HathoraLobbyRoomOpts.HathoraRegion);

            Lobby lobby;
            try
            {
                lobby = await lobbyApi.CreateLobbyAsync(
                    HathoraClientConfig.HathoraCoreOpts.AppId,
                    NetSession.PlayerAuthToken, // Player token; not dev
                    request,
                    cancellationToken: _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleClientApiException(
                    nameof(NetHathoraClientClientLobbyApi),
                    nameof(ClientCreateLobbyAsync), 
                    apiException);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientClientLobbyApi] ClientCreateLobbyAsync => roomId: {lobby.RoomId}");
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
                    HathoraClientConfig.HathoraCoreOpts.AppId,
                    roomId,
                    _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleClientApiException(
                    nameof(NetHathoraClientClientLobbyApi),
                    nameof(ClientGetLobbyInfoAsync), 
                    apiException);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientClientLobbyApi] ClientGetLobbyInfoAsync => roomId: {lobby.RoomId}");
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
                    HathoraClientConfig.HathoraCoreOpts.AppId,
                    HathoraClientConfig.HathoraLobbyRoomOpts.HathoraRegion,
                    _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleClientApiException(
                    nameof(NetHathoraClientClientLobbyApi),
                    nameof(ClientListPublicLobbiesAsync), 
                    apiException);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientClientLobbyApi] ClientListPublicLobbiesAsync => " +
                $"numLobbiesFound: {lobbies?.Count ?? 0}");
            
            return lobbies;
        }
        #endregion // Client Lobby Async Hathora SDK Calls
    }
}

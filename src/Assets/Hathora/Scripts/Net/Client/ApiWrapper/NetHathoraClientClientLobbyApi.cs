// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Client.Models;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Models;
using JetBrains.Annotations;
using UnityEngine;

namespace Hathora.Scripts.Net.Client.ApiWrapper
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
        /// <param name="_netHathoraConfig"></param>
        /// <param name="_netSession"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public override void Init(
            NetHathoraConfig _netHathoraConfig,
            NetSession _netSession,
            Configuration _hathoraSdkConfig = null)
        {
            Debug.Log("[NetHathoraClientClientLobbyApi] Initializing API...");
            base.Init(_netHathoraConfig, _netSession, _hathoraSdkConfig);
            this.lobbyApi = new LobbyV2Api(base.HathoraSdkConfig);
        }


        #region Client Lobby Async Hathora SDK Calls
        /// <summary>
        /// Create a new Player Client Lobby.
        /// </summary>
        /// <param name="lobbyVisibility"></param>
        /// <returns>Lobby on success</returns>
        public async Task<Lobby> ClientCreateLobbyAsync(CreateLobbyRequest.VisibilityEnum lobbyVisibility)
        {
            LobbyInitConfig lobbyInitConfig = new();
            CreateLobbyRequest request = new(
                lobbyVisibility, 
                lobbyInitConfig, 
                NetHathoraConfig.HathoraLobbyRoomOpts.HathoraRegion);

            Lobby lobby;
            try
            {
                lobby = await lobbyApi.CreateLobbyAsync(
                    NetHathoraConfig.HathoraCoreOpts.AppId,
                    NetSession.PlayerAuthToken, // Player token; not dev
                    request);
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
        /// <returns></returns>
        public async Task<Lobby> ClientGetLobbyInfoAsync(string roomId)
        {
            Lobby lobby;
            try
            {
                lobby = await lobbyApi.GetLobbyInfoAsync(
                    NetHathoraConfig.HathoraCoreOpts.AppId,
                    roomId);
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
        
        [ItemCanBeNull]
        public async Task<List<Lobby>> ClientListPublicLobbiesAsync()
        {
            List<Lobby> lobbies;
            try
            {
                lobbies = await lobbyApi.ListActivePublicLobbiesAsync(
                    NetHathoraConfig.HathoraCoreOpts.AppId,
                    NetHathoraConfig.HathoraLobbyRoomOpts.HathoraRegion);
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

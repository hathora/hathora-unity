// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Client.Models;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.Net.Server;
using JetBrains.Annotations;
using UnityEngine;

namespace Hathora.Scripts.Net.Client.ApiWrapper
{
    /// <summary>
    /// * Call Init() to pass UserConfig/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientLobbyApi : NetHathoraApiBase
    {
        private LobbyV2Api lobbyApi;

        public override void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _netSession)
        {
            Debug.Log("[NetHathoraClientLobbyApi] Initializing API...");
            base.Init(_hathoraSdkConfig, _hathoraServerConfig, _netSession);
            this.lobbyApi = new LobbyV2Api(_hathoraSdkConfig);
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
            CreateLobbyRequest request = new CreateLobbyRequest(
                lobbyVisibility, 
                lobbyInitConfig, 
                hathoraServerConfig.Region);

            Lobby lobby;
            try
            {
                lobby = await lobbyApi.CreateLobbyAsync(
                    hathoraServerConfig.AppId,
                    NetSession.PlayerAuthToken, // Player token; not dev
                    request);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClientLobbyApi]**ERR @ ClientCreateLobbyAsync (CreateLobbyAsync): {e.Message}");
                await Task.FromException<Exception>(e);
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
        /// <returns></returns>
        public async Task<Lobby> ClientGetLobbyInfoAsync(string roomId)
        {
            Lobby lobby;
            try
            {
                lobby = await lobbyApi.GetLobbyInfoAsync(
                    hathoraServerConfig.AppId,
                    roomId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClientLobbyApi]**ERR @ ClientGetLobbyInfoAsync (GetLobbyInfoAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientLobbyApi] ClientGetLobbyInfoAsync => roomId: {lobby.RoomId}");
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
                    hathoraServerConfig.AppId,
                    hathoraServerConfig.Region);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClientLobbyApi]**ERR @ ClientListPublicLobbiesAsync " +
                    $"(ListActivePublicLobbiesAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientLobbyApi] ClientListPublicLobbiesAsync => " +
                $"numLobbiesFound: {lobbies?.Count ?? 0}");
            
            return lobbies;
        }
        #endregion // Client Lobby Async Hathora SDK Calls
    }
}

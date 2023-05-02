// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Client.Models;
using Hathora.Net.Server;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// * Call Init() to pass config/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientLobby : NetHathoraApiBase
    {
        private LobbyV2Api lobbyApi;

        public override void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _playerSession)
        {
            base.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
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
            if (!base.IsClient)
                return null; // fail

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
                    PlayerSession.PlayerAuthToken, // Player token; not dev
                    request);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClientLobby]**ERR @ ClientCreateLobbyAsync (CreateLobbyAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientLobby] ClientCreateLobbyAsync => roomId: {lobby.RoomId}");
            PlayerSession.Lobby = lobby;
            
            return lobby;
        }
         
        public async Task<Lobby> ClientGetJoinInfoAsync(string roomId)
        {
            if (!base.IsClient)
                return null; // fail

            Lobby lobby;
            try
            {
                lobby = await lobbyApi.GetLobbyInfoAsync(
                    hathoraServerConfig.AppId,
                    roomId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClientLobby]**ERR @ ClientGetJoinInfoAsync (GetLobbyInfoAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientLobby] ClientGetJoinInfoAsync => roomId: {lobby.RoomId}");
            PlayerSession.Lobby = lobby;
            
            return lobby;
        }
        
        public async Task<Lobby> ClientGetConnectionInfo(string roomId)
        {
            if (!base.IsClient)
                return null; // fail

            Lobby lobby;
            try
            {
                lobby = await lobbyApi.GetLobbyInfoAsync(
                    hathoraServerConfig.AppId,
                    roomId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClientLobby]**ERR @ ClientGetJoinInfoAsync (GetLobbyInfoAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientLobby] ClientGetJoinInfoAsync => roomId: {lobby.RoomId}");
            PlayerSession.Lobby = lobby;
            
            return lobby;
        }
        #endregion // Client Lobby Async Hathora SDK Calls
    }
}

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
    /// * Does not handle UI - Sub to the callback events.
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


        #region Event Delegates
        /// <summary>=> Lobby // TODO: This doesn't sync 1:1 with Fishnet</summary>
        public event EventHandler<Lobby> CreateLobbyComplete;
        #endregion // Event Delegates

        
        #region Client Lobby Async Hathora SDK Calls
        /// <summary>
        /// 
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
        
        public async Task<Lobby> ClientJoinLobbyAsync(string roomId)
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
                Debug.LogError($"[NetHathoraClientLobby]**ERR @ ClientJoinLobbyAsync (GetLobbyInfoAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null; // fail
            }

            Debug.Log($"[NetHathoraClientLobby] ClientJoinLobbyAsync => roomId: {lobby.RoomId}");
            PlayerSession.Lobby = lobby;
            
            return lobby;
        }
        #endregion // Client Lobby Async Hathora SDK Calls
    }
}

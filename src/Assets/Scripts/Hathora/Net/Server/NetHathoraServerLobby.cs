// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Server.Models;
using UnityEngine;

namespace Hathora.Net.Server
{
    /// <summary>
    /// Call base.Init() to pass dev tokens, etc.
    /// </summary>
    public class NetHathoraServerLobby : HathoraNetServerApiBase
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

        
        #region Server Lobby Async Hathora SDK Calls
        public async Task ServerCreateLobbyAsync(CreateLobbyRequest.VisibilityEnum lobbyVisibility)
        {
            if (!base.IsServer)
                return;

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
                    PlayerSession.AuthToken, // Player token; not dev
                    request);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraServerLobby]**ERR @ ServerCreateLobbyAsync (CreateLobbyAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                onCreateLobbyFail();
                return;
            }

            Debug.Log($"[NetHathoraServerLobby] ServerCreateLobbyAsync => roomId: {lobby.RoomId}");

            if (lobby != null)
                onServerCreateLobbySuccess(lobby);
        }
        #endregion // Server Lobby Async Hathora SDK Calls
        
        
        #region Success Callbacks
        private void onServerCreateLobbySuccess(Lobby lobby)
        {
            Debug.LogWarning("[NetHathoraServerLobby] TODO @ " +
                "onServerCreateLobbySuccess: Cache lobby @ session");
            
            // PlayerSession.Lobby = lobby; // TODO
            CreateLobbyComplete?.Invoke(this, lobby);
        }
        #endregion // Success Callbacks
        
        
        #region Fail Callbacks
        private void onCreateLobbyFail()
        {
            const Lobby lobby = null;
            CreateLobbyComplete?.Invoke(this, lobby);
        }
        #endregion // Fail Callbacks
    }
}

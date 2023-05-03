// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Common;
using Hathora.Net.Client.ApiWrapper;
using Hathora.Net.Client.Models;
using Hathora.Net.Server;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// This spawns BEFORE the player, or even connected to the network.
    /// This is the entry point to call Hathora SDK: Auth, lobby, rooms, etc.
    /// To add scripts, add to the `ClientApis` serialized field.
    /// </summary>
    public class NetHathoraClient : MonoBehaviour
    {
        [SerializeField]
        private HathoraServerConfig hathoraServerConfig;
        
        [SerializeField]
        public ClientApiContainer ClientApis;

        [SerializeField]
        private NetSession netSession;
        
        public static NetHathoraClient Singleton { get; private set; }

        private Configuration hathoraSdkConfig;

        
        #region Init
        private void Awake()
        {
            setSingleton();
        }

        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[NetHathoraClient]**ERR @ setSingleton: Destroying dupe");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }
        
        private void Start()
        {
            this.hathoraSdkConfig = new Configuration();
            ClientApis.InitAll(hathoraSdkConfig, hathoraServerConfig, netSession);
        }
        #endregion // Init
        
        
        #region Interactions from UI
        public async Task AuthLoginAsync()
        {
            AuthResult result = null;
            try
            {
                result = await ClientApis.authApiApi.ClientAuthAsync();
            }
            catch
            {
                OnAuthComplete(isSuccess:false);
                return;
            }
           
            OnAuthComplete(result.IsSuccess);
        }

        public async Task CreateLobbyAsync(CreateLobbyRequest.VisibilityEnum visibility = 
            CreateLobbyRequest.VisibilityEnum.Public)
        {
            Lobby lobby = null;
            try
            {
                lobby = await ClientApis.lobbyApiApi.ClientCreateLobbyAsync(visibility);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                OnCreateOrJoinLobbyCompleteAsync(null);
                return;
            }
            
            OnCreateOrJoinLobbyCompleteAsync(lobby);
        }

        /// <summary>
        /// The player pressed ENTER || unfocused the roomId input.
        /// </summary>
        public async Task GetLobbyInfoAsync(string roomId)
        {
            Lobby lobby = null;
            try
            {
                lobby = await ClientApis.lobbyApiApi.ClientGetJoinInfoAsync(roomId);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                OnCreateOrJoinLobbyCompleteAsync(null);
                return;
            }

            OnCreateOrJoinLobbyCompleteAsync(lobby);
        }
        
        /// <summary>Public lobbies only.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ViewPublicLobbies()
        {
            List<Lobby> lobbies = null;
            try
            {
                lobbies = await ClientApis.lobbyApiApi.ClientListPublicLobbiesAsync();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                throw new NotImplementedException("TODO: Get lobbies err handling UI");
            }
            
            OnViewPublicLobbiesComplete(lobbies);
        }
        #endregion // Interactions from UI
        
        
        #region Callbacks
        private void OnAuthComplete(bool isSuccess)
        {
            if (!isSuccess)
            {
                NetUI.Singleton.OnAuthFailed();
                return;
            }
            
            NetUI.Singleton.OnAuthedLoggedIn();
        }

        private void OnViewPublicLobbiesComplete(List<Lobby> lobbies)
        {
            int numLobbiesFound = lobbies?.Count ?? 0;
            Debug.Log($"[NetHathoraPlayer] OnViewPublicLobbiesComplete: # Lobbies found: {numLobbiesFound}");

            if (lobbies == null || numLobbiesFound == 0)
            {
                throw new NotImplementedException("TODO: !Lobbies handling");
            }
            
            List<Lobby> sortedLobbies = lobbies.OrderBy(lobby => lobby.CreatedAt).ToList();
            NetUI.Singleton.OnViewLobbies(sortedLobbies);
        }
        
        /// <summary>
        /// On success, we'll poll for ActiveConnectionInfo.
        /// </summary>
        /// <param name="lobby"></param>
        private async Task OnCreateOrJoinLobbyCompleteAsync(Lobby lobby)
        {
            if (string.IsNullOrEmpty(lobby?.RoomId))
            {
                NetUI.Singleton.OnCreatedOrJoinedLobbyFail();
                return;
            }

            NetUI.Singleton.OnCreatedOrJoinedLobby(lobby.RoomId);
            
            // Poll for ActiveConnectionInfo
            ActiveConnectionInfo activeConnectionInfo;
            try
            {
                activeConnectionInfo = await ClientApis.roomApiApi.ClientGetConnectionInfoAsync(lobby.RoomId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClient] OnCreateOrJoinLobbyCompleteAsync: {e.Message}");
                NetUI.Singleton.OnCreatedOrJoinedLobbyFail();
                await Task.FromException<Exception>(e);
                return; // fail
            }
            
            // Success
            Debug.Log("Success @ OnCreateOrJoinLobbyCompleteAsync => activeConnectionInfo " +
                $"// ConnectionInfo: {activeConnectionInfo.Host}:{activeConnectionInfo.Port} ({activeConnectionInfo.TransportType}) " +
                $"// TODO post-events");
        }
        #endregion // Callbacks
    }
}

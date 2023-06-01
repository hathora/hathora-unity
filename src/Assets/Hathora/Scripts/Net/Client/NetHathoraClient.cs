// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Client.Models;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Scripts.Net.Client
{
    /// <summary>
    /// This spawns BEFORE the player, or even connected to the network.
    /// This is the entry point to call Hathora SDK: Auth, lobby, rooms, etc.
    /// To add scripts, add to the `ClientApis` serialized field.
    /// </summary>
    public class NetHathoraClient : MonoBehaviour
    {
        [SerializeField]
        private NetHathoraConfig netHathoraConfig;
        
        [SerializeField]
        private NetSession netSession;
        
        [SerializeField]
        public ClientApiContainer ClientApis;
     
        
        public static NetHathoraClient Singleton { get; private set; }

        private Configuration hathoraSdkConfig;

        
        #region Init
        private void Awake()
        {
            setSingleton();
            assertUsingValidNetConfig();
        }

        private void assertUsingValidNetConfig()
        {
            const string baseName = nameof(netHathoraConfig);
            string configName = netHathoraConfig.name;
            bool isTemplate = configName != $"{baseName}.template";
            
            if (!isTemplate)
                return;

            NetUI.Singleton.SetInvalidConfig(configName);
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
            ClientApis.InitAll(
                netHathoraConfig, 
                netSession, 
                _hathoraSdkConfig: null); // Base will create this
        }
        #endregion // Init
        
        
        #region Interactions from UI
        /// <summary>
        /// Auths anonymously => Creates new netSession.
        /// </summary>
        public async Task AuthLoginAsync()
        {
            AuthResult result = null;
            try
            {
                result = await ClientApis.clientAuthApi.ClientAuthAsync();
            }
            catch
            {
                OnAuthLoginComplete(isSuccess:false);
                return;
            }
           
            OnAuthLoginComplete(result.IsSuccess);
        }

        /// <summary>
        /// Creates lobby => caches Lobby info @ netSession
        /// </summary>
        /// <param name="visibility"></param>
        public async Task CreateLobbyAsync(CreateLobbyRequest.VisibilityEnum visibility = 
            CreateLobbyRequest.VisibilityEnum.Public)
        {
            Lobby lobby = null;
            try
            {
                lobby = await ClientApis.clientLobbyApi.ClientCreateLobbyAsync(visibility);
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
        /// Gets lobby info, if you arleady know the roomId.
        /// (!) Creating a lobby will automatically return the lobbyInfo (along with the roomId).
        /// </summary>
        public async Task GetLobbyInfoAsync(string roomId)
        {
            Lobby lobby = null;
            try
            {
                lobby = await ClientApis.clientLobbyApi.ClientGetLobbyInfoAsync(roomId);
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
        public async Task ViewPublicLobbies()
        {
            List<Lobby> lobbies = null;
            try
            {
                lobbies = await ClientApis.clientLobbyApi.ClientListPublicLobbiesAsync();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                throw new NotImplementedException("TODO: Get lobbies err handling UI");
            }
            
            OnViewPublicLobbiesComplete(lobbies);
        }
        
        /// <summary>
        /// Gets ip:port (+transport type) info so we can connect the Client via the selected transport (eg: Fishnet).
        /// AKA "GetServerInfo" (from UI). Polls until status is `Active`: May take a bit!
        /// </summary>
        public async Task GetActiveConnectionInfo(string roomId)
        {
            ConnectionInfoV2 connectionInfo;
            try
            {
                connectionInfo = await ClientApis.clientRoomApi.ClientGetConnectionInfoAsync(roomId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClient] OnCreateOrJoinLobbyCompleteAsync: {e.Message}");
                NetUI.Singleton.OnGetServerInfoFail();
                return; // fail
            }
            
            // Success
            OnGetActiveConnectionInfoComplete(connectionInfo);
        }
        
        /// <summary>AKA OnGetServerInfoSuccess</summary>
        private void OnGetActiveConnectionInfoComplete(ConnectionInfoV2 connectionInfo)
        {
            if (string.IsNullOrEmpty(connectionInfo?.ExposedPort?.Host))
            {
                NetUI.Singleton.OnGetServerInfoFail();
                return;
            }
            
            NetUI.Singleton.OnGetServerInfoSuccess(connectionInfo);
        }
        #endregion // Interactions from UI
        
        
        #region Callbacks
        private void OnAuthLoginComplete(bool isSuccess)
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
        /// On success, most users will want to call GetActiveConnectionInfo().
        /// </summary>
        /// <param name="lobby"></param>
        private void OnCreateOrJoinLobbyCompleteAsync(Lobby lobby)
        {
            if (string.IsNullOrEmpty(lobby?.RoomId))
            {
                NetUI.Singleton.OnCreatedOrJoinedLobbyFail();
                return;
            }

            NetUI.Singleton.OnCreatedOrJoinedLobby(lobby.RoomId);
        }
        #endregion // Callbacks
    }
}

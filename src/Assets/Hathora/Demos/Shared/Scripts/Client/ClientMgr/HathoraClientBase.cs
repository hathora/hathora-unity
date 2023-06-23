// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Client;
using Hathora.Core.Scripts.Runtime.Client.Config;
using Hathora.Core.Scripts.Runtime.Client.Models;
using Hathora.Core.Scripts.Runtime.Common.Extensions;
using Hathora.Demos.Shared.Scripts.Client.Models;
using UnityEngine;

namespace Hathora.Demos.Shared.Scripts.Client.ClientMgr
{
    /// <summary>
    /// - This spawns BEFORE the player, or even connected to the network.
    /// - This is the entry point to call Hathora SDK: Auth, lobby, rooms, etc.
    /// - To add API scripts: Add to the `ClientApis` serialized field.
    /// </summary>
    public abstract class HathoraClientBase : MonoBehaviour
    {
        /// <summary>Updates this on state changes</summary>
        protected bool IsConnecting;
        
        [Header("(!) Get from Hathora dir; see hover tooltip")]
        [SerializeField, Tooltip("AppId should parity HathoraServerConfig (see top menu Hathora/Configuration")]
        protected HathoraClientConfig HathoraClientConfig;
        
        [Header("Session, APIs")]
        [SerializeField]
        protected HathoraClientSession HathoraClientSession;
        
        [SerializeField]
        protected ClientApiContainer ClientApis;
     
        
        // public static Hathora{X}Client Singleton { get; private set; } // TODO: Implement me in child

        
        #region Init
        private void Awake() => OnAwake();

        protected virtual void OnAwake()
        {
            AssertUsingValidNetConfig();
            // setSingleton(); // TODO: Override me + implement in child class 
        }

        public virtual void AssertUsingValidNetConfig()
        {
            // Are we using any Client Config at all?
            if (HathoraClientConfig == null || !HathoraClientConfig.HasAppId && HathoraFishNetUI.Singleton != null)
                HathoraFishNetUI.Singleton.SetInvalidConfig(HathoraClientConfig);
        }

        // // TODO: implement me in child class:
        // protected virtual void setSingleton()
        // {
        //     if (Singleton != null)
        //     {
        //         Debug.LogError("[HathoraClientBase]**ERR @ setSingleton: Destroying dupe");
        //         Destroy(gameObject);
        //         return;
        //     }
        //     
        //     Singleton = this;
        // }
        
        private void Start() => OnStart();

        protected virtual void OnStart()
        {
            ClientApis.InitAll(
                HathoraClientConfig, 
                _hathoraSdkConfig: null); // Base will create this
            
            // TODO: Override + sub to any callbacks events, such as connection state updates 
        }
        #endregion // Init
        
        
        #region Interactions from UI

        /// <summary>If !success, call OnConnectFailed().</summary>
        /// <returns>isValid</returns>
        protected bool ValidateServerConfigConnectionInfo()
        {
            // Validate host:port connection info
            if (!HathoraClientSession.CheckIsValidServerConnectionInfo())
            {
                OnConnectFailed("Invalid ServerConnectionInfo");
                return false; // !success
            }
            
            return true; // success
        }

        protected virtual void OnConnectFailed(string _friendlyReason)
        {
            IsConnecting = false;
            HathoraFishNetUI.Singleton.OnJoinLobbyFailed(_friendlyReason);
        }

        protected virtual void OnConnectSuccess()
        {
            Debug.Log("[HathoraClientBase] OnConnectSuccess");
            
            IsConnecting = false;
            HathoraFishNetUI.Singleton.OnJoinLobbySuccess();
        }

        /// <summary>
        /// Auths anonymously => Creates new hathoraClientSession.
        /// </summary>
        public async Task AuthLoginAsync()
        {
            AuthResult result;
            try
            {
                result = await ClientApis.clientAuthApi.ClientAuthAsync();
            }
            catch
            {
                OnAuthLoginComplete(_isSuccess:false);
                return;
            }
           
            HathoraClientSession.InitNetSession(result.PlayerAuthToken);
            OnAuthLoginComplete(result.IsSuccess);
        }

        /// <summary>
        /// Creates lobby => caches Lobby info @ hathoraClientSession
        /// </summary>
        /// <param name="_region"></param>
        /// <param name="_visibility"></param>
        public async Task CreateLobbyAsync(
            Region _region,
            CreateLobbyRequest.VisibilityEnum _visibility = CreateLobbyRequest.VisibilityEnum.Public)
        {
            Lobby lobby;
            try
            {
                lobby = await ClientApis.clientLobbyApi.ClientCreateLobbyAsync(
                    HathoraClientSession.PlayerAuthToken,
                    _visibility,
                    _region);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                OnCreateOrJoinLobbyCompleteAsync(null);
                return;
            }
            
            HathoraClientSession.Lobby = lobby;
            OnCreateOrJoinLobbyCompleteAsync(lobby);
        }

        /// <summary>
        /// Gets lobby info, if you arleady know the roomId.
        /// (!) Creating a lobby will automatically return the lobbyInfo (along with the roomId).
        /// </summary>
        public async Task GetLobbyInfoAsync(string _roomId)
        {
            Lobby lobby;
            try
            {
                lobby = await ClientApis.clientLobbyApi.ClientGetLobbyInfoAsync(_roomId);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                OnCreateOrJoinLobbyCompleteAsync(null);
                return;
            }

            HathoraClientSession.Lobby = lobby;
            OnCreateOrJoinLobbyCompleteAsync(lobby);
        }
        
        /// <summary>Public lobbies only.</summary>
        /// <param name="_region">
        /// TODO (to confirm): null region returns *all* region lobbies?
        /// </param>
        public async Task ViewPublicLobbies(Region? _region = null)
        {
            List<Lobby> lobbies;
            try
            {
                lobbies = await ClientApis.clientLobbyApi.ClientListPublicLobbiesAsync();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                throw new NotImplementedException("TODO: Get lobbies err handling UI");
            }

            HathoraClientSession.Lobbies = lobbies;
            OnViewPublicLobbiesComplete(lobbies);
        }
        
        /// <summary>
        /// Gets ip:port (+transport type) info so we can connect the Client via the selected transport (eg: Fishnet).
        /// AKA "GetServerInfo" (from UI). Polls until status is `Active`: May take a bit!
        /// </summary>
        public async Task GetActiveConnectionInfo(string _roomId)
        {
            ConnectionInfoV2 connectionInfo;
            try
            {
                connectionInfo = await ClientApis.clientRoomApi.ClientGetConnectionInfoAsync(_roomId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraClientBase] OnCreateOrJoinLobbyCompleteAsync: {e.Message}");
                HathoraFishNetUI.Singleton.OnGetServerInfoFail();
                return; // fail
            }
            
            HathoraClientSession.ServerConnectionInfo = connectionInfo;
            OnGetActiveConnectionInfoComplete(connectionInfo);
        }
        #endregion // Interactions from UI
        
        
        #region Callbacks        
        /// <summary>AKA OnGetServerInfoSuccess</summary>
        protected virtual void OnGetActiveConnectionInfoComplete(ConnectionInfoV2 _connectionInfo)
        {
            if (string.IsNullOrEmpty(_connectionInfo?.ExposedPort?.Host))
            {
                HathoraFishNetUI.Singleton.OnGetServerInfoFail();
                return;
            }
            
            HathoraFishNetUI.Singleton.OnGetServerInfoSuccess(_connectionInfo);
        }
        
        protected virtual void OnAuthLoginComplete(bool _isSuccess)
        {
            if (!_isSuccess)
            {
                HathoraFishNetUI.Singleton.OnAuthFailed();
                return;
            }

            HathoraFishNetUI.Singleton.OnAuthedLoggedIn();
        }

        protected virtual void OnViewPublicLobbiesComplete(List<Lobby> _lobbies)
        {
            int numLobbiesFound = _lobbies?.Count ?? 0;
            Debug.Log($"[NetHathoraPlayer] OnViewPublicLobbiesComplete: # Lobbies found: {numLobbiesFound}");

            if (_lobbies == null || numLobbiesFound == 0)
            {
                throw new NotImplementedException("TODO: !Lobbies handling");
            }
            
            List<Lobby> sortedLobbies = _lobbies.OrderBy(lobby => lobby.CreatedAt).ToList();
            HathoraFishNetUI.Singleton.OnViewLobbies(sortedLobbies);
        }
        
        /// <summary>
        /// On success, most users will want to call GetActiveConnectionInfo().
        /// </summary>
        /// <param name="_lobby"></param>
        protected virtual void OnCreateOrJoinLobbyCompleteAsync(Lobby _lobby)
        {
            if (string.IsNullOrEmpty(_lobby?.RoomId))
            {
                HathoraFishNetUI.Singleton.OnCreatedOrJoinedLobbyFail();
                return;
            }

            string friendlyRegion = _lobby.Region.ToString().SplitPascalCase();
            HathoraFishNetUI.Singleton.OnCreatedOrJoinedLobby(_lobby.RoomId, friendlyRegion);
        }
        #endregion // Callbacks
    }
}

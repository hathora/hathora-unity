// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Client;
using Hathora.Core.Scripts.Runtime.Client.Config;
using Hathora.Core.Scripts.Runtime.Client.Models;
using Hathora.Core.Scripts.Runtime.Common.Extensions;
using Hathora.Demos.Shared.Scripts.Client.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Demos.Shared.Scripts.Client.ClientMgr
{
    /// <summary>
    /// - This spawns BEFORE the player, or even connected to the network.
    /// - This is the entry point to call Hathora SDK: Auth, lobby, rooms, etc.
    /// - To add API scripts: Add to the `ClientApis` serialized field.
    /// </summary>
    public abstract class HathoraClientMgrBase : MonoBehaviour
    {
        #region Serialized Fields
        [FormerlySerializedAs("HathoraClientConfig")]
        [Header("(!) Get from Hathora dir; see hover tooltip")]
        [SerializeField, Tooltip("AppId should parity HathoraServerConfig (see top menu Hathora/Configuration")]
        private HathoraClientConfig hathoraClientConfig;
        protected HathoraClientConfig HathoraClientConfig => hathoraClientConfig;

        [FormerlySerializedAs("HathoraClientSession")]
        [Header("Session, APIs")]
        [SerializeField]
        private HathoraClientSession hathoraClientSession;
        public HathoraClientSession HathoraClientSession => hathoraClientSession;
        
        [FormerlySerializedAs("ClientApis")]
        [SerializeField]
        private ClientApiContainer clientApis;
        protected ClientApiContainer ClientApis => clientApis;
        #endregion // Serialized Fields

        private bool hasSdkDemoUi => ClientMgrDemoUi != null;

        
        // public static Hathora{X}Client Singleton { get; private set; } // TODO: Implement me in child
        
        /// <summary>Updates this on state changes</summary>
        protected bool IsConnecting { get; set; }
        
        private HathoraClientMgrDemoUi ClientMgrDemoUi { get; set; }

        
        #region Init
        private void Awake() => OnAwake();
        private void Start() => OnStart();
        
        protected virtual void OnAwake()
        {
            SetSingleton();
        }

        /// <summary>
        /// You want other classes to easily be able to access your ClientMgr
        /// </summary>
        protected abstract void SetSingleton();

        /// <summary>Override OnStart and call this before anything.</summary>
        /// <param name="_clientMgrDemoUi"></param>
        protected virtual void InitOnStart(HathoraClientMgrDemoUi _clientMgrDemoUi)
        {
            ClientMgrDemoUi = _clientMgrDemoUi;
        }

        protected virtual void OnStart()
        {
            validateReqs();
            initApis(_hathoraSdkConfig: null); // Base will create this
        }
        
        /// <summary>
        /// Init all Client API wrappers. Uses serialized HathoraClientConfig
        /// </summary>
        /// <param name="_hathoraSdkConfig">We'll automatically create this, if empty</param>
        private void initApis(Configuration _hathoraSdkConfig = null)
        {
            if (clientApis.ClientAuthApi != null)
                clientApis.ClientAuthApi.Init(hathoraClientConfig, _hathoraSdkConfig);
            
            if (clientApis.ClientLobbyApi != null)
                clientApis.ClientLobbyApi.Init(hathoraClientConfig, _hathoraSdkConfig);

            if (clientApis.ClientRoomApi != null)
                clientApis.ClientRoomApi.Init(hathoraClientConfig, _hathoraSdkConfig);
        }

        public virtual void validateReqs()
        {
            // Are we using any Client Config at all?
            bool hasConfig = hathoraClientConfig != null;
            bool hasAppId = hathoraClientConfig.HasAppId;
            bool hasNoAppIdButHasUiInstance = !hasAppId && hasSdkDemoUi;
            
            if (!hasConfig || hasNoAppIdButHasUiInstance)
                ClientMgrDemoUi.SetInvalidConfig(hathoraClientConfig);
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
        #endregion // Init
        
        
        #region Interactions from UI
        
        
        #region Interactions from UI -> Required Overrides
        public abstract Task<bool> ConnectAsClient();
        
        /// <summary>
        /// Starts a NetworkManager local Server.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// TODO: Mv to HathoraServerMgr
        /// </summary>
        public abstract Task StartServer();

        /// <summary>
        /// Stops a NetworkManager local Server.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// TODO: Mv to HathoraServerMgr
        /// </summary>
        public abstract Task StopServer();

        /// <summary>Starts a NetworkManager Client</summary>
        /// <param name="_hostPort">host:port provided by Hathora</param>
        public abstract Task StartClient(string _hostPort = null);
        
        /// <summary>Stops a NetworkManager Client</summary>
        /// <returns></returns>
        public abstract Task StopClient(); 
        
        /// <summary>Starts a NetworkManager Host (Server+Client). TODO: Create a `HathoraCommonMgr` and mv this</summary>
        public abstract Task StartHost();

        /// <summary>Stops a NetworkManager Host (Server+Client). TODO: Create a `HathoraCommonMgr` and mv this</summary>
        public abstract Task StopHost();
        #endregion // Interactions from UI -> Required Overrides
       
        
        #region Interactions from UI -> Optional overrides
        /// <summary>If !success, call OnConnectFailed().</summary>
        /// <returns>isValid</returns>
        protected virtual bool ValidateServerConfigConnectionInfo()
        {
            // Validate host:port connection info
            if (!hathoraClientSession.CheckIsValidServerConnectionInfo())
            {
                OnConnectFailed("Invalid ServerConnectionInfo");
                return false; // !success
            }
            
            return true; // success
        }

        /// <summary>Sets `IsConnecting` + logs ip:port (transport).</summary>
        /// <param name="_transportName"></param>
        protected virtual void SetConnectingState(string _transportName)
        {
            IsConnecting = true;

            Debug.Log("[HathoraClientBase.SetConnectingState] Connecting to: " + 
                $"{hathoraClientSession.GetServerInfoIpPort()} via " +
                $"NetworkManager.{_transportName} transport");
        }
        #endregion // Interactions from UI -> Optional overrides

        
        /// <summary>
        /// Auths anonymously => Creates new hathoraClientSession.
        /// </summary>
        public async Task<AuthResult> AuthLoginAsync(CancellationToken _cancelToken = default)
        {
            AuthResult result;
            try
            {
                result = await clientApis.ClientAuthApi.ClientAuthAsync(_cancelToken);
            }
            catch
            {
                OnAuthLoginComplete(_isSuccess:false);
                return null;
            }
           
            hathoraClientSession.InitNetSession(result.PlayerAuthToken);
            OnAuthLoginComplete(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// Creates lobby => caches Lobby info @ hathoraClientSession
        /// </summary>
        /// <param name="_region"></param>
        /// <param name="_visibility"></param>
        /// <param name="_initConfigJsonStr"></param>
        /// <param name="_cancelToken"></param>
        public async Task<Lobby> CreateLobbyAsync(
            Region _region,
            CreateLobbyRequest.VisibilityEnum _visibility = CreateLobbyRequest.VisibilityEnum.Public,
            string _initConfigJsonStr = "{}",
            string _roomId = null,
            CancellationToken _cancelToken = default)
        {
            Lobby lobby;
            try
            {
                lobby = await clientApis.ClientLobbyApi.ClientCreateLobbyAsync(
                    hathoraClientSession.PlayerAuthToken,
                    _visibility,
                    _region,
                    _initConfigJsonStr,
                    _roomId,
                    _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                OnCreateOrJoinLobbyCompleteAsync(null);
                return null;
            }
            
            hathoraClientSession.Lobby = lobby;
            OnCreateOrJoinLobbyCompleteAsync(lobby);

            return lobby;
        }

        /// <summary>
        /// Gets lobby info, if you arleady know the roomId.
        /// (!) Creating a lobby will automatically return the lobbyInfo (along with the roomId).
        /// </summary>
        public async Task<Lobby> GetLobbyInfoAsync(
            string _roomId, 
            CancellationToken _cancelToken = default)
        {
            Lobby lobby;
            try
            {
                lobby = await clientApis.ClientLobbyApi.ClientGetLobbyInfoAsync(
                    _roomId,
                    _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                OnCreateOrJoinLobbyCompleteAsync(null);
                return null;
            }

            hathoraClientSession.Lobby = lobby;
            OnCreateOrJoinLobbyCompleteAsync(lobby);

            return lobby;
        }

        /// <summary>Public lobbies only.</summary>
        /// <param name="_region">
        /// TODO (to confirm): null region returns *all* region lobbies?
        /// </param>
        /// <param name="_cancelToken"></param>
        public async Task<List<Lobby>> ViewPublicLobbies(
            Region? _region = null,
            CancellationToken _cancelToken = default)
        {
            List<Lobby> lobbies;
            try
            {
                lobbies = await clientApis.ClientLobbyApi.ClientListPublicLobbiesAsync(
                    _region,
                    _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                throw new NotImplementedException("TODO: Get lobbies err handling UI");
            }

            hathoraClientSession.Lobbies = lobbies;
            OnViewPublicLobbiesComplete(lobbies);

            return lobbies;
        }
        
        /// <summary>
        /// Gets ip:port (+transport type) info so we can connect the Client via the selected transport (eg: Fishnet).
        /// AKA "GetServerInfo" (from UI). Polls until status is `Active`: May take a bit!
        /// </summary>
        public async Task<ConnectionInfoV2> GetActiveConnectionInfo(
            string _roomId, 
            CancellationToken _cancelToken = default)
        {
            ConnectionInfoV2 connectionInfo;
            try
            {
                connectionInfo = await clientApis.ClientRoomApi.ClientGetConnectionInfoAsync(
                    _roomId, 
                    _cancelToken: _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraClientBase] OnCreateOrJoinLobbyCompleteAsync: {e.Message}");
                if (hasSdkDemoUi)
                    ClientMgrDemoUi.OnGetServerInfoFail();
                return null; // fail
            }
            
            hathoraClientSession.ServerConnectionInfo = connectionInfo;
            OnGetActiveConnectionInfoComplete(connectionInfo);

            return connectionInfo;
        }
        #endregion // Interactions from UI
        
        
        #region Callbacks
        protected virtual void OnConnectFailed(string _friendlyReason)
        {
            IsConnecting = false;
            
            if (hasSdkDemoUi)
                ClientMgrDemoUi.OnJoinLobbyFailed(_friendlyReason);
        }
        
        protected virtual void OnConnectSuccess()
        {
            IsConnecting = false;
            
            if (hasSdkDemoUi)
                ClientMgrDemoUi.OnJoinLobbyConnectSuccess();
        }
        
        protected virtual void OnGetActiveConnectionInfoFail()
        {
            if (hasSdkDemoUi)
                ClientMgrDemoUi.OnGetServerInfoFail();
        }
        
        /// <summary>AKA OnGetServerInfoSuccess - mostly UI</summary>
        protected virtual void OnGetActiveConnectionInfoComplete(ConnectionInfoV2 _connectionInfo)
        {
            if (ClientMgrDemoUi == null)
                return;

            if (string.IsNullOrEmpty(_connectionInfo?.ExposedPort?.Host))
            {
                ClientMgrDemoUi.OnGetServerInfoFail();
                return;
            }
            
            ClientMgrDemoUi.OnGetServerInfoSuccess(_connectionInfo);
        }
        
        protected virtual void OnAuthLoginComplete(bool _isSuccess)
        {
            if (ClientMgrDemoUi == null)
                return;

            if (!_isSuccess)
            {
                ClientMgrDemoUi.OnAuthFailed();
                return;
            }

            ClientMgrDemoUi.OnAuthedLoggedIn();
        }

        protected virtual void OnViewPublicLobbiesComplete(List<Lobby> _lobbies)
        {
            int numLobbiesFound = _lobbies?.Count ?? 0;
            Debug.Log("[NetHathoraPlayer] OnViewPublicLobbiesComplete: " +
                $"# Lobbies found: {numLobbiesFound}");

            // UI >>
            if (ClientMgrDemoUi == null)
                return;

            if (_lobbies == null || numLobbiesFound == 0)
                throw new NotImplementedException("TODO: !Lobbies handling");

            List<Lobby> sortedByNewestToOldest = _lobbies.OrderByDescending(lobby => 
                lobby.CreatedAt).ToList();
            
            ClientMgrDemoUi.OnViewLobbies(sortedByNewestToOldest);
        }
        
        /// <summary>
        /// On success, most users will want to call GetActiveConnectionInfo().
        /// </summary>
        /// <param name="_lobby"></param>
        protected virtual void OnCreateOrJoinLobbyCompleteAsync(Lobby _lobby)
        {
            if (ClientMgrDemoUi == null)
                return;

                // UI >>
            if (string.IsNullOrEmpty(_lobby?.RoomId))
            {
                ClientMgrDemoUi.OnCreatedOrJoinedLobbyFail();
                return;
            }
            
            // Success >> We may not have a UI
            if (ClientMgrDemoUi == null)
                return;

            string friendlyRegion = _lobby.Region.ToString().SplitPascalCase();
            ClientMgrDemoUi.OnCreatedOrJoinedLobby(
                _lobby.RoomId, 
                friendlyRegion);
        }
        #endregion // Callbacks
        
        
        #region Utils
        /// <summary>
        /// This was likely passed in from the UI to override the default NetworkManager (often from Standalone Client).
        /// Eg: "1.proxy.hathora.dev:12345" -> "1.proxy.hathora.dev", 12345
        /// </summary>
        /// <param name="_hostPort"></param>
        /// <returns></returns>
        protected static (string hostNameOrIp, ushort port) SplitPortFromHostOrIp(string _hostPort)
        {
            if (string.IsNullOrEmpty(_hostPort))
                return default;
            
            string[] hostPortArr = _hostPort.Split(':');
            string hostNameOrIp = hostPortArr[0];
            ushort port = ushort.Parse(hostPortArr[1]);
            
            return (hostNameOrIp, port);
        }
        #endregion // Utils
    }
}

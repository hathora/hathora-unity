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
using Hathora.Demos.Shared.Scripts.Client.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Hathora.Demos.Shared.Scripts.Client.ClientMgr
{
    /// <summary>
    /// - This spawns BEFORE the player, or even connected to the network.
    /// - This is the entry point to call Hathora SDK: Auth, lobby, rooms, etc.
    /// - Opposed to the SDK itself, this gracefully wraps around it with callbacks + events.
    /// 
    /// - Available Events to subscribe to:
    ///     * OnAuthLoginDoneEvent
    ///     * OnClientStartingEvent
    ///     * OnStartClientFailEvent
    ///     * OnClientStoppedEvent
    ///     * OnGetActiveConnectionInfoFailEvent
    ///     * OnGetActiveConnectionInfoDoneEvent
    ///     * OnGetActivePublicLobbiesDoneEvent
    ///     * OnCreateLobbyDoneAsyncEvent
    ///
    /// - If you have a UI script, subscribe to the events above to handle them ^
    /// - To add API scripts: Add to the `ClientApis` serialized field.
    /// </summary>
    public abstract class HathoraClientMgrBase : MonoBehaviour
    {
        public static HathoraClientMgrBase Singleton { get; private set; }
        
        
        #region Serialized Fields
        [Header("(!) Get from Hathora dir; see hover tooltip")]
        [SerializeField, Tooltip("AppId should parity HathoraServerConfig (see top menu Hathora/Configuration")]
        private HathoraClientConfig hathoraClientConfig;
        public HathoraClientConfig HathoraClientConfig => hathoraClientConfig;

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


        #region Public Events
        /// <summary>Event triggers when auth is done (check isSuccess)</summary>
        /// <returns>isSuccess</returns>
        public static event Action<bool> OnAuthLoginDoneEvent;
        
        /// <summary>Event triggers when a net client is starting</summary>
        [Obsolete("This Event may be moved: NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        public static event Action OnClientStartingEvent;
        
        /// <summary>Event triggers when a net client started</summary>
        [Obsolete("This Event may be moved: NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        public static event Action OnClientStartedEvent;

        [Obsolete("This Event may be moved: NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        public static Action OnClientStoppedEvent;
        
        /// <summary>lobby</summary>
        public static event Action<Lobby> OnCreateLobbyDoneEvent;
        
        /// <summary>friendlyReason</summary>
        public static event Action<string> OnStartClientFailEvent;
        
        /// <summary>connectionInfo:ConnectionInfoV2</summary>
        public static event Action<ConnectionInfoV2> OnGetActiveConnectionInfoDoneEvent;
        
        /// <summary>lobbies:List (sorted by Newest @ top)</summary>
        public static event Action<List<Lobby>> OnGetActivePublicLobbiesDoneEvent;
        #endregion // Public Events
        
        /// <summary>Updates this on state changes</summary>
        protected bool IsConnectingAsClient { get; set; }
        
        
        #region Init
        protected virtual void Awake() =>
            SetSingleton();
        
        protected virtual void Start() =>
            initApis(_hathoraSdkConfig: null); // Base will create this

        /// <summary>
        /// You want other classes to easily be able to access your ClientMgr
        /// </summary>
        /// <summary>
        /// Set a singleton instance - we'll only ever have one serverMgr.
        /// Children probably want to override this and, additionally, add a Child singleton
        /// </summary>
        protected virtual void SetSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraClientMgrBase.SetSingleton] Error: " +
                    "setSingleton: Destroying dupe");
                
                Destroy(gameObject);
                return;
            }
            
            Singleton = this;
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
        #endregion // Init
        
        
        #region Interactions from UI
        #region Interactions from UI -> Required Overrides

        /// <summary>
        /// Starts a NetworkManager local Server.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// </summary>
        [Obsolete("NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        public abstract Task StartServer();

        /// <summary>
        /// Stops a NetworkManager local Server.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require HathoraServerMgr or secret keys to manage the net server.
        /// </summary>
        [Obsolete("NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        public abstract Task StopServer();

        /// <summary>Starts a NetworkManager Client</summary>
        /// <param name="_hostPort">host:port provided by Hathora</param>
        [Obsolete("NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        public abstract Task StartClient(string _hostPort = null);
        
        /// <summary>Stops a NetworkManager Client</summary>
        [Obsolete("NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        public abstract Task StopClient();
        #endregion // Interactions from UI -> Required Overrides
       
        
        #region Interactions from UI -> Optional overrides
        /// <summary>Both Server+Client at the same time:
        /// Essentially the same as StartServer() -> StartClient().
        /// Some NetworkManagers will have an actual StartHost() method; some do not.
        /// </summary>
        [Obsolete("NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        public virtual Task StartHost()
        {
            StartServer();
            StartClient();
            return Task.CompletedTask;
        }

        public virtual Task StopHost()
        {
            StopServer(); // Sometimes just this is enough
            StopClient();
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// If you want to StartClient() but only have a roomId:
        /// - Query Room api's GetConnectionInfo for host:port =>
        /// - StartClient()
        /// </summary>
        /// <param name="_roomId">Known from invite code, server list cache, or Lobby query</param>
        /// <returns>isSuccess</returns>
        [Obsolete("NetworkManager 'Net Code' will soon be detached from Hathora managers. " +
            "Instead, await GetActiveConnectionInfo().")]
        public virtual async Task<bool> StartClientByRoomIdAsync(string _roomId)
        {
            // Logs + Validate
            string logPrefix = $"[HathoraClientMgr.{nameof(StartClientByRoomIdAsync)}]";

            Debug.Log($"{logPrefix} Joining room `{_roomId}` ...");

            if (string.IsNullOrEmpty(_roomId))
                return false; // !isSuccess

            ConnectionInfoV2 connectionInfo = await GetActiveConnectionInfo(_roomId);
            
            ExposedPort exposedPort = connectionInfo?.ExposedPort;
            bool hasHost = !string.IsNullOrEmpty(exposedPort?.Host);
            if (!hasHost)
                return false;

            // ---------
            // [DEPRECATED - This part will soon be detached from Hathora managers]
            // Connect as Client via net code =>
            string hostPost = $"{exposedPort.Host}:{exposedPort.Port}";
            await StartClient(hostPost);
            
            return true; // isSuccess
        }
        
        /// <summary>
        /// Call this if we just got a Hathora ActiveConnectionInfo and we're about to connect to that host:port.
        /// - If !success, call OnConnectFailed().
        /// </summary>
        /// <returns>isValid</returns>
        protected virtual bool ValidateLastQueriedConnectionInfo()
        {
            // Validate host:port connection info
            if (hathoraClientSession.CheckIsValidServerConnectionInfo())
                return true; // success
            
            OnStartClientFail("Invalid ServerConnectionInfo");
            return false; // !success
        }

        /// <summary>Sets `IsConnecting` + logs ip:port (transport).</summary>
        /// <param name="_transportName"></param>
        protected virtual void SetConnectingState(string _transportName)
        {
            IsConnectingAsClient = true;

            Debug.Log("[HathoraClientBase.SetConnectingState] Connecting to: " + 
                $"{hathoraClientSession.GetServerInfoIpPort()} via " +
                $"NetworkManager.{_transportName} transport");
        }
        #endregion // Interactions from UI -> Optional overrides

        
        /// <summary>
        /// Auths anonymously => Creates new hathoraClientSession.
        /// - Resets cache completely on done (not necessarily success)
        /// - Sets `PlayerAuthToken` cache
        /// - Callback @ virtual OnAuthLoginComplete(isSuccess)
        /// </summary>
        public async Task<AuthResult> AuthLoginAsync(CancellationToken _cancelToken = default)
        {
            AuthResult authResult = await clientApis.ClientAuthApi.ClientAuthAsync(_cancelToken);

            hathoraClientSession.InitNetSession(authResult.PlayerAuthToken);
            OnAuthLoginDone(authResult.IsSuccess);

            return authResult;
        }

        /// <summary>
        /// Creates lobby => caches Lobby info @ hathoraClientSession.
        /// - Sets `Lobby` cache on done (not necessarily success)
        /// - Callback @ virtual OnCreateLobbyCompleteAsync(lobby)
        /// - Asserts IsAuthed
        /// </summary>
        /// <param name="_region"></param>
        /// <param name="_visibility"></param>
        /// <param name="_initConfigJsonStr"></param>
        /// <param name="_roomId"></param>
        /// <param name="_cancelToken"></param>
        public async Task<Lobby> CreateLobbyAsync(
            Region _region,
            CreateLobbyRequest.VisibilityEnum _visibility = CreateLobbyRequest.VisibilityEnum.Public,
            string _initConfigJsonStr = "{}",
            string _roomId = null,
            CancellationToken _cancelToken = default)
        {
            Assert.IsTrue(hathoraClientSession.IsAuthed, "expected hathoraClientSession.IsAuthed");

            Lobby lobby = await clientApis.ClientLobbyApi.ClientCreateLobbyAsync(
                hathoraClientSession.PlayerAuthToken,
                _visibility,
                _region,
                _initConfigJsonStr,
                _roomId,
                _cancelToken);
            
            hathoraClientSession.Lobby = lobby;
            OnCreateLobbyDoneAsync(lobby);

            return lobby;
        }

        /// <summary>
        /// Gets lobby info by roomId.
        /// - Asserts IsAuthed
        /// - Sets `Lobby` cache on done (not necessarily success)
        /// - Callback @ virtual OnCreateLobbyCompleteAsync(lobby)
        /// </summary>
        public async Task<Lobby> GetLobbyInfoAsync(
            string _roomId, 
            CancellationToken _cancelToken = default)
        {
            Assert.IsTrue(hathoraClientSession.IsAuthed, "expected hathoraClientSession.IsAuthed");

            Lobby lobby = await clientApis.ClientLobbyApi.ClientGetLobbyInfoAsync(
                _roomId,
                _cancelToken);
        
            hathoraClientSession.Lobby = lobby;
            OnCreateLobbyDoneAsync(lobby);

            return lobby;
        }

        /// <summary>
        /// Gets Public+active lobbies.
        /// - Asserts IsAuthed
        /// - Sets `Lobbies` cache on done (not necessarily success)
        /// - Callback @ virtual OnViewPublicLobbiesComplete(lobbies)
        /// </summary>
        /// <param name="_region">null returns all regions</param>
        /// <param name="_cancelToken"></param>
        public async Task<List<Lobby>> GetActivePublicLobbiesAsync(
            Region? _region = null,
            CancellationToken _cancelToken = default)
        {
            Assert.IsTrue(hathoraClientSession.IsAuthed, "expected hathoraClientSession.IsAuthed");
            
            List<Lobby> lobbies = await clientApis.ClientLobbyApi.ClientListPublicLobbiesAsync(
                _region,
                _cancelToken);

            hathoraClientSession.Lobbies = lobbies;
            OnGetActivePublicLobbiesDone(lobbies);

            return lobbies;
        }
        
        /// <summary>
        /// Gets ip:port (+transport type) info so we can connect the Client
        /// via the selected transport (eg: Fishnet).
        /// - Asserts IsAuthed
        /// - Polls until status is `Active`: May take a bit!
        /// - Sets `ServerConnectionInfo` cache on done (not necessarily success)
        /// - Callback @ virtual OnGetActiveConnectionInfoComplete(connectionInfo)
        /// </summary>
        public async Task<ConnectionInfoV2> GetActiveConnectionInfo(
            string _roomId, 
            CancellationToken _cancelToken = default)
        {
            Assert.IsTrue(hathoraClientSession.IsAuthed, "expected hathoraClientSession.IsAuthed");
            ConnectionInfoV2 connectionInfo = null;
            
            try
            {
                connectionInfo = await clientApis.ClientRoomApi.ClientGetConnectionInfoAsync(
                    _roomId,
                    _cancelToken: _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"[HathoraClientMgrBase.GetActiveConnectionInfo] " +
                    $"ClientGetConnectionInfoAsync => Error: {e}");

                throw;
            }
            finally
            {
                hathoraClientSession.ServerConnectionInfo = connectionInfo;
                OnGetActiveConnectionInfoDone(connectionInfo);
            }
            
            return connectionInfo;
        }
        #endregion // Interactions from UI
        
        
        #region Virtual callbacks w/Events
        /// <summary>AuthLogin() callback.</summary>
        /// <param name="_isSuccess"></param>
        protected virtual void OnAuthLoginDone(bool _isSuccess) =>
            OnAuthLoginDoneEvent?.Invoke(_isSuccess);
        
        /// <summary>GetActiveConnectionInfo() done callback (not necessarily successful).</summary>
        protected virtual void OnGetActiveConnectionInfoDone(ConnectionInfoV2 _connectionInfo) =>
            OnGetActiveConnectionInfoDoneEvent?.Invoke(_connectionInfo);

        /// <summary>GetActivePublicLobbies() callback.</summary>
        /// <param name="_lobbies"></param>
        protected virtual void OnGetActivePublicLobbiesDone(List<Lobby> _lobbies)
        {
            // Sort lobbies by create date -> Pass to UI
            List<Lobby> sortedFromNewestToOldest = _lobbies.OrderByDescending(lobby => 
                lobby.CreatedAt).ToList();
            
            OnGetActivePublicLobbiesDoneEvent?.Invoke(sortedFromNewestToOldest);
        }
        
        /// <summary>
        /// On success, most users will want to call GetActiveConnectionInfo().
        /// </summary>
        /// <param name="_lobby"></param>
        protected virtual void OnCreateLobbyDoneAsync(Lobby _lobby) => 
            OnCreateLobbyDoneEvent?.Invoke(_lobby);

        #region Virtual callbacks w/events -> NetworkManager Callbacks (!) To be moved later away from HathoraMgrs
        /// <summary>
        /// OnClientConnectionState() success callback when state == Started. Before this:
        /// - We already called StartClient() || StartClientToLastCachedRoom()
        /// - IsConnectingAsClient == true
        /// </summary>
        protected virtual void OnClientStarting()
        {
            IsConnectingAsClient = false;
            OnClientStartingEvent?.Invoke();
        }
        
        /// <summary>We just started and can now run net code</summary>
        [Obsolete("This Event may be moved: NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        protected virtual void OnClientStarted() =>
            OnClientStartedEvent?.Invoke();

        /// <summary>We were disconnected from net code</summary>
        [Obsolete("This Event may be moved: NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        protected virtual void OnClientStopped() =>
            OnClientStoppedEvent?.Invoke();
        
        /// <summary>Tried to connect to a Server as a Client, but failed</summary>
        /// <param name="_friendlyReason"></param>
        [Obsolete("This Event may be moved: NetworkManager 'Net Code' will soon be detached from Hathora managers")]
        protected virtual void OnStartClientFail(string _friendlyReason)
        {
            IsConnectingAsClient = false;
            OnStartClientFailEvent?.Invoke(_friendlyReason);
        }
        #endregion // Virtual callbacks w/events -> // NetworkManager Callbacks (!) To be moved later away from HathoraMgrs
        #endregion // Virtual callbacks w/Events
        
        
        #region Utils
        /// <summary>
        /// This was likely passed in from the UI to override the default
        /// NetworkManager (often from Standalone Client). Eg:
        /// "1.proxy.hathora.dev:12345" -> "1.proxy.hathora.dev", 12345
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
        
        /// <summary>
        /// We just need HathoraClientConfig serialized to a scene NetworkManager, with `AppId` set.
        /// - Does not throw, so you can properly handle UI on err.
        /// </summary>
        /// <returns>isValid</returns>
        public bool CheckIsValidToAuth() =>
            hathoraClientConfig != null && hathoraClientConfig.HasAppId;
        #endregion // Utils
    }
}

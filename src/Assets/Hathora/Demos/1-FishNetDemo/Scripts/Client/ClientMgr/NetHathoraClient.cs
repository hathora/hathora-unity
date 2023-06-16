// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Client;
using Hathora.Core.Scripts.Runtime.Client.Config;
using Hathora.Core.Scripts.Runtime.Client.Models;
using Hathora.Core.Scripts.Runtime.Common.Extensions;
using Hathora.Demos.Shared.Scripts.Client;
using Hathora.Demos.Shared.Scripts.Client.Models;
using UnityEngine;

namespace Hathora.Demos._1_FishNetDemo.Scripts.Client.ClientMgr
{
    /// <summary>
    /// This spawns BEFORE the player, or even connected to the network.
    /// This is the entry point to call Hathora SDK: Auth, lobby, rooms, etc.
    /// To add scripts, add to the `ClientApis` serialized field.
    /// </summary>
    public class NetHathoraClient : MonoBehaviour
    {
        /// <summary>Updates @ OnClientConnectionState</summary>
        private LocalConnectionState localConnectionState;
        private bool isConnecting;
        
        [Header("(!) Get from Hathora dir; see hover tooltip")]
        [SerializeField, Tooltip("AppId should parity HathoraServerConfig (see top menu Hathora/Configuration")]
        private HathoraClientConfig netHathoraConfig;
        
        [Header("Session, APIs")]
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
        }

        public void AssertUsingValidNetConfig()
        {
            // Are we using any Client Config at all?
            if (netHathoraConfig == null || !netHathoraConfig.HasAppId && NetUI.Singleton != null)
                NetUI.Singleton.SetInvalidConfig(netHathoraConfig);
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
            
            // This is a Client manager script; listen for relative events
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
        }
        #endregion // Init
        
        
        #region Interactions from UI
        /// <summary>
        /// Connect to the Server as a Client via net code. Uses cached vals.
        /// Currently uses FishNet.Tugboat (UDP) transport.
        /// This will trigger `OnClientConnectionState(state)`
        /// </summary>
        /// <returns>isSuccess</returns>
        public bool ConnectAsync()
        {
            Debug.Log("[NetHathoraClient] ConnectAsync");

            isConnecting = true;
            Transport transport = InstanceFinder.TransportManager.Transport;
            ClientManager clientMgr = InstanceFinder.ClientManager;

            // -----------------
            // Validate; UI and err handling is handled within
            bool isReadyToConnect = preConnectCheck(clientMgr, transport);
            if (!isReadyToConnect)
                return false; // !isSuccess

            // -----------------
            // Connect
            Debug.Log("[NetHathoraClient.ConnectAsync] Connecting to: " + 
                $"{netSession.GetServerInfoIpPort()} via FishNet.{transport.name} transport");

            ExposedPort connectInfo = netSession.ServerConnectionInfo.ExposedPort;
            bool isSuccess = InstanceFinder.ClientManager.StartConnection(
                connectInfo.Host, 
                (ushort)connectInfo.Port);

            if (!isSuccess)
            {
                onConnectFailed("StartConnection !isSuccess");
                return false;
            }
            
            return true; // isSuccess
        }

        private bool preConnectCheck(
            ClientManager _clientMgr, 
            Transport _transport)
        {
            // Validate host:port connection info
            if (!netSession.CheckIsValidServerConnectionInfo())
            {
                onConnectFailed("Invalid ServerConnectionInfo");
                return false; // !isStarted
            }

            if (InstanceFinder.NetworkManager == null)
            {
                onConnectFailed("!NetworkManager");
                return false; // !isSuccess
            }

            // Validate state
            LocalConnectionState currentState = _transport.GetConnectionState(server: false);
            if (currentState != LocalConnectionState.Stopped)
            {
                _clientMgr.StopConnection();
                onConnectFailed("Prior connection !stopped: Try again soon");
                return false; // !isSuccess
            }
            
            // Success - ready to connect
            return true;
        }

        private void onConnectFailed(string _friendlyReason)
        {
            isConnecting = false;
            NetUI.Singleton.OnJoinLobbyFailed(_friendlyReason);
        }

        private void OnClientConnectionState(ClientConnectionStateArgs _state)
        {
            localConnectionState = _state.ConnectionState;
            Debug.Log($"[NetHathoraClient.OnClientConnectionState] " +
                $"New state: {localConnectionState}");
            
            // onConnectSuccess?
            if (localConnectionState == LocalConnectionState.Started)
                onConnectSuccess();
            
            // onConnectFailed?
            bool stopped = localConnectionState == LocalConnectionState.Stopped; 
            bool stoppedConnecting = stopped && isConnecting;
            if (stoppedConnecting)
                onConnectFailed("Connection stopped");
        }

        private void onConnectSuccess()
        {
            Debug.Log("[NetHathoraClient] onConnectSuccess");
            isConnecting = false;
            NetUI.Singleton.OnJoinLobbySuccess();
        }

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
        /// <param name="_region"></param>
        /// <param name="_visibility"></param>
        public async Task CreateLobbyAsync(
            Region _region,
            CreateLobbyRequest.VisibilityEnum _visibility = CreateLobbyRequest.VisibilityEnum.Public)
        {
            Lobby lobby = null;
            try
            {
                lobby = await ClientApis.clientLobbyApi.ClientCreateLobbyAsync(
                    _visibility,
                    _region);
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
        /// <param name="_region">
        /// TODO (to confirm): null region returns *all* region lobbies?
        /// </param>
        public async Task ViewPublicLobbies(Region? _region = null)
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

            string friendlyRegion = lobby.Region.ToString().SplitPascalCase();
            NetUI.Singleton.OnCreatedOrJoinedLobby(lobby.RoomId, friendlyRegion);
        }
        #endregion // Callbacks
    }
}

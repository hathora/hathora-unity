// Created by dylan@hathora.dev

using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting;
using Hathora.Cloud.Sdk.Model;
using Hathora.Demos.Shared.Scripts.Client;
using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using UnityEngine;

namespace Hathora.Demos._1_FishNetDemo.HathoraScripts.Client.ClientMgr
{
    /// <summary>
    /// - This spawns BEFORE the player, or even connected to the network.
    /// - This is the entry point to call Hathora SDK: Auth, lobby, rooms, etc.
    /// - To add API scripts: Add to the `ClientApis` serialized field.
    /// </summary>
    public class HathoraFishnetClient : HathoraClientBase
    {
        #region vars
        public static HathoraFishnetClient Singleton { get; private set; }

        /// <summary>Updates @ OnClientConnectionState</summary>
        private LocalConnectionState localConnectionState;
        #endregion // vars

        
        #region Init
        protected override void OnAwake()
        {
            base.OnAwake();
            setSingleton();
        }

        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraFishnetClient]**ERR @ setSingleton: Destroying dupe");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }
        
        protected override void OnStart()
        {
            base.OnStart();
            
            // This is a Client manager script; listen for relative events
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
        }
        #endregion // Init
        
        
        #region Interactions from UI
        public void StartHost()
        {
            StartServer();
            StartClient();
        }

        public void StartServer() =>
            InstanceFinder.ServerManager.StartConnection();
        
        public void StartClient() =>
            InstanceFinder.ClientManager.StartConnection();
        
        public void StopHost() =>
            StopServer(_sendDisconnectMsgToClients: true);

        public void StopServer(bool _sendDisconnectMsgToClients) =>
            InstanceFinder.ServerManager.StopConnection(_sendDisconnectMsgToClients);
        
        public void StopClient() =>
            InstanceFinder.ClientManager.StopConnection();

        /// <summary>
        /// Connect to the Server as a Client via net code. Uses cached vals.
        /// Currently uses FishNet.Tugboat (UDP) transport.
        /// This will trigger `OnClientConnectionState(state)`
        /// </summary>
        /// <returns>isSuccess</returns>
        public bool Connect()
        {
            Debug.Log("[HathoraFishnetClient] ConnectAsync");

            IsConnecting = true;
            Transport transport = InstanceFinder.TransportManager.Transport;
            ClientManager clientMgr = InstanceFinder.ClientManager;

            // -----------------
            // Validate; UI and err handling is handled within
            bool isReadyToConnect = ValidateIsReadyToConnect(clientMgr, transport);
            if (!isReadyToConnect)
                return false; // !isSuccess

            // -----------------
            // Connect
            Debug.Log("[HathoraFishnetClient.ConnectAsync] Connecting to: " + 
                $"{HathoraClientSession.GetServerInfoIpPort()} via FishNet " +
                $"NetworkManager.{transport.name} transport");

            ExposedPort connectInfo = HathoraClientSession.ServerConnectionInfo.ExposedPort;
            bool isSuccess = InstanceFinder.ClientManager.StartConnection(
                connectInfo.Host, 
                (ushort)connectInfo.Port);

            if (!isSuccess)
            {
                OnConnectFailed("StartConnection !isSuccess");
                return false;
            }
            
            return true; // isSuccess
        }

        private bool ValidateIsReadyToConnect(
            ClientManager _clientMgr, 
            Transport _transport)
        {
            if (!ValidateServerConfigConnectionInfo())
                return false;

            if (InstanceFinder.NetworkManager == null)
            {
                OnConnectFailed("!NetworkManager");
                return false; // !isSuccess
            }

            // Validate state
            LocalConnectionState currentState = _transport.GetConnectionState(server: false);
            if (currentState != LocalConnectionState.Stopped)
            {
                _clientMgr.StopConnection();
                OnConnectFailed("Prior connection !stopped: Try again soon");
                return false; // !isSuccess
            }
            
            // Success - ready to connect
            return true;
        }

        private void OnClientConnectionState(ClientConnectionStateArgs _state)
        {
            localConnectionState = _state.ConnectionState;
            Debug.Log($"[HathoraFishnetClient.OnClientConnectionState] " +
                $"New state: {localConnectionState}");
            
            // onConnectSuccess?
            if (localConnectionState == LocalConnectionState.Started)
                OnConnectSuccess();
            
            // onConnectFailed?
            bool stopped = localConnectionState == LocalConnectionState.Stopped; 
            bool stoppedConnecting = stopped && IsConnecting;
            if (stoppedConnecting)
                OnConnectFailed("Connection stopped");
        }
        #endregion // Callbacks
    }
}

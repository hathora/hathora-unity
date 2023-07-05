// Created by dylan@hathora.dev

using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting;
using Hathora.Cloud.Sdk.Model;
using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using UnityEngine;

namespace Hathora.Demos._1_FishNetDemo.HathoraScripts.Client.ClientMgr
{
    /// <summary>
    /// - This spawns BEFORE the player, or even connected to the network.
    /// - This is the entry point to call Hathora SDK: Auth, lobby, rooms, etc.
    /// - To add API scripts: Add to the `ClientApis` serialized field.
    /// </summary>
    public class HathoraFishnetClientMgr : HathoraClientMgrBase
    {
        #region vars
        public static HathoraFishnetClientMgr Singleton { get; private set; }
        
        private Transport transport => 
            InstanceFinder.TransportManager.Transport; 

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
            base.InitOnStart(HathoraFishnetClientMgrUi.Singleton);
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

        public override void StartServer() =>
            InstanceFinder.ServerManager.StartConnection();
        
        public override void StartClient() =>
            InstanceFinder.ClientManager.StartConnection();
        
        public override void StopHost() =>
            StopServer(); // StopServer() will also stop the client

        public override void StopServer() =>
            InstanceFinder.ServerManager.StopConnection(sendDisconnectMessage: true);
        
        public override void StopClient() =>
            InstanceFinder.ClientManager.StopConnection();

        /// <summary>
        /// Connect to the Server as a Client via net code. Uses cached vals.
        /// Currently uses FishNet.Tugboat (UDP) transport.
        /// This will trigger `OnClientConnectionState(state)`
        /// </summary>
        /// <returns>
        /// startedConnection; to ATTEMPT the connection (isValid pre-connect vals); we're not connected yet.
        /// </returns>
        public override bool ConnectAsClient()
        {
            Debug.Log("[HathoraFishnetClient] ConnectAsync");
            
            // Set connecting state + log where we're connecting to
            base.SetConnectingState(transport.name);

            // -----------------
            // Validate; UI and err handling is handled within
            bool isReadyToConnect = ValidateIsReadyToConnect(InstanceFinder.ClientManager, transport);
            if (!isReadyToConnect)
                return false; // !startedConnection

            // -----------------
            // Set port + host (ip)
            ExposedPort connectInfo = HathoraClientSession.ServerConnectionInfo.ExposedPort;
            transport.SetPort((ushort)connectInfo.Port);
            transport.SetClientAddress(connectInfo.Host);
            
            // Connect now => cb @ OnClientConnected()
            bool startedConnection = InstanceFinder.ClientManager.StartConnection();
            return startedConnection;
        }

        private bool ValidateIsReadyToConnect(
            ClientManager _clientMgr, 
            Transport _transport)
        {
            if (!ValidateServerConfigConnectionInfo())
                return false;

            if (InstanceFinder.NetworkManager == null)
            {
                base.OnConnectFailed("!NetworkManager");
                return false; // !isSuccess
            }

            // Validate state
            LocalConnectionState currentState = _transport.GetConnectionState(server: false);
            if (currentState != LocalConnectionState.Stopped)
            {
                _clientMgr.StopConnection();
                base.OnConnectFailed("Prior connection !stopped: Try again soon");
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
                base.OnConnectSuccess();
            
            // onConnectFailed?
            bool stopped = localConnectionState == LocalConnectionState.Stopped; 
            bool stoppedConnecting = stopped && IsConnecting;
            if (stoppedConnecting)
                base.OnConnectFailed("Connection stopped");
        }
        #endregion // Callbacks
    }
}

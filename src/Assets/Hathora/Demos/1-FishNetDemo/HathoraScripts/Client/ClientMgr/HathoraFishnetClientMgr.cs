// Created by dylan@hathora.dev

using System.Threading.Tasks;
using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting;
using FishNet.Transporting.Bayou;
using Hathora.Cloud.Sdk.Model;
using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using UnityEngine;
using UnityEngine.Assertions;

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

        private static Transport transport => 
            InstanceFinder.TransportManager.Transport;

        /// <summary>Updates @ OnClientConnectionState</summary>
        private LocalConnectionState localConnectionState;
        #endregion // vars

        
        #region Init
        /// <summary>Be sure to override when using Start() + Awake()</summary>
        protected override void Awake() =>
            base.Awake();
        
        protected override void Start()
        {
            base.Start();
            base.InitOnStart(HathoraFishnetClientMgrDemoUi.Singleton); // Allows UI calls on logic callbacks
            
            // This is a Client manager script; listen for relative events
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
        }

        protected override void SetSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraFishnetClient]**ERR @ SetSingleton: Destroying dupe");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }
        #endregion // Init
        
        
        #region Interactions from UI
        /// <summary>
        /// Starts a NetworkManager local Server *and* Client at the same time.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// TODO: Mv to HathoraHostMgr
        /// </summary>
        public override async Task StartHost()
        {
            await StartServer();
            await StartClient();
        }

        /// <summary>
        /// Starts a NetworkManager local Server.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// </summary>
        public override Task StartServer()
        {
            InstanceFinder.ServerManager.StartConnection();
            return Task.CompletedTask;
        }

        ///<summary>Starts a NetworkManager Client</summary>
        /// <param name="_hostPort">host:port provided by Hathora; eg: "1.proxy.hathora.dev:12345"</param>
        public override Task StartClient(string _hostPort = null)
        {
            Debug.Log("[HathoraFishnetClientMgr] StartClient");
            (string hostNameOrIp, ushort port) hostPortContainer = SplitPortFromHostOrIp(_hostPort);
            bool hasHost = !string.IsNullOrEmpty(hostPortContainer.hostNameOrIp);
            bool hasPort = hostPortContainer.port > 0;

            // Start FishNet Client via selected Transport
            if (hasHost && hasPort)
            {
                Debug.Log($"[HathoraFishnetClientMgr] StartClient w/Custom hostPort: " +
                    $"`{hostPortContainer.hostNameOrIp}:{hostPortContainer.port}`");
                
                InstanceFinder.ClientManager.StartConnection(
                    hostPortContainer.hostNameOrIp, 
                    hostPortContainer.port);    
            }
            else
            {
                Debug.Log($"[HathoraFishnetClientMgr] StartClient w/NetworkSettings config");
                InstanceFinder.ClientManager.StartConnection();
            }
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops a NetworkManager local Server *and* Client at the same time.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// TODO: Mv to HathoraHostMgr
        /// </summary>
        public override Task StopHost()
        {
            StopServer(); // StopServer() will also stop the client
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops a NetworkManager local Server.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// </summary>
        public override Task StopServer()
        {
            InstanceFinder.ServerManager.StopConnection(sendDisconnectMessage: true);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts a NetworkManager Client.
        /// </summary>
        public override Task StopClient()
        {
            InstanceFinder.ClientManager.StopConnection();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Connect to the Server as a Client via net code. Uses cached vals.
        /// This will trigger `OnClientConnectionState(state)`
        /// - WebGL: Asserts for `Bayou` as the NetworkManager's selected transport
        /// - !WebGL: Asserts for `!Bayou` as the NetworkManager's selected transport (such as `Tugboat` UDP)
        /// </summary>
        /// <returns>
        /// startedConnection; to ATTEMPT the connection (isValid pre-connect vals); we're not connected yet.
        /// </returns>
        public override Task<bool> ConnectAsClient()
        {
            // Set connecting state + log where we're connecting to
            string transportName = transport.GetType().Name;
            base.SetConnectingState(transportName);

            // -----------------
            // Validate; UI and err handling is handled within
            bool isReadyToConnect = ValidateIsReadyToConnect(InstanceFinder.ClientManager, transport);
            if (!isReadyToConnect)
                return Task.FromResult(false); // !startedConnection

            Bayou bayouTransport = transport as Bayou; 
#if UNITY_WEBGL
            Assert.IsNotNull(bayouTransport, "Expected NetworkManager to use " +
                $"{nameof(bayouTransport)} for WebGL build -- if more transports for WebGL " +
                "came out later, edit this Assert script");
#else
            Assert.IsNull(bayouTransport, "!Expected NetworkManager to use " +
                $"{nameof(bayouTransport)} for !WebGL build - Set NetworkManager "+
                "transport to, for example, `Tugboat` (supporting UDP).");
#endif

            // -----------------
            // Set port + host (ip)
            ExposedPort connectInfo = HathoraClientSession.ServerConnectionInfo.ExposedPort;
            transport.SetPort((ushort)connectInfo.Port);
            transport.SetClientAddress(connectInfo.Host);
            
            // Connect now => cb @ OnClientConnected()
            bool startedConnection = InstanceFinder.ClientManager.StartConnection();
            return Task.FromResult(startedConnection);
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

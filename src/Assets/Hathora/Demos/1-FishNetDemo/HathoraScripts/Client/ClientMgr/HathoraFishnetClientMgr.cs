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
        protected override void Awake() =>
            base.Awake(); // Triggers SetSingleton()
        
        protected override void Start()
        {
            base.Start();
            
            // This is a Client manager script; listen for relative events
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
        }

        protected override void SetSingleton()
        {
            base.SetSingleton();
            
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
        /// Starts a NetworkManager local Server.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// </summary>
        public override Task StartNetServer()
        {
            InstanceFinder.ServerManager.StartConnection();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Connect with info `from HathoraClientSession.ServerConnectionInfo.ExposedPort`,
        /// replacing the NetworkManager host:port.
        /// </summary>
        /// <returns></returns>
        public Task StartNetClientFromHathoraLastQueriedLobbyInfo()
        {
            ExposedPort connectInfo = HathoraClientSession.ServerConnectionInfo.ExposedPort;
            string hostPort = $"{connectInfo.Host}:{connectInfo.Port}";
            
            return StartNetClient(hostPort);
        }

        ///<summary>Sets NetworkManager -> StartNetClientFromNetworkMgrCache()</summary>
        /// <param name="_hostPort">host:port provided by Hathora; eg: "1.proxy.hathora.dev:12345"</param>
        public override Task StartNetClient(string _hostPort = null)
        {
            string logPrefix = $"[HathoraFishnetClientMgr] {nameof(StartNetClient)}]"; 
            Debug.Log($"{logPrefix} Start");
            
            (string hostNameOrIp, ushort port) hostPortContainer = SplitPortFromHostOrIp(_hostPort);
            bool hasHost = !string.IsNullOrEmpty(hostPortContainer.hostNameOrIp);
            bool hasPort = hostPortContainer.port > 0;

            // Start FishNet Client via selected Transport
            if (!hasHost || !hasPort)
            {
                // Just use vals from the NetworkMgr
                Debug.Log($"{logPrefix} w/NetworkSettings config");
            }
            else
            {
                // Set custom host:port 1st
                Debug.Log($"{logPrefix} w/Custom hostPort: " +
                    $"`{hostPortContainer.hostNameOrIp}:{hostPortContainer.port}`");

                InstanceFinder.TransportManager.Transport.SetClientAddress(hostPortContainer.hostNameOrIp);
                InstanceFinder.TransportManager.Transport.SetPort(hostPortContainer.port);
            }
            
            StartNetClientFromNetworkMgrCache();
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Connect to the NetServer as a NetClient.
        /// Unlike StartNetClient (that takes host:port args), we'll use cached vals from NetworkManager.
        /// This will trigger `OnClientConnectionState(state)`
        /// - WebGL: Asserts for `Bayou` as the NetworkManager's selected transport
        /// - !WebGL: Asserts for `!Bayou` as the NetworkManager's selected transport (such as `Tugboat` UDP)
        /// </summary>
        /// <returns>
        /// startedConnection; to ATTEMPT the connection (isValid pre-connect vals); we're not connected yet.
        /// </returns>
        public bool StartNetClientFromNetworkMgrCache()
        {
            Debug.Log("[HathoraFishnetClient] StartNetClientFromNetworkMgrCache");

            // Set connecting state + log where we're connecting to
            string transportName = transport.GetType().Name;
            base.SetConnectingState(transportName);

            // -----------------
            // Validate; UI and err handling is handled within
            bool isReadyToConnect = validateIsReadyToConnect();
            if (!isReadyToConnect)
                return false; // !startedConnection
            
            // Connect now using NetworkManager settings we just set above
            bool startedConnection = InstanceFinder.ClientManager.StartConnection();
            return startedConnection; // startedConnection => callback @ OnClientConected()0
        }

        /// <summary>
        /// Stops a NetworkManager local Server.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// </summary>
        public override Task StopNetServer()
        {
            InstanceFinder.ServerManager.StopConnection(sendDisconnectMessage: true);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts a NetworkManager Client.
        /// </summary>
        public override Task StopNetClient()
        {
            InstanceFinder.ClientManager.StopConnection();
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// We're about to connect to a server as a Client - ensure we're ready.
        /// </summary>
        /// <returns></returns>
        private bool validateIsReadyToConnect()
        {
            Debug.Log("[HathoraFishnetClientMgr] validateIsReadyToConnect");
            
            if (InstanceFinder.NetworkManager == null)
            {
                base.OnNetStartClientFail("!NetworkManager");
                return false; // !isSuccess
            }

            // Validate state
            LocalConnectionState currentState = transport.GetConnectionState(server: false);
            if (currentState != LocalConnectionState.Stopped)
            {
                InstanceFinder.ClientManager.StopConnection();
                base.OnNetStartClientFail("Prior connection !stopped: Try again soon");
                return false; // !isSuccess
            }
            
            // Validate transport
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
            
            // Success - ready to connect
            return true;
        }

        private void OnClientConnectionState(ClientConnectionStateArgs _state)
        {
            localConnectionState = _state.ConnectionState;
            Debug.Log($"[HathoraFishnetClient.OnClientConnectionState] " +
                $"New state: {localConnectionState}");
            
            switch (localConnectionState)
            {
                case LocalConnectionState.Starting:
                    OnNetClientStarting();
                    break;

                // onConnectSuccess?
                case LocalConnectionState.Started:
                    base.OnNetClientStarted();
                    break;
            }

            // OnNetStartClientFail?
            bool stopped = localConnectionState == LocalConnectionState.Stopped; 
            bool stoppedConnecting = stopped && IsConnectingAsClient;
            if (stoppedConnecting)
                base.OnNetStartClientFail("Connection stopped");
        }
        #endregion // Callbacks
    }
}

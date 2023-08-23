// Created by dylan@hathora.dev

using Hathora.Demos.Shared.Scripts.Common;
using Mirror;
using Mirror.SimpleWeb;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Client.ClientMgr
{
    /// <summary>
    /// Acts as the liason between NetworkManager and HathoraClientMgr.
    /// - This child class tracks FishNet NetworkManager state changes, and:
    ///   * Handles setting the NetworkManager [host|ip]:port.
    ///   * Can talk to Hathora scripts to get cached host:port.
    ///   * Can initialize or stop NetworkManager connections.
    ///   * Tells base to log + trigger OnDone() events other scripts subcribe to.
    /// - Base contains funcs like: StartServer, StartClient, StartHost.
    /// - Base contains events like: OnClientStarted, OnClientStopped.
    /// - Base tracks `ClientState` like: Stopped, Starting, Started.
    /// </summary>
    public class MirrorStateMgr : NetworkMgrStateTracker
    {
        #region vars
        /// <summary>
        /// `New` keyword overrides base Singleton when accessing child directly.
        /// </summary>
        public new static MirrorStateMgr Singleton { get; private set; }

        /// <summary>Get specific transport info. For setting the port, see portTransport.</summary>
        private static Transport transport => 
            Mirror.NetworkManager.singleton == null 
                ? null 
                : Mirror.NetworkManager.singleton.transport;
        
        /// <summary>Use this to set the `Port` prop of the transport.</summary>
        private static PortTransport portTransport => 
            NetworkManager.singleton.transport as PortTransport;
        #endregion // vars
        

        #region Init
        protected override void Awake()
        {
            base.Awake(); // Sets base Singleton
            setSingleton();
        }

        /// <summary>Subscribe to NetworkManager Client state changes</summary>
        protected override void Start()
        {
            base.Start();
            subToMirrorStateEvents();
        }

        /// <summary>Mirror has no singular event tracker enum; track individual events.</summary>
        private void subToMirrorStateEvents()
        {
            transport.OnClientConnected += OnClientStarted;
            transport.OnClientError += onMirrorClientError;
            transport.OnClientDisconnected += () => 
                base.OnStartClientFail(CONNECTION_STOPPED_FRIENDLY_STR);;
        }
        
        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError($"[{nameof(MirrorStateMgr)}]**ERR @ " +
                    $"{nameof(setSingleton)}: Destroying dupe");
                
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }
        #endregion // Init
        

        #region NetworkManager Server
        /// <summary>Starts a NetworkManager local Server.</summary>
        public void StartServer() =>
            NetworkManager.singleton.StartServer();
        
        /// <summary>Stops a NetworkManager local Server.</summary>
        public void StopServer() =>
            NetworkManager.singleton.StopServer();
        #endregion
        
        
        #region NetworkManager Client
        ///<summary>
        /// Connect to the NetworkManager Server as a NetworkManager Client using custom host:ip.
        /// We'll set the host:ip to the NetworkManger -> then call StartClientFromNetworkMgr().
        /// </summary>
        /// <param name="_hostPort">Contains "host:port" - eg: "1.proxy.hathora.dev:12345"</param>
        /// <returns>
        /// startedConnection; to *attempt* the connection (isValid pre-connect vals); we're not connected yet.
        /// </returns>
        public bool StartClient(string _hostPort)
        {
            string logPrefix = $"[{nameof(MirrorStateMgr)}.{nameof(StartClient)}]";
            Debug.Log($"{logPrefix} Start");
            
            (string hostNameOrIp, ushort port) hostPortContainer = SplitPortFromHostOrIp(_hostPort);
            bool hasHost = !string.IsNullOrEmpty(hostPortContainer.hostNameOrIp);
            bool hasPort = hostPortContainer.port > 0;

            // Start Mirror Client via selected Transport
            if (!hasHost)
            {
                Debug.LogError($"{logPrefix} !hasHost (from provided `{_hostPort}`): " +
                    "Instead, using default NetworkSettings config");
            }
            else if (!hasPort)
            {
                Debug.LogError($"{logPrefix} !hasPort (from provided `{_hostPort}`): " +
                    "Instead, using default NetworkSettings config");
            }
            else
            {
                // Set custom host:port 1st
                Debug.Log($"{logPrefix} w/Custom hostPort: " +
                    $"`{hostPortContainer.hostNameOrIp}:{hostPortContainer.port}`");

                NetworkManager.singleton.networkAddress = hostPortContainer.hostNameOrIp;
                portTransport.Port = hostPortContainer.port;
            }

            return StartClient();
        }
        
        /// <summary>
        /// Connect to the NetworkManager Server as a NetworkManager Client using NetworkManager host:ip.
        /// This will trigger `OnClientConnecting()` related events.
        ///
        /// TRANSPORT VALIDATION:
        /// - WebGL: Asserts for `SimpleWebTransport` as the NetworkManager's selected transport
        /// - !WebGL: Asserts for `!SimpleWebTransport` as the NetworkManager's selected transport (such as `Kcp` UDP)
        /// </summary>
        /// <returns>
        /// startedConnection; to *attempt* the connection (isValid pre-connect vals); we're not connected yet.
        /// </returns>
        public bool StartClient()
        {
            string logPrefix = $"[{nameof(MirrorStateMgr)}.{nameof(StartClient)}]";
            Debug.Log($"{logPrefix} Start");
            
            // Validate
            bool isReadyToConnect = validateIsReadyToConnect(); // Handles UI + logs within
            if (!isReadyToConnect)
                return false; // !startedConnection
            
            // Log "host:port (transport)" -> Connect using NetworkManager settings
            string transportName = transport.GetType().Name;
            Debug.Log($"[{logPrefix} Connecting to {NetworkManager.singleton.networkAddress}:" +
                $"{portTransport.Port}` via `{transportName}` transport");
            
            base.OnClientConnecting(); // => callback @ OnClientConected() || OnStartClientFail()
            NetworkManager.singleton.StartClient();
            return true; // startedConnection => callback @ OnClientConected()
        }
        
        /// <summary>
        /// Starts a NetworkManager Client using Hathora lobby session [last queried] cached host:port.
        /// Connect with info from `HathoraClientSession.ServerConnectionInfo.ExposedPort`,
        /// replacing the NetworkManager host:port.
        /// </summary>
        /// <returns>
        /// startedConnection; to *attempt* the connection (isValid pre-connect vals); we're not connected yet.
        /// </returns>
        public bool StartClientFromHathoraLobbySession()
        {
            string hostPort = GetHathoraSessionHostPort();
            return StartClient(hostPort);
        }
        
        /// <summary>Starts a NetworkManager Client.</summary>
        public void StopClient() =>
            NetworkManager.singleton.StopClient();
        
        /// <summary>We're about to connect to a server as a Client - ensure we're ready.</summary>
        /// <returns>isValid</returns>
        private bool validateIsReadyToConnect()
        {
            string logPrefix = $"[{nameof(MirrorStateMgr)}.{nameof(validateIsReadyToConnect)}]";
            Debug.Log($"{logPrefix} Start");

            if (NetworkManager.singleton == null)
            {
                OnStartClientFail("!NetworkManager");
                return false; // !isSuccess
            }

            // Validate state: Stop connection 1st, if necessary
            if (base.ClientState != ClientTrackedState.Stopped)
                NetworkClient.Disconnect();
            
            // Validate transport
            SimpleWebTransport webglSimpleWebTransport = transport as SimpleWebTransport;
            
#if UNITY_WEBGL
            Debug.Log($"{logPrefix} Validating WebGL Transport...");
            Assert.IsNotNull(webglSimpleWebTransport, $"{logPrefix} Expected NetworkManager to use " +
                $"{nameof(webglSimpleWebTransport)} for WebGL build -- if more transports for WebGL " +
                "came out later, edit this Assert script");
#else
            Debug.Log($"{logPrefix} Validating !WebGL Transport...");
            Assert.IsNull(webglSimpleWebTransport, "!Expected NetworkManager to use " +
                $"{nameof(webglSimpleWebTransport)} for !WebGL build - Set NetworkManager "+
                "transport to, for example, `Kcp` (supporting UDP).");
#endif
            
            // Success - ready to connect
            return true;
        }
        #endregion // NetworkManager Client
        
        
        /// <summary>Client connection err</summary>
        /// <param name="_transportErr"></param>
        /// <param name="_extraInfo">This is a complete guess of what it is; we just know it's a string</param>
        private void onMirrorClientError(TransportError _transportErr, string _extraInfo)
        {
            string friendlyReason = "onMirrorClientErr: " +
                $"transportErr={_transportErr}, " +
                $"extraInfo={_extraInfo}";
                
            base.OnStartClientFail(friendlyReason);
        }

        private void OnDestroy()
        {
            if (transport == null)
                return; // Perhaps already destroyed
            
            transport.OnClientConnected -= OnClientStarted;
            transport.OnClientError -= onMirrorClientError;
            transport.OnClientDisconnected -= () => 
                base.OnStartClientFail(CONNECTION_STOPPED_FRIENDLY_STR);
        }
    }
}

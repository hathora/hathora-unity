// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using kcp2k;
using Mirror;
using Mirror.SimpleWeb;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Client.ClientMgr
{
    /// <summary>
    /// - This spawns BEFORE the player, or even connected to the network.
    /// - This is the entry point to call Hathora SDK: Auth, lobby, rooms, etc.
    /// - To add API scripts: Add to the `ClientApis` serialized field.
    /// 
    /// MIRROR NOTES / NOTABLE PROPS + FUNCS:
    /// - Mirror.NetworkClient
    /// - Mirror.NetworkManager.singleton
    /// - Mirror.NetworkManager.ConnectState is INTERNAL, but broken up into individual props
    /// </summary>
    public class HathoraMirrorClientMgr : HathoraClientMgrBase
    {
        #region vars
        public static HathoraMirrorClientMgr Singleton { get; private set; }
        
        private static bool isConnected => NetworkClient.isConnected;
        private static bool isConnecting => NetworkClient.isConnecting;

        private static Transport transport => 
            Mirror.NetworkManager.singleton.transport;
        
        private static PortTransport portTransport => 
            Mirror.NetworkManager.singleton.transport as PortTransport;
        #endregion // vars
        

        #region Init
        /// <summary>Be sure to override when using Start() + Awake()</summary>
        protected override void Awake() =>
            base.Awake();

        protected override void Start()
        {
            base.Start();

            // This is a Client manager script; listen for relative events
            transport.OnClientConnected += OnClientStarted;
            transport.OnClientError += onMirrorClientError;
            transport.OnClientDisconnected += () => 
                base.OnStartClientFail("Disconnected");;
        }
        
        protected override void SetSingleton()
        {
            base.SetSingleton();

            if (Singleton != null)
            {
                Debug.LogError("[HathoraMirrorClient]**ERR @ SetSingleton: Destroying dupe");
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
        public override Task StartServer()
        {
            NetworkManager.singleton.StartServer();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Only call if UNITY_WEBGL (not validated here):
        /// This is actually pulled directly from Mirror's SimpleWebTransport.cs (!public)
        /// </summary>
        /// <returns>"WS" || "WSS"</returns>
        private string GetWebglClientScheme()
        {
            SimpleWebTransport swt = NetworkManager.singleton.transport as SimpleWebTransport;
            Assert.IsNotNull(swt, "Expected `SimpleWebTransport` in NetworkManager.Transport, " +
                "since UNITY_WEBGL - you are trying to call WS/WSS TCP transport from a Transport that !supports this");

            bool isWss = swt.sslEnabled || swt.clientUseWss; 
            return isWss 
                ? SimpleWebTransport.SecureScheme 
                : SimpleWebTransport.NormalScheme;
        }

        /// <param name="_hostPort">
        /// host:port provided by Hathora; eg: "1.proxy.hathora.dev:12345".
        /// If !hostPort, we'll use the default from NetworkManager and its selected Transport.
        /// </param>
        public override Task StartClient(string _hostPort = null)
        {
            string logPrefix = $"[HathoraMirrorClientMgr.{nameof(StartClient)}]";
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

                // InstanceFinder.TransportManager.Transport.SetClientAddress(hostPortContainer.hostNameOrIp);
                // InstanceFinder.TransportManager.Transport.SetPort(hostPortContainer.port);
            }

            StartClientFromNetworkMgr();
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Connect to the NetworkManager Server as a NetworkManager Client.
        /// Unlike StartClient (that takes host:port args), we'll use cached vals from NetworkManager.
        /// - WebGL: Asserts for `SimpleWebTransport` as the NetworkManager's selected transport
        /// - !WebGL: Asserts for `!SimpleWebTransport` as the NetworkManager's selected transport (such as `Kcp` UDP)
        /// </summary>
        /// <returns>
        /// startedConnection; to ATTEMPT the connection (isValid pre-connect vals); we're not connected yet.
        /// </returns>
        public bool StartClientFromNetworkMgr()
        {
            string logPrefix = $"[HathoraMirrorClientMgr.{nameof(StartClientFromNetworkMgr)}]";
            Debug.Log($"{logPrefix} Start");

            // Set connecting state + log where we're connecting to
            string transportName = transport.GetType().Name;
            base.SetConnectingState(transportName);
            
            // -----------------
            // Validate; UI and err handling is handled within
            bool isReadyToConnect = validateIsReadyToConnect(); // Handles UI + logs within
            if (!isReadyToConnect)
                return false; // !startedConnection

            // -----------------
            // Set port + host (ip)
            ExposedPort connectInfo = HathoraClientSession.ServerConnectionInfo.ExposedPort;
            NetworkManager.singleton.networkAddress = connectInfo.Host; // host address (eg: `localhost`); not an IP address
            portTransport.Port = (ushort)connectInfo.Port;
            
            // Connect now using NetworkManager settings we just set above
            NetworkManager.singleton.StartClient();
            return true; // startedConnection => callback @ OnClientConected()
        }

        public override Task StopServer()
        {
            NetworkManager.singleton.StopServer();
            return Task.CompletedTask;
        }

        public override Task StopClient()
        {
            NetworkManager.singleton.StopClient();
            return Task.CompletedTask;
        }

        private bool validateIsReadyToConnect()
        {
            string logPrefix = $"[HathoraMirrorClientMgr.{nameof(validateIsReadyToConnect)}]";
            Debug.Log($"{logPrefix} Start");
            
            if (!ValidateLastQueriedConnectionInfo())
                return false;

            if (NetworkManager.singleton == null)
            {
                OnStartClientFail("!NetworkManager");
                return false; // !isSuccess
            }

            // Validate state
            if (isConnected || isConnecting)
            {
                NetworkClient.Disconnect();
                OnStartClientFail("Prior connection still active: Disconnecting... " +
                    "Try again soon");
                
                return false; // !isSuccess
            }
            
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
                return;
            
            transport.OnClientConnected -= OnClientStarted;
            transport.OnClientError -= onMirrorClientError;
            transport.OnClientDisconnected -= () => 
                base.OnStartClientFail("Disconnected");;
        }
        #endregion // Callbacks
    }
}

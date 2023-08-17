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

        /// <summary>Req'd to set port</summary>
        private static KcpTransport kcpTransport =>
            (KcpTransport)transport;

        #endregion // vars
        

        #region Init
        /// <summary>SetSingleton(), SetTransport()</summary>
        protected override void OnAwake() =>
            base.OnAwake();

        protected override void SetSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraMirrorClient]**ERR @ SetSingleton: Destroying dupe");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }

        protected override void OnStart()
        {
            base.InitOnStart(HathoraMirrorClientMgrDemoUi.Singleton);
            base.OnStart();

            // This is a Client manager script; listen for relative events
            transport.OnClientConnected += base.OnConnectSuccess;
            transport.OnClientError += onMirrorClientError;
            transport.OnClientDisconnected += () => 
                base.OnConnectFailed("Disconnected");;
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
            if (string.IsNullOrEmpty(_hostPort))
            {
                // No custom host:port >> Start normally with NetworkManager settings
                NetworkManager.singleton.StartClient();
                return Task.CompletedTask;
            }
            
            // Start Mirror Client via selected Transport
            (string hostNameOrIp, ushort port) hostPortContainer = SplitPortFromHostOrIp(_hostPort);
            bool hasHost = !string.IsNullOrEmpty(hostPortContainer.hostNameOrIp);
            bool hasPort = hostPortContainer.port > 0;

            if (!hasHost || !hasPort)
            {
                // Has some kind of custom host:port, but is incomplete >> Start normally with NetworkManager settings
                Debug.LogError("[HathoraMirrorClientMgr.StartClient] Invalid `host:port` provided: " +
                    $"`{_hostPort}` -- Continuing with NetworkManager settings");
                
                NetworkManager.singleton.StartClient();
                return Task.CompletedTask;
            }
            
            // We have both host (or ip) && port >>
            string uriPrefix = "kcp"; // Default UDP transport prefix using KCP Transport

#if UNITY_WEBGL
            uriPrefix = GetWebglClientScheme(); // "ws" || "wss"
#endif
            
            Debug.Log($"[HathoraMirrorClientMgr.StartClient] Setting uri to `{uriPrefix}://{_hostPort}` ...");
            Uri uri = new($"{uriPrefix}://{_hostPort}"); // eg: "wss://1.proxy.hathora.dev:12345"
            Debug.Log($"[HathoraMirrorClientMgr.StartClient] Final uri: `{uri}`");
            
            // Start Mirror Client via selected Transport
            NetworkManager.singleton.StartClient(uri); // eg: "1.proxy.hathora.dev:12345"
            
            Debug.Log("[HathoraMirrorClientMgrBase.StartClient] " +
                $"Transport set to `{transport}` ({uriPrefix})");
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts a NetworkManager local Server *and* Client at the same time.
        /// This is in ClientMgr since it involves NetworkManager Net code,
        /// and does not require ServerMgr or secret keys to manage the net server.
        /// TODO: Mv to HathoraHostMgr
        /// </summary>
        public override Task StartHost()
        {
            NetworkManager.singleton.StartHost();
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
            NetworkManager.singleton.StopHost();
            return Task.CompletedTask;
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

        /// <summary>
        /// Connect to the Server as a Client via net code. Uses cached vals.
        /// Currently uses Mirror.Kcp (UDP) transport.
        /// </summary>
        /// <returns>
        /// startedConnection; to ATTEMPT the connection (isValid pre-connect vals); we're not connected yet.
        /// </returns>
        public override Task<bool> ConnectAsClient()
        {
            Debug.Log("[HathoraMirrorClient] ConnectAsync (expecting `Kcp` transport)");

            // Set connecting state + log where we're connecting to
            base.SetConnectingState(transport.name);
            
            // -----------------
            // Validate; UI and err handling is handled within
            bool isReadyToConnect = ValidateIsReadyToConnect(); // Handles UI + logs within
            if (!isReadyToConnect)
                return Task.FromResult(false); // !startedConnection

            // -----------------
            // Set port + host (ip)
            ExposedPort connectInfo = HathoraClientSession.ServerConnectionInfo.ExposedPort;
            NetworkManager.singleton.networkAddress = connectInfo.Host; // host address (eg: `localhost`); not an IP address
            kcpTransport.port = (ushort)connectInfo.Port;
            
            // Connect now => cb @ OnClientConnected()
            StartClient();
            return Task.FromResult(false); // startedConnection; continued @ OnClientConnected()
        }

        private bool ValidateIsReadyToConnect()
        {
            if (!ValidateServerConfigConnectionInfo())
                return false;

            if (NetworkManager.singleton == null)
            {
                OnConnectFailed("!NetworkManager");
                return false; // !isSuccess
            }

            // Validate state
            if (isConnected || isConnecting)
            {
                NetworkClient.Disconnect();
                OnConnectFailed("Prior connection still active: Disconnecting... " +
                    "Try again soon");
                
                return false; // !isSuccess
            }
            
            // Success - ready to connect
            return true;
        }
        
        /// <summary>
        /// Client connection err
        /// </summary>
        /// <param name="_transportErr"></param>
        /// <param name="_extraInfo">This is a complete guess of what it is; we just know it's a string</param>
        private void onMirrorClientError(TransportError _transportErr, string _extraInfo)
        {
            Debug.LogError("[HathoraMirrorClient] onMirrorClientError: " +
                           $"_transportErr: {_transportErr}, " +
                           $"_extraInfo: {_extraInfo}");

            base.IsConnecting = false;
        }
        #endregion // Callbacks
    }
}

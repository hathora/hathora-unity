// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;
using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using kcp2k;
using Mirror;
using UnityEngine;

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
        protected override void OnAwake()
        {
            base.OnAwake();
            setSingleton();
        }

        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraMirrorClient]**ERR @ setSingleton: Destroying dupe");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }
        
        protected override void OnStart()
        {
            base.InitOnStart(HathoraMirrorClientMgrUi.Singleton);
            base.OnStart();

            // This is a Client manager script; listen for relative events
            transport.OnClientConnected += base.OnConnectSuccess;
            
            transport.OnClientDisconnected += () => 
                base.OnConnectFailed("Disconnected");;
        }
        #endregion // Init
        

        #region Interactions from UI
        public override void StartServer() => 
            NetworkManager.singleton.StartServer();

        public override void StartClient() => 
            NetworkManager.singleton.StartClient();

        public override void StopHost() =>
            NetworkManager.singleton.StopHost();

        public override void StopServer() => 
            NetworkManager.singleton.StopServer();

        public override void StopClient() => 
            NetworkManager.singleton.StopClient();
        
        /// <summary>
        /// Connect to the Server as a Client via net code. Uses cached vals.
        /// Currently uses Mirror.Kcp (UDP) transport.
        /// </summary>
        /// <returns>isSuccess</returns>
        public bool ConnectAsClient()
        {
            Debug.Log("[HathoraMirrorClient] ConnectAsync (expecting `Kcp` transport)");

            // Set connecting state + log where we're connecting to
            base.SetConnectingState(transport.name);
            
            // -----------------
            // Validate; UI and err handling is handled within
            bool isReadyToConnect = ValidateIsReadyToConnect();
            if (!isReadyToConnect)
                return false; // !isSuccess

            // -----------------
            // via NetworkManager: set transport port -> Set Host (ip)
            ExposedPort connectInfo = HathoraClientSession.ServerConnectionInfo.ExposedPort;
            NetworkManager.singleton.networkAddress = connectInfo.Host;
            
            StartClient();
            
            // TODO: How to validate success? Is this a synchronous connect?
            if (!isConnected)
            {
                OnConnectFailed("StartConnection !isSuccess");
                return false;
            }
            
            return true; // isSuccess => Continued at OnClientConnected()
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
        #endregion // Callbacks
    }
}

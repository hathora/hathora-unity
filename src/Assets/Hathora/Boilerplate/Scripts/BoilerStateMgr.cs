// Created by dylan@hathora.dev

using Hathora.Demos.Shared.Scripts.Common;
using UnityEngine;

namespace Hathora.Demos.Boilerplate.Scripts
{
    /// <summary>
    /// Acts as the liason between NetworkManager and HathoraClientMgr.
    /// - Base contains funcs like: StartServer, StartClient, StartHost.
    /// - Base contains events like: OnClientStarted, OnClientStopped.
    /// - Base tracks `ClientState` like: Stopped, Starting, Started.
    /// </summary>
    public class BoilerStateMgr : NetworkMgrStateTracker
    {
        #region vars
        /// <summary>
        /// `New` keyword overrides base Singleton when accessing child directly.
        /// </summary>
        public new static BoilerStateMgr Singleton { get; private set; }
        
        // TODO: You may want to add a shortcut to your NetworkManager instance, or Transport instance(s).  
        #endregion // vars

        
        #region Init
        /// <summary>Set Singleton instance</summary>
        protected override void Awake()
        {
            base.Awake(); // Sets base singleton
            setSingleton();
        }

        /// <summary>Subscribe to NetworkManager Client state changes</summary>
        protected override void Start()
        {
            base.Start();
            subToNetworkManagerStateEvents();
        }

        /// <summary>Catch events like OnNetworkStart and handle them below.</summary>
        private void subToNetworkManagerStateEvents()
        {
            // TODO: Sub to NetworkManager events. For example, trigger UI txt to say "Started!" when Client connected
            
            // NetworkManager.Singleton.OnSomeEvent += OnSomeEvent;
            
            // TODO: ^ Add the opposite (-=) at OnDestroy() to dispose
        }

        /// <summary>Allow this script to be called from anywhere.</summary>
        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError($"[{nameof(BoilerStateMgr)}]**ERR @ " +
                    $"{nameof(setSingleton)}: Destroying dupe");
                
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }
        #endregion // Init
        
        
        #region NetworkManager Server
        /// <summary>Starts a NetworkManager local Server.</summary>
        public void StartServer()
        {
            // NetworkManager.Singleton.StartServer(); // TODO   
        }

        /// <summary>Stops a NetworkManager local Server.</summary>
        public void StopServer()
        {
            // NetworkManager.Singleton.StopServer(); // TODO
        }
        #endregion // NetworkManager Server
        
        
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
            // Wrong overload?
            if (string.IsNullOrEmpty(_hostPort))
                return StartClient();
            
            string logPrefix = $"[{nameof(BoilerStateMgr)}] {nameof(StartClient)}]"; 
            Debug.Log($"{logPrefix} Start");
            
            // Validate host:prot
            (string hostNameOrIp, ushort port) hostPortContainer = SplitPortFromHostOrIp(_hostPort);
            bool hasHost = !string.IsNullOrEmpty(hostPortContainer.hostNameOrIp);
            bool hasPort = hostPortContainer.port > 0;

            // Start NetworkManager Client via selected Transport
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

                // NetworkManager.Singleton.SomeTransport.SetClientAddress(hostPortContainer.hostNameOrIp); // TODO
                // NetworkManager.Singleton.SomeTransport.SetPort(hostPortContainer.hostNameOrIp); // TODO
            }
            
            return StartClient();
        }
        
        /// <summary>
        /// Connect to the NetworkManager Server as a NetworkManager Client using NetworkManager host:ip.
        /// This will trigger `OnClientConnecting()` related events.
        ///
        /// TRANSPORT VALIDATION:
        /// - WebGL: Asserts for `Bayou` as the NetworkManager's selected transport
        /// - !WebGL: Asserts for `!Bayou` as the NetworkManager's selected transport (such as `Tugboat` UDP)
        /// </summary>
        /// <returns>
        /// startedConnection; to *attempt* the connection (isValid pre-connect vals); we're not connected yet.
        /// </returns>
        public bool StartClient()
        {
            string logPrefix = $"[{nameof(BoilerStateMgr)}.{nameof(StartClient)}";
            Debug.Log($"{logPrefix} Start");
            
            // Validate
            bool isReadyToConnect = validateIsReadyToConnect();
            if (!isReadyToConnect)
                return false; // !startedConnection
            
            #region TODO: Log the NetworkManager transport - safe to delete if you don't need to log this
            // // MyNetworkManagerTransport transport = NetworkManager.Singleton.Transport; // TODO
            // string transportName = transport.GetType().Name;
            // Debug.Log($"[{logPrefix} Connecting to {transport.GetClientAddress()}:" +
            //     $"{transport.GetPort()}` via `{transportName}` transport");
            #endregion
            
            base.OnClientConnecting(); // => callback @ OnClientConected() || OnStartClientFail()
            bool startedConnection = false; // NetworkManager.Singleton.StartClient(); // TODO
            return startedConnection;
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
        public void StopClient()
        {
            // NetworkManager.Singleton.StopClient(); // TODO
        }

        /// <summary>We're about to connect to a server as a Client - ensure we're ready.</summary>
        /// <returns>isValid</returns>
        private bool validateIsReadyToConnect()
        {
            Debug.Log($"[{nameof(BoilerStateMgr)}] {nameof(validateIsReadyToConnect)}");

            // TODO: Validate with your NetworkManager you're ready to connect.
            // - eg: Ensure a connection hasn't already started
            // - eg: Ensure the correct Transport is selected (if using multiple)
            return true;
        }
        #endregion // NetworkManager Client
 

        private void OnDestroy()
        {
            //// TODO: Cleanup the events you subbed to at subToNetworkManagerStateEvents
            // NetworkManager.Singleton.OnSomeEvent -= OnSomeEvent;
        }
    }
}

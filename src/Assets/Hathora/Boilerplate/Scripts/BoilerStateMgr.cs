// Created by dylan@hathora.dev

using System;
using Hathora.Core.Scripts.Runtime.Client;
using HathoraCloud.Models.Shared;
using UnityEngine;

namespace HathoraBoiler.Scripts
{
    /// <summary>
    /// Acts as the liason between NetworkManager and HathoraClientMgr.
    /// - Contains funcs like: StartServer, StartClient, StartHost.
    /// - Contains events like: OnClientStarted, OnClientStopped.
    /// - Tracks `ClientState` like: Stopped, Starting, Started.
    /// </summary>
    public class BoilerStateMgr : MonoBehaviour
    {
        #region Core vars
        [SerializeField, Tooltip("Log all client state changes")]
        private bool verboseLogs = true;
        public bool VerboseLogs => verboseLogs;

        /// <summary>When a connection stops, errs out or we get d/c'd, what should we show to clients?</summary>
        protected const string CONNECTION_STOPPED_FRIENDLY_STR = "Connection Stopped";
        
        public static BoilerStateMgr Singleton { get; private set; }
        
        // TODO: You may want to add a shortcut to your NetworkManager instance, or Transport instance(s).  
        #endregion // Core vars

        
        #region Client Events
        /// <summary>Event triggers when a NetworkManager client is attempting to connect.</summary>
        public static event Action OnClientConnectingEvent;
        
        /// <summary>Event triggers when a NetworkManager client is connected and starting (but not yet started).</summary>
        public static event Action OnClientStartingEvent;
        
        /// <summary>Event triggers when a NetworkManager client started.</summary>
        /// <returns>roomId, friendlyRegion</returns>
        public static event Action OnClientStartedEvent;

        /// <summary>Event triggers when a NetworkManager client stopped (or disconnected).</summary>
        public static Action OnClientStoppedEvent;
        
        /// <summary>Event triggers when a NetworkManager client failed to start (with UI-friendly err).</summary>
        /// <returns>friendlyReason</returns>
        public static event Action<string> OnStartClientFailEvent;
        #endregion // Client Events
        
        
        #region Init
        /// <summary>Set Singleton instance</summary>
        private void Awake() =>
            setSingleton();

        /// <summary>Subscribe to NetworkManager Client state changes</summary>
        private void Start() =>
            subToNetworkManagerStateEvents();

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
        
        
        #region Client State
        /// <summary>Client state breadcrumbs</summary>
        public enum ClientTrackedState
        {
            /// <summary>Disconnected or failed to connect</summary>
            Stopped,
            
            /// <summary>We're just about to attempt to connect</summary>
            Connecting,

            /// <summary>Connected, but not yet started</summary>
            Starting,

            /// <summary>Connected and started</summary>
            Started,
        }
        
        /// <summary>What's our status?</summary>
        protected ClientTrackedState ClientState { get; set; }

        protected virtual void OnClientConnecting()
        {
            if (verboseLogs)
                Debug.Log($"[{nameof(BoilerStateMgr)}] {nameof(OnClientConnecting)}");
            
            ClientState = ClientTrackedState.Connecting;
            OnClientConnectingEvent?.Invoke(); 
        }
        
        /// <summary>
        /// We are already connected: We are starting, but not yet ready (!started).
        /// OnClientConnectionState() success callback when state == Started. Before this:
        /// - We already called StartClient() || StartClientToLastCachedRoom()
        /// - IsConnectingAsClient == true
        /// </summary>
        protected virtual void OnClientStarting()
        {
            if (verboseLogs)
                Debug.Log($"[{nameof(BoilerStateMgr)}] {nameof(OnClientStarting)}");
            
            ClientState = ClientTrackedState.Starting;
            OnClientStartingEvent?.Invoke();
        }

        /// <summary>We just started and can now run net code</summary>
        protected virtual void OnClientStarted()
        {
            if (verboseLogs)
                Debug.Log($"[{nameof(BoilerStateMgr)}] {nameof(OnClientStarted)}");

            ClientState = ClientTrackedState.Started;
            OnClientStartedEvent?.Invoke();
        }

        /// <summary>We were disconnected from net code</summary>
        protected virtual void OnClientStopped()
        {
            if (verboseLogs)
                Debug.Log($"[{nameof(BoilerStateMgr)}] {nameof(OnClientStopped)}");
            
            ClientState = ClientTrackedState.Stopped;
            OnClientStoppedEvent?.Invoke();
        }
        
        /// <summary>
        /// Tried to connect to a Server as a Client, but failed.
        /// Triggers OnStartClientFailEvent -> OnClientStopped().
        /// </summary>
        /// <param name="_friendlyReason"></param>
        protected virtual void OnStartClientFail(string _friendlyReason)
        {
            if (verboseLogs)
                Debug.Log($"[{nameof(BoilerStateMgr)}.{nameof(OnClientStopped)}] {_friendlyReason}");
            
            OnStartClientFailEvent?.Invoke(_friendlyReason);
            OnClientStopped();
        }
        #endregion // Client State
        
        
        #region Hathora
        /// <summary>
        /// Get the last queried "host:port" from a Hathora Client session.
        /// - From `HathoraClientSession.ServerConnectionInfo.ExposedPort`.
        /// </summary>
        public string GetHathoraSessionHostPort()
        {
            ExposedPort connectInfo = HathoraClientSession.Singleton.ServerConnectionInfo.ExposedPort;

            string hostPort = $"{connectInfo.Host}:{connectInfo.Port}";
            return hostPort;
        }
        #endregion Hathora
        
        
        #region Utils
        /// <summary>
        /// This was likely passed in from the UI to override the default
        /// NetworkManager (often from Standalone Client). Eg:
        /// "1.proxy.hathora.dev:12345" -> "1.proxy.hathora.dev", 12345
        /// </summary>
        /// <param name="_hostPort"></param>
        /// <returns></returns>
        protected static (string hostNameOrIp, ushort port) SplitPortFromHostOrIp(string _hostPort)
        {
            if (string.IsNullOrEmpty(_hostPort))
                return default;
            
            string[] hostPortArr = _hostPort.Split(':');
            string hostNameOrIp = hostPortArr[0];
            ushort port = ushort.Parse(hostPortArr[1]);
            
            return (hostNameOrIp, port);
        }
        #endregion // Utils
        
        
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
            
            OnClientConnecting(); // => callback @ OnClientConected() || OnStartClientFail()
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

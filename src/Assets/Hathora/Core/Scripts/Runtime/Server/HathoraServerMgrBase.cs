// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Common.Utils;
using Hathora.Core.Scripts.Runtime.Server.ApiWrapper;
using Hathora.Core.Scripts.Runtime.Server.Models;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Server
{
    /// <summary>
    /// Inits and centralizes all Hathora Server [runtime] API wrappers.
    /// (!) If you are inheritting this, child should likely have setSingleton().
    ///     (CTRL+F here for template)
    /// 
    /// Unlike HathoraClientMgrBase, we don't need a parent since Server is lower-level
    /// than Client (eg: No UI, Session or net code specific to a platform).
    /// 
    /// (!) Unlike HathoraClientMgr, there's no current need to be abstract,
    /// although you're encouraged to create a child for your own project. 
    /// </summary>
    public class HathoraServerMgrBase : MonoBehaviour
    {
        #region Vars
        public static HathoraServerMgrBase Singleton { get; private set; }
        
        /// <summary>Set null/empty to !fake a procId in the Editor</summary>
        [SerializeField, Tooltip("When in the Editor, we'll get this Hathora ProcessInfo " +
             "as if deployed on Hathora; useful for debugging")]
        private string debugEditorMockProcId;
        protected string DebugEditorMockProcId => debugEditorMockProcId;
        
        [Header("(!) Top menu: Hathora/ServerConfigFinder")]
        [SerializeField]
        private HathoraServerConfig hathoraServerConfig;
        public HathoraServerConfig HathoraServerConfig
        {
            get {
				#if !UNITY_SERVER && !UNITY_EDITOR
				Debug.LogError("[HathoraServerMgr] (!) Tried to get hathoraServerConfig " +
                    "from Server when NOT a <server || editor>");
				return null;
				#endif // !UNITY_SERVER && !UNITY_EDITOR

                if (hathoraServerConfig == null)
                {
                    Debug.LogError("[HathoraServerMgr.hathoraServerConfig.get] HathoraServerMgr exists, " +
                        "but !HathoraServerConfig -- Did you forget to serialize a config into your scene?");
                }

                return hathoraServerConfig;
            }
        }
        
        private ServerApiContainer serverApis;
        
        /// <summary>
        /// Get the Hathora Server SDK API wrappers for all wrapped Server APIs.
        /// (!) There may be high-level variants of the calls here; check 1st!
        /// </summary>
        public ServerApiContainer ServerApis => serverApis;

        /// <summary>(!) This is set async on Awake; check for null</summary>
        private volatile HathoraServerContext serverContext;
        public HathoraServerContext ServerContext => serverContext;
        
        /// <summary>Set @ Awake, and only if deployed on Hathora</summary>
        private string hathoraProcessIdEnvVar;
        
        private bool hasHathoraProcessIdEnvVar =>
            !string.IsNullOrEmpty(hathoraProcessIdEnvVar);
        
        public static event Action<HathoraServerContext> OnInitializedEvent;
        #endregion // Vars

        
        #region Init
        protected virtual async void Awake()
        {
#if !UNITY_SERVER && !UNITY_EDITOR
            Debug.Log("(!) [HathoraServerMgrBase.Awake] Destroying - not a server");
            Destroy(this);
            return;
#endif
            

            Debug.Log("[HathoraServerMgrBase] Awake");
            SetSingleton();

            // Unlike Client calls, we can init immediately @ Awake
            InitApis(_hathoraSdkConfig: null); // Base will create this

#if (UNITY_EDITOR)
            // Optional mocked ID for debugging: Create a Room manually in Hathora console => paste ProcessId @ debugEditorMockProcId
            hathoraProcessIdEnvVar = getServerDeployedProcessId(debugEditorMockProcId);
#else
            serverDeployedProcessId = getServerDeployedProcessId();
#endif
            
            _ = GetHathoraServerContext(_throwErrIfNoLobby: false); // !await; sets `HathoraServerContext ServerContext` ^
        }
        
        /// <summary>If we were not server || editor, we'd already be destroyed @ Awake</summary>
        protected virtual void Start()
        {
        }

        /// <param name="_overrideProcIdVal">Mock a val for testing within the Editor</param>
        protected virtual string getServerDeployedProcessId(string _overrideProcIdVal = null)
        {
            if (!string.IsNullOrEmpty(_overrideProcIdVal))
            {
                Debug.Log("[HathoraServerMgrBase.getServerDeployedProcessId] (!) Overriding " +
                    $"HATHORA_PROCESS_ID with mock val: `{_overrideProcIdVal}`");

                return _overrideProcIdVal;
            }
            return Environment.GetEnvironmentVariable("HATHORA_PROCESS_ID");
        }

        /// <summary>
        /// Set a singleton instance - we'll only ever have one serverMgr.
        /// Children probably want to override this and, additionally, add a Child singleton
        /// </summary>
        protected virtual void SetSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraServerMgrBase.SetSingleton] Error: " +
                    "setSingleton: Destroying dupe");
                
                Destroy(gameObject);
                return;
            }
            
            Singleton = this;
        }

        protected virtual bool ValidateReqs()
        {
            string logPrefix = $"[HathoraServerMgrBase{nameof(ValidateReqs)}]";
            
            if (hathoraServerConfig == null)
            {
#if UNITY_SERVER
                Debug.LogError($"{logPrefix} !HathoraServerConfig: " +
                    $"Serialize to {gameObject.name}.{nameof(HathoraServerMgrBase)} (if you want " +
                    "server runtime calls from Server standalone || Editor)");
                return false;
#elif UNITY_EDITOR
                Debug.Log($"<color=orange>(!)</color> {logPrefix} !HathoraServerConfig: Np in Editor, " +
                    "but if you want server runtime calls when you build as UNITY_SERVER, " +
                    $"serialize {gameObject.name}.{nameof(HathoraServerMgrBase)}");

#else
                // We're probably a Client - just silently stop this. Clients don't have a dev key.
#endif
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Call this when you 1st know well run Server runtime events.
        /// Init all Server [runtime] API wrappers. Passes serialized HathoraServerConfig.
        /// (!) Unlike ClientMgr that are Mono-derived, we init via Constructor instead of Init().
        /// </summary>
        /// <param name="_hathoraSdkConfig">We'll automatically create this, if empty</param>
        protected virtual void InitApis(Configuration _hathoraSdkConfig = null)
        {
            if (!ValidateReqs())
                return;
            
            serverApis.ServerAppApi = new HathoraServerAppApi(hathoraServerConfig, _hathoraSdkConfig);
            serverApis.ServerLobbyApi = new HathoraServerLobbyApi(hathoraServerConfig, _hathoraSdkConfig);
            serverApis.ServerProcessApi = new HathoraServerProcessApi(hathoraServerConfig, _hathoraSdkConfig);
            serverApis.ServerRoomApi = new HathoraServerRoomApi(hathoraServerConfig, _hathoraSdkConfig);
        }
        #endregion // Init
        
        
        #region Chained API calls outside Init
        /// <summary>
        /// If Deployed (not localhost), get HathoraServerContext: { Room, Process, [Lobby], utils }.
        /// - Servers deployed in Hathora will have a special env var containing the ProcessId (HATHORA_PROCESS_ID).
        /// - If env var exists, we're deployed in Hathora.
        /// - Note the GetLobbyInitConfig() util: Parse this `object` to your own model.
        /// - Calls automatically @ Awake => triggers `OnInitializedEvent` on success.
        /// </summary>
        /// <param name="_throwErrIfNoLobby"></param>
        /// <param name="_cancelToken"></param>
        /// <returns>Triggers `OnInitializedEvent` event on success</returns>
        public async Task<HathoraServerContext> GetHathoraServerContext(
            bool _throwErrIfNoLobby,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[HathoraServerMgrBase] {nameof(GetHathoraServerContext)}";
            Debug.Log($"{logPrefix} Start");

            if (!hasHathoraProcessIdEnvVar)
            {
                #if UNITY_SERVER
                Debug.LogError($"{logPrefix} !serverDeployedProcessId; ensure: " +
                    "(1) HathoraServerConfig is serialized to a scene HathoraServerMgr, " +
                    "(2) We're deployed to Hathora (if testing locally, ignore this err)");
                #endif // UNITY_SERVER
                
                return null;
            }
            
            serverContext = new HathoraServerContext(hathoraProcessIdEnvVar);
            
            // ----------------
            // Get Process from env var "HATHORA_PROCESS_ID" => We probably cached this, already, @ )
            // We await => just in case we called this early, to prevent race conditions
            Process processInfo = await ServerApis.ServerProcessApi.GetProcessInfoAsync(
                hathoraProcessIdEnvVar, 
                _cancelToken);
            
            string procId = processInfo.ProcessId;
            if (string.IsNullOrEmpty(procId) || _cancelToken.IsCancellationRequested)
                return null;
            
            serverContext.ProcessInfo = processInfo;

            // ----------------
            // Get all active Rooms by ProcessId =>
            List<PickRoomExcludeKeyofRoomAllocations> activeRooms =
                await ServerApis.ServerRoomApi.GetActiveRoomsForProcessAsync(procId, _cancelToken);

            // Get 1st Room -> validate
            PickRoomExcludeKeyofRoomAllocations firstActiveRoom = activeRooms?.FirstOrDefault();
            if (firstActiveRoom == null || _cancelToken.IsCancellationRequested)
            {
                Debug.LogError(_cancelToken.IsCancellationRequested ? "Cancelled" : "!firstActiveRoom");
                return null;
            }

            serverContext.ActiveRoomsForProcess = activeRooms;
			
            // ----------------
            // We have Room info, but we need Lobby: Get from RoomId =>
            Lobby lobby = await ServerApis.ServerLobbyApi.GetLobbyInfoAsync(
                firstActiveRoom.RoomId,
                _cancelToken);

            if (_cancelToken.IsCancellationRequested)
            {
                Debug.LogError("Cancelled");
                return null;
            }
            if (lobby == null && _throwErrIfNoLobby)
            {
                Debug.LogError("!lobby");
                return null;
            }
            
            serverContext.Lobby = lobby;

            // Done
            OnInitializedEvent?.Invoke(serverContext);
            return serverContext;
        }
        #endregion // Chained API calls outside Init
    }
}

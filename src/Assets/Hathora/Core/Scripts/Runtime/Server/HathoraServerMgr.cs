// Created by dylan@hathora.dev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Common.ApiWrapper;
using Hathora.Core.Scripts.Runtime.Server.ApiWrapper;
using Hathora.Core.Scripts.Runtime.Server.Models;
using HathoraCloud;
using HathoraCloud.Models.Operations;
using HathoraCloud.Models.Shared;
using UnityEngine;
using Security = HathoraCloud.Models.Shared.Security;

namespace Hathora.Core.Scripts.Runtime.Server
{
    /// <summary>
    /// Inits and centralizes all Hathora Server [runtime] API wrappers.
    /// - This is the entry point to call Hathora SDK: Auth, process, rooms, etc.
    /// - Opposed to the SDK itself, this gracefully wraps around it with callbacks + events.
    /// - Ready to be inheritted with protected virtual members, should you want to!
    /// </summary>
    public class HathoraServerMgr : MonoBehaviour
    {
        #region Vars
        public static HathoraServerMgr Singleton { get; private set; }
        
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
                string logPrefix = $"[{nameof(HathoraServerMgr)}.{nameof(hathoraServerConfig)}.get]";
                
				#if !UNITY_SERVER && !UNITY_EDITOR
				Debug.LogError("[HathoraServerMgr] (!) Tried to get hathoraServerConfig " +
                    "from Server when NOT a <server || editor>");
				return null;
				#endif // !UNITY_SERVER && !UNITY_EDITOR

                if (hathoraServerConfig == null)
                {
                    Debug.LogError($"{logPrefix} HathoraServerMgr exists, " +
                        "but !HathoraServerConfig -- Did you forget to serialize a config into your scene?");
                }

                return hathoraServerConfig;
            }
        }
        
        /// <summary>
        /// Container for Hathora Client/Common APIs.
        /// (!) First check `this` for wrappers that may contain a chain of API calls for common uses.
        ///   - For example, GetServerContext() gets: ProcessId, Process, Room, ConnectionInfo, [Lobby].
        /// </summary>
        protected ServerApiContainer Apis { get; private set; }
        
        /// <summary>Direct SDK access: Inits with info from `HathoraServerConfig`</summary>
        public HathoraCloudSDK HathoraSdk { get; private set; }

        /// <summary>(!) This is set async on Awake; check for null</summary>
        private volatile HathoraServerContext serverContext;
        
        /// <summary>Set @ Awake, and only if deployed on Hathora</summary>
        private string hathoraProcessIdEnvVar;
        
        /// <summary>
        /// This will only be true if we're deployed on Hathora, by verifying
        /// a special env var ("HATHORA_PROCESS_ID").
        /// </summary>
        public bool IsDeployedOnHathora =>
            !string.IsNullOrEmpty(hathoraProcessIdEnvVar);
        
        public static event Action<HathoraServerContext> OnInitializedEvent;
        #endregion // Vars

        
        #region Init
        protected virtual async void Awake()
        {
#if !UNITY_SERVER && !UNITY_EDITOR
            Debug.Log("(!) [HathoraServerMgr.Awake] Destroying - not a server");
            Destroy(this);
            return;
#endif
            
            
            Debug.Log($"[{nameof(HathoraServerMgr)}] Awake");
            
            setSingleton();
            initHathoraSdk();
            InitApiWrappers();
            

#if (UNITY_EDITOR)
            // Optional mocked ID for debugging: Create a Room manually in Hathora console => paste ProcessId @ debugEditorMockProcId
            hathoraProcessIdEnvVar = getServerDeployedProcessId(debugEditorMockProcId);
#else
            hathoraProcessIdEnvVar = getServerDeployedProcessId();
#endif
            
            _ = GetHathoraServerContextAsync(_throwErrIfNoLobby: false); // !await; sets `HathoraServerContext ServerContext` ^
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
                Debug.Log($"[{nameof(HathoraServerMgr)}.{nameof(getServerDeployedProcessId)}] " +
                    $"(!) Overriding HATHORA_PROCESS_ID with mock val: `{_overrideProcIdVal}`");

                return _overrideProcIdVal;
            }
            return Environment.GetEnvironmentVariable("HATHORA_PROCESS_ID");
        }

        /// <summary>
        /// Set a singleton instance - we'll only ever have one serverMgr.
        /// Children probably want to override this and, additionally, add a Child singleton
        /// </summary>
        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError($"[{nameof(HathoraServerMgr)}.{nameof(setSingleton)}] " +
                    "Error: Destroying dupe");
                
                Destroy(gameObject);
                return;
            }
            
            Singleton = this;
        }
        
        private void initHathoraSdk()
        {
            if (!CanInitSdk())
                return;
            
            Security security = new()
            {
                HathoraDevToken = hathoraServerConfig.HathoraCoreOpts.DevAuthOpts.HathoraDevToken,
            };
            
            this.HathoraSdk = new HathoraCloudSDK(security, hathoraServerConfig.HathoraCoreOpts.AppId);
        }

        /// <returns>isValid</returns>
        protected virtual bool CanInitSdk()
        {
            string logPrefix = $"[{nameof(HathoraServerMgr)}.{nameof(CanInitSdk)}]";
            
            if (hathoraServerConfig == null)
            {
#if UNITY_SERVER
                Debug.LogError($"{logPrefix} !HathoraServerConfig: " +
                    $"Serialize to {gameObject.name}.{nameof(HathoraServerMgr)} to "+
                    "call Server + Common API calls");
                return false;
#elif UNITY_EDITOR
                Debug.Log($"<color=orange>(!)</color> {logPrefix} !HathoraServerConfig: Np in Editor, " +
                    "but if you want server runtime calls when you build as UNITY_SERVER, " +
                    $"serialize {gameObject.name}.{nameof(HathoraServerMgr)}");

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
        protected virtual void InitApiWrappers()
        {
            if (!CanInitSdk())
                return;

            Apis = new ServerApiContainer(HathoraSdk, hathoraServerConfig);
        }
        #endregion // Init
        
        
        #region ServerContext Getters
        /// <summary>
        /// Set @ Awake async, chained through 3 API calls - async to prevent race conditions.
        /// - If UNITY_SERVER: While !null, delay 0.1s until !null
        /// - If !UNITY_SERVER: Return cached null - this is only for deployed Hathora servers
        /// - Timeout after 10s
        /// </summary>
        /// <returns></returns>
        public async Task<HathoraServerContext> GetCachedServerContextAsync(
            CancellationToken _cancelToken = default)
        {
#if !UNITY_SERVER
            bool isMockTesting = !string.IsNullOrEmpty(debugEditorMockProcId);
            if (!isMockTesting)
                return null; // For headless servers deployed on Hathora only (and !debugEditorMockProcId)
#endif
            
            using CancellationTokenSource cts = new();
            cts.CancelAfter(TimeSpan.FromSeconds(10));
            
            if (serverContext != null)
                return serverContext;

            // We're probably still gathering the data => await for up to 10s
            string logPrefix = $"[{nameof(HathoraServerMgr)}.{nameof(GetCachedServerContextAsync)}]";
            Debug.Log($"{logPrefix} <color=orange>(!)</color> serverContext == null: " +
                "Awaiting up to 10s for val set async");
            
            return await waitForServerContextAsync(cts.Token);
        }
        
        /// <summary>
        /// [Coroutine alternative to async/await] Set @ Awake async, chained through 3 API calls -
        /// Async to prevent race conditions.
        /// - If UNITY_SERVER: While !null, delay 0.1s until !null
        /// - If !UNITY_SERVER: Return null - this is only for deployed Hathora servers
        /// - Timeout after 10s
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetCachedServerContextCoroutine(Action<HathoraServerContext> _callback)
        {
            Task<HathoraServerContext> task = GetCachedServerContextAsync();

            // Wait until the task is completed
            while (!task.IsCompleted)
                yield return null; // Wait for the next frame

            // Handle any exceptions that were thrown by the task
            if (task.IsFaulted)
            {
                string logPrefix = $"[{nameof(HathoraServerMgr)}.{nameof(GetCachedServerContextCoroutine)}]";
                Debug.LogError($"{logPrefix} An error occurred while getting the server context: {task.Exception}");
            }
            else
            {
                // Retrieve the result and invoke the callback
                HathoraServerContext result = task.Result;
                _callback?.Invoke(result);
            }
        }

        private async Task<HathoraServerContext> waitForServerContextAsync(CancellationToken _cancelToken)
        {
            while (serverContext == null)
            {
                if (_cancelToken.IsCancellationRequested)
                {
                    string logPrefix = $"[{nameof(HathoraServerMgr)}.{nameof(GetCachedServerContextCoroutine)}]";
                    Debug.LogError($"{logPrefix} Timed out after 10s");
                    return null;
                }
                
                await Task.Delay(TimeSpan.FromSeconds(0.1), _cancelToken);
            }

            return serverContext;
        }
        #endregion // ServerContext Getters
        
        
        #region Chained API calls outside Init
        /// <summary>
        /// If Deployed (not localhost), get HathoraServerContext: { Room, Process, [Lobby], utils }.
        /// - (!) If cached info is ok, instead call `GetCachedHathoraServerContextAsync()`.
        /// - Servers deployed in Hathora will have a special env var containing the ProcessId (HATHORA_PROCESS_ID).
        /// - If env var exists, we're deployed in Hathora.
        /// - Note the result ParseRoomConfig() util: Parse this `object` to your own model.
        /// - Calls automatically @ Awake => triggers `OnInitializedEvent` on success.
        /// - Caches locally @ serverContext; public get via GetCachedServerContextAsync().
        /// </summary>
        /// <param name="_throwErrIfNoLobby">Be extra sure to try/catch this, if true</param>
        /// <param name="_cancelToken"></param>
        /// <returns>Triggers `OnInitializedEvent` event on success</returns>
        public async Task<HathoraServerContext> GetHathoraServerContextAsync(
            bool _throwErrIfNoLobby,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerMgr)}.{nameof(GetHathoraServerContextAsync)}]";
            Debug.Log($"{logPrefix} Start");

            if (!IsDeployedOnHathora)
            {
                #if UNITY_SERVER && !UNITY_EDITOR
                Debug.LogError($"{logPrefix} !serverDeployedProcessId; ensure: " +
                    "(1) HathoraServerConfig is serialized to a scene HathoraServerMgr, " +
                    "(2) We're deployed to Hathora (if testing locally, ignore this err)");
                #endif // UNITY_SERVER
                
                return null;
            }
            
            // GetCachedServerContext() will await !null, so we don't want to the main var *yet*
            HathoraServerContext tempServerContext = new HathoraServerContext(hathoraProcessIdEnvVar);
            
            // ----------------
            // Get Process from env var "HATHORA_PROCESS_ID" => We probably cached this, already, @ )
            // We await => just in case we called this early, to prevent race conditions
            Process processInfo = await Apis.ServerProcessApiWrapper.GetProcessInfoAsync(
                hathoraProcessIdEnvVar, 
                _returnNullOnStoppedProcess: true,
                _cancelToken);
            
            string procId = processInfo?.ProcessId;
            
            if (_cancelToken.IsCancellationRequested)
            {
                Debug.LogError($"{logPrefix} Cancelled - `OnInitialized` event will !trigger");
                return null;
            }
            if (string.IsNullOrEmpty(procId))
            {
                string errMsg = $"{logPrefix} !Process: Did you serialize a `HathoraServerConfig` " +
                    "to your scene's `HathoraServerMgr` (often nested in a `HathoraManager` root GameObject)?";

                // Are we debugging in the Editor? Add +info
                bool isMockDebuggingInEditor = UnityEngine.Application.isEditor && 
                    !string.IsNullOrEmpty(debugEditorMockProcId);
                if (isMockDebuggingInEditor)
                    errMsg += " <b>Since isEditor && debugEditorMockProcId, `your HathoraServerMgr.debugEditorMockProcId` " +
                        "is likely stale.</b> Create a new Room -> Update your `debugEditorMockProcId` -> try again.";
                
                Debug.LogError(errMsg);
                return null;
            }
            
            tempServerContext.ProcessInfo = processInfo;

            // ----------------
            // Get all active Rooms by ProcessId =>
            List<RoomWithoutAllocations> activeRooms = await 
                Apis.ServerRoomApiWrapper.ServerGetActiveRoomsForProcessAsync(procId, _cancelToken);

            // Get 1st Room -> validate
            RoomWithoutAllocations firstActiveRoom = activeRooms?.FirstOrDefault();
            
            if (_cancelToken.IsCancellationRequested)
            {
                Debug.LogError($"{logPrefix} Cancelled - `OnInitialized` event will !trigger");
                return null;
            }
            if (firstActiveRoom == null)
            {
                Debug.LogError($"{logPrefix} !Room");
                return null;
            }

            tempServerContext.ActiveRoomsForProcess = activeRooms;
			
            // ----------------
            // We have Room info, but we *may* need Lobby: Get from RoomId =>
            // TODO: This may soon change, where Room info may include Lobby info.
            // TODO: When implemented, remove this block (minus validation).
            LobbyV3 lobby = null;
            try
            {
                // Try catch since we may not have a Lobby, which could be ok
                lobby = await Apis.LobbyApiWrapper.GetLobbyInfoByRoomIdAsync(
                    firstActiveRoom.RoomId,
                    _cancelToken);
            }
            catch (Exception e)
            {
                // Should 404 if !Lobby, returning null
                if (_throwErrIfNoLobby)
                {
                    Debug.LogError($"Error: {e}");
                    throw;
                }
                
                Debug.Log($"{logPrefix} <b>!Lobby, but likely expected</b> " +
                    "(since !_throwErrIfNoLobby) - continuing...");
            }

            if (_cancelToken.IsCancellationRequested)
            {
                Debug.LogError("Cancelled");
                return null;
            }
            
            tempServerContext.Lobby = lobby;

            // ----------------
            // Done
            Debug.Log($"{logPrefix} Done");
            this.serverContext = tempServerContext;
            OnInitializedEvent?.Invoke(tempServerContext);
            return tempServerContext;
        }
        #endregion // Chained API calls outside Init
    }
}

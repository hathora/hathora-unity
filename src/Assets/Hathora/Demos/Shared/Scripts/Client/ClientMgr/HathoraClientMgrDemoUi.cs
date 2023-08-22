// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Client;
using Hathora.Core.Scripts.Runtime.Client.Config;
using Hathora.Core.Scripts.Runtime.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hathora.Demos.Shared.Scripts.Client.ClientMgr
{
    /// <summary>
    /// [SDK DEMO] Handles the non-Player UI so we can keep the logic separate.
    /// Generally, this is going to be pre-connection UI such as create/join lobbies.
    /// UI OnEvent entry points from Buttons start here.
    /// </summary>
    public abstract class HathoraClientMgrDemoUi : MonoBehaviour
    {
        #region Vars
        #region Vars -> SDK Demo UI (Unwrapped from formerly-named `HathoraClientSdkUiContainer` to workaround Unity bug)
        [FormerlySerializedAs("InvalidConfigTemplatePnl")]
        [Header("SDK Demo UI -> Help")]
        [SerializeField]
        private GameObject invalidConfigTemplatePnl;
        protected GameObject InvalidConfigTemplatePnl => invalidConfigTemplatePnl;
        
        [FormerlySerializedAs("InvalidConfigPnl")]
        [SerializeField]
        private GameObject invalidConfigPnl;
        protected GameObject InvalidConfigPnl => invalidConfigPnl;
        
        [Header("SDK Demo UI -> Auth")]
        [SerializeField]
        private Button authBtn;
        protected Button AuthBtn => authBtn;
        
        [SerializeField]
        private TextMeshProUGUI authTxt;
        protected TextMeshProUGUI AuthTxt => authTxt;

        [Header("SDK Demo UI -> Lobby (Before Exists)")]
        [SerializeField]
        private Button createLobbyBtn;
        protected Button CreateLobbyBtn => createLobbyBtn;
        
        [SerializeField]
        private Button getLobbyInfoBtn;
        protected Button GetLobbyInfoBtn => getLobbyInfoBtn;
        
        [SerializeField]
        private TMP_InputField getLobbyInfoInput;
        protected TMP_InputField GetLobbyInfoInput => getLobbyInfoInput;
        
        [Header("SDK Demo UI -> Lobby (After Exists)")]
        [SerializeField]
        private TextMeshProUGUI lobbyRoomIdTxt;
        protected TextMeshProUGUI LobbyRoomIdTxt => lobbyRoomIdTxt;
        
        [SerializeField]
        private Button viewLobbiesBtn;
        protected Button ViewLobbiesBtn => viewLobbiesBtn;
        
        [SerializeField]
        private Button copyLobbyRoomIdBtn;
        protected Button CopyLobbyRoomIdBtn => copyLobbyRoomIdBtn;
        
        [SerializeField]
        private TextMeshProUGUI copiedRoomIdFadeTxt;
        protected TextMeshProUGUI CopiedRoomIdFadeTxt => copiedRoomIdFadeTxt;
        
        [SerializeField]
        private TextMeshProUGUI createOrGetLobbyInfoErrTxt;
        protected TextMeshProUGUI CreateOrGetLobbyInfoErrTxt => createOrGetLobbyInfoErrTxt;
        
        [SerializeField]
        private TextMeshProUGUI viewLobbiesSeeLogsFadeTxt;
        protected TextMeshProUGUI ViewLobbiesSeeLogsFadeTxt => viewLobbiesSeeLogsFadeTxt;
        
        [Header("SDK Demo UI -> Room (Get Server/Connection Info)")]
        [SerializeField]
        private Button getServerInfoBtn;
        protected Button GetServerInfoBtn => getServerInfoBtn;
        
        [SerializeField]
        private TextMeshProUGUI getServerInfoTxt;
        protected TextMeshProUGUI GetServerInfoTxt => getServerInfoTxt;
        
        [SerializeField]
        private Button copyServerInfoBtn;
        protected Button CopyServerInfoBtn => copyServerInfoBtn;
        
        [SerializeField]
        private TextMeshProUGUI copiedServerInfoFadeTxt;
        protected TextMeshProUGUI CopiedServerInfoFadeTxt => copiedServerInfoFadeTxt;
        
        [SerializeField]
        private TextMeshProUGUI getServerInfoErrTxt;
        protected TextMeshProUGUI GetServerInfoErrTxt => getServerInfoErrTxt;
        
        [Header("SDK Demo UI -> NetCode Transport: Join Lobby [as Client]")]
        [SerializeField]
        private Button joinLobbyAsClientBtn;
        protected Button JoinLobbyAsClientBtn => joinLobbyAsClientBtn;
        
        [SerializeField]
        private TextMeshProUGUI joiningLobbyStatusTxt;
        protected TextMeshProUGUI JoiningLobbyStatusTxt => joiningLobbyStatusTxt;
        
        [SerializeField, Tooltip("This will show while you still see the Join button returned")]
        private TextMeshProUGUI joiningLobbyStatusErrTxt;
        protected TextMeshProUGUI JoiningLobbyStatusErrTxt => joiningLobbyStatusErrTxt;
        #endregion Vars -> // SDK Demo UI (Unwrapped from formerly-named `HathoraClientSdkUiContainer` to workaround Unity bug)

        [Header("Hello World Demo UI")]
        [SerializeField, Tooltip("Contains UI elements - like txts/btns - for Hello World demo")]
        private HathoraClientMgrHelloWorldDemoUi helloWorldDemoUi;
        protected HathoraClientMgrHelloWorldDemoUi HelloWorldDemoUi => helloWorldDemoUi;

        private HathoraClientMgrBase clientMgrBase => HathoraClientMgrBase.Singleton;
        private const float FADE_TXT_DISPLAY_DURATION_SECS = 0.5f;
        private const string HATHORA_VIOLET_COLOR_HEX = "#EEDDFF";
        private static string headerBoldColorBegin => $"<b><color={HATHORA_VIOLET_COLOR_HEX}>";
        private const string headerBoldColorEnd = "</color></b>";

        protected static HathoraClientSession HathoraClientSession => 
            HathoraClientSession.Singleton;
        #endregion // Vars
        

        #region Init
        protected virtual void Awake()
        {
            Debug.Log($"[HathoraClientMgrDemoUi.Awake] InstanceId={GetInstanceID()}");
            SetSingleton();
        }

        protected virtual void Start() =>
            subToClientMgrEvents();
        
        /// <summary>Override this and set your singleton instance</summary>
        protected abstract void SetSingleton();

        private void subToClientMgrEvents()
        {
            Debug.Log("[HathoraClientMgrDemoUI] subToClientMgrEvents");
            
            HathoraClientMgrBase.OnAuthLoginDoneEvent += OnAuthLoginDone;
            HathoraClientMgrBase.OnGetActivePublicLobbiesDoneEvent += OnGetActivePublicLobbiesDone;
            HathoraClientMgrBase.OnCreateLobbyDoneEvent += OnCreateLobbyDone;
            HathoraClientMgrBase.OnGetActiveConnectionInfoDoneEvent += OnGetActiveConnectionInfoDone;
            HathoraClientMgrBase.OnClientStoppedEvent += OnClientStopped;
            HathoraClientMgrBase.OnStartClientFailEvent += OnStartClientFail;
        }
        #endregion // Init
        
        
        #region ClientMgr Event Callbacks
        /// <summary>ClientMgr callback</summary>
        /// <param name="_isSuccess"></param>
        protected virtual void OnAuthLoginDone(bool _isSuccess)
        {
            if (_isSuccess)
                OnAuthSuccess();
            else
                OnAuthFailed();
        }

        /// <summary>ClientMgr callback</summary>
        /// <param name="_lobbies"></param>
        protected virtual async void OnGetActivePublicLobbiesDone(List<Lobby> _lobbies)
        {
            viewLobbiesSeeLogsFadeTxt.text = "See Logs";

            try
            {
                await ShowFadeTxtThenFadeAsync(viewLobbiesSeeLogsFadeTxt);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
            
            foreach (Lobby lobby in _lobbies)
            {
                Debug.Log($"[NetPlayerUI] OnViewLobbies - lobby found: " +
                    $"RoomId={lobby.RoomId}, CreatedAt={lobby.CreatedAt}, CreatedBy={lobby.CreatedBy}");
            }
            
            // TODO: Create a UI view for these servers
            viewLobbiesBtn.interactable = true;
        }

        /// <summary>ClientMgr callback</summary>
        /// <param name="_lobby"></param>
        protected virtual void OnCreateLobbyDone(Lobby _lobby)
        {
            if (_lobby == null)
                OnCreatedOrJoinedLobbyFail();
            else
            {
                onCreatedOrJoinedLobbySuccess(
                    _lobby.RoomId, 
                    _lobby.Region.ToString().SplitPascalCase());
            }
        }
        
        protected virtual void OnGetActiveConnectionInfoDone(ConnectionInfoV2 _connectionInfo)
        {
            bool hasPort = _connectionInfo?.ExposedPort?.Port > 0;
            bool hasHost = !string.IsNullOrEmpty(_connectionInfo?.ExposedPort?.Host);

            if (hasPort && hasHost)
                onGetActiveConnectionInfoSuccess(_connectionInfo);
            else
                onGetActiveConnectionInfoFail();
        }

        /// <summary>
        /// This will show the Join Lobby btn again, hide the status txt (behind the btn),
        /// then show an orange err txt below the join lobby btn.
        /// </summary>
        protected virtual void OnClientStopped()
        {
            joiningLobbyStatusTxt.gameObject.SetActive(false);
            
            joiningLobbyStatusErrTxt.text = "<color=orange>Stopped (See Logs)</color>";
            joiningLobbyStatusErrTxt.gameObject.SetActive(true);
            joinLobbyAsClientBtn.gameObject.SetActive(true);
        }

        /// <summary>Failed, after a callback from clicking a "Client" net code btn.</summary>
        /// <param name="_friendlyErr"></param>
        protected virtual void OnStartClientFail(string _friendlyErr)
        {
            Debug.Log($"[HathoraNetUiBase] OnNetStartClientFail: {_friendlyErr}");

            joiningLobbyStatusTxt.gameObject.SetActive(false);
            joinLobbyAsClientBtn.gameObject.SetActive(true);

            if (string.IsNullOrEmpty(_friendlyErr))
                return;
            
#if UNITY_WEBGL && UNITY_EDITOR
            _friendlyErr += " (Unity !supports WebSocket in Editor)";
#endif
            
            joiningLobbyStatusErrTxt.text = $"<color=orange>{_friendlyErr}</color>";
            joiningLobbyStatusErrTxt.gameObject.SetActive(true);
        }
        #endregion // ClientMgr Event Callbacks
        
        
        #region UI Interactions (BtnClicks, InputEnds)
        public virtual void OnStartServerBtnClick() { }

        /// <param name="_hostPortOverride">host:port provided by Hathora</param>
        public virtual void OnStartClientBtnClick(string _hostPortOverride = null) =>
            Debug.Log($"[HathoraNetClientMgrUiBase.OnStartClientBtnClick] " +
                $"_hostPortOverride=={_hostPortOverride} (if null, we'll get from NetworkManager)");
        
        public virtual void OnStartHostBtnClick() { }
        public virtual void OnStopServerBtnClick() { }
        public virtual void OnStopClientBtnClick() { }
        public virtual void OnStopHostBtnClick() { }

        public async void OnAuthLoginBtnClick()
        {
            if (!clientMgrBase.CheckIsValidToAuth())
            {
                OnAuthFailed("Invalid AppId");
                onInvalidClientConfig(HathoraClientMgrBase.Singleton.HathoraClientConfig);
                return;
            }
                
            setShowAuthTxt("<color=yellow>Logging in...</color>");

            try
            {
                await clientMgrBase.AuthLoginAsync(); // => Callback @ onAuthDone()
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
        }

        public async void OnCreateLobbyBtnClick()
        {
            setShowLobbyTxt("<color=yellow>Creating Lobby...</color>");

            // (!) Region Index starts at 1 (not 0) // TODO: Get from UI
            const Region _region = Region.WashingtonDC;

            try
            {
                await clientMgrBase.CreateLobbyAsync(_region); // public lobby
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
        }

        /// <summary>
        /// The player pressed ENTER || unfocused the ServerConnectionInfo input.
        /// </summary>
        public async void OnGetLobbyInfoInputEnd()
        {
            // Status update >> cache input str >> clear input txt
            setGettingLobbyInfoUi();
            string roomIdInputStr = GetLobbyInfoInputStr();
            getLobbyInfoInput.text = "";

            if (string.IsNullOrEmpty(roomIdInputStr))
            {
                // Empty? Just go back to logged in phase
                OnAuthLoginDone(_isSuccess: true);

                return;
            }

            try
            {
                await clientMgrBase.GetLobbyInfoAsync(roomIdInputStr);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
        }

        /// <summary>
        /// Btn disabled OnClick via inspector: Restore on done
        /// </summary>
        public async void OnViewLobbiesBtnClick()
        {
            viewLobbiesSeeLogsFadeTxt.text = "<color=yellow>Getting Lobbies...</color>";

            try
            {
                await ShowFadeTxtThenFadeAsync(viewLobbiesSeeLogsFadeTxt);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }

            // TODO: Get region from UI; null returns ALL regions
            Region? region = null;
            
            try
            {
                await clientMgrBase.GetActivePublicLobbiesAsync(region); // => Callback @ onRefreshActiveLobbiesDone
            }
            catch (Exception e)
            {
                viewLobbiesBtn.interactable = true;
            }
        }
        
        /// <summary>(!) Clipboard !works in webgl</summary>
        public async void OnCopyLobbyRoomIdBtnClick()
        {
            GUIUtility.systemCopyBuffer = HathoraClientSession.RoomId; // Copy to clipboard
            
            // Show + Fade
            try
            {
                await ShowFadeTxtThenFadeAsync(copiedRoomIdFadeTxt);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");

                throw;
            }
        }

        /// <summary>
        /// We should only call this if we already have the lobby info (ServerConnectionInfo).
        /// </summary>
        public async void OnGetServerInfoBtnClick()
        {
            setServerInfoTxt("<color=yellow>Getting server connection info...</color>");
            
            // The ServerConnectionInfo should already be cached
            try
            {
                // TODO: While we await this, update status text to append a "." with StringBuilder every second !ready (await status)
                await clientMgrBase.GetActiveConnectionInfo(HathoraClientSession.RoomId);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
        }
        
        /// <summary>Copies as "ip:port". (!) Clipboard !works in webgl</summary>
        public async void OnCopyServerInfoBtnClick()
        {
            string serverInfo = HathoraClientSession.GetServerInfoIpPort(); // "ip:port"
            GUIUtility.systemCopyBuffer = serverInfo; // Copy to clipboard
            
            // Show + Fade
            try
            {
                await ShowFadeTxtThenFadeAsync(copiedServerInfoFadeTxt);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
        }

        /// <summary>
        /// Sets UI only: Override to call logic
        /// TODO: Rename to OnNetJoinLobbyAsClientBtnClick()
        /// </summary>
        public virtual void OnJoinLobbyAsClientBtnClick()
        {
            Debug.Log("[HathoraNetUiBase] OnJoinLobbyAsClientBtnClick");

            joinLobbyAsClientBtn.gameObject.SetActive(false);
            joiningLobbyStatusErrTxt.gameObject.SetActive(false);
            
            joiningLobbyStatusTxt.text = "<color=yellow>Joining Lobby...</color>";
            joiningLobbyStatusTxt.gameObject.SetActive(true);
        }
        #endregion // UI Interactions (BtnClicks, InputEnds)
        
        
        #region UI Utils (!Called directly from UI)
        /// <summary>
        /// (!) Don't reset roomIdInputStr.text here
        /// </summary>
        private void setGettingLobbyInfoUi()
        {
            // Hide all of lobby EXCEPT the room id text
            setShowLobbyTxt("<color=yellow>Getting Lobby Info...</color>");
            setInitLobbyUi(false);
        }
        
        /// <summary>Sets the status txt next to auth login btn</summary>
        /// <param name="_authStr"></param>
        private void setShowAuthTxt(string _authStr)
        {
            authTxt.text = _authStr;
            authTxt.gameObject.SetActive(true);
        }
        
        /// <summary>Sets the status txt next to create/get lobby(s)</summary>
        /// <param name="_roomId"></param>
        private void setShowLobbyTxt(string _roomId)
        {
            lobbyRoomIdTxt.text = _roomId;
            lobbyRoomIdTxt.gameObject.SetActive(true);
        }

        private void setShowCreateOrJoinLobbyErrTxt(string friendlyErrStr)
        {
            createOrGetLobbyInfoErrTxt.text = friendlyErrStr;
            createOrGetLobbyInfoErrTxt.gameObject.SetActive(true);
        }

        private void setGetServerInfoErrTxt(string friendlyErrStr)
        {
            getServerInfoErrTxt.text = friendlyErrStr;
            getServerInfoErrTxt.gameObject.SetActive(true);
        }

        private void setServerInfoTxt(string serverInfo)
        {
            Debug.Log($"[HathoraClientMgrDemoUi] setServerInfoTxt: {serverInfo}");

            getServerInfoTxt.text = serverInfo;
            getServerInfoTxt.gameObject.SetActive(true);
        }

        /// <summary>
        /// This also resets interactable
        /// </summary>
        /// <param name="show"></param>
        private void setInitLobbyUi(bool show)
        {
            createLobbyBtn.interactable = show;
            getLobbyInfoBtn.interactable = show;
            
            createLobbyBtn.gameObject.SetActive(show);
            getLobbyInfoBtn.gameObject.SetActive(show);
            
            // On or off: If this is resetting, we'll hide it. 
            // This also hides the cancel btn
            lobbyRoomIdTxt.gameObject.SetActive(false); // Behind 'Create Lobby' btn
            getLobbyInfoInput.gameObject.SetActive(false);
            copyLobbyRoomIdBtn.gameObject.SetActive(false);
            createOrGetLobbyInfoErrTxt.gameObject.SetActive(false);
            copiedRoomIdFadeTxt.gameObject.SetActive(false);
            viewLobbiesSeeLogsFadeTxt.gameObject.SetActive(false);
        }

        private string GetLobbyInfoInputStr() =>
            getLobbyInfoInput.text.Trim();
        
        /// <summary>Show a txt -> Slowly fade out in a more-polished way</summary>
        /// <param name="fadeTxt"></param>
        protected async Task ShowFadeTxtThenFadeAsync(TextMeshProUGUI fadeTxt)
        {
            fadeTxt.gameObject.SetActive(true);
            await Task.Delay((int)(FADE_TXT_DISPLAY_DURATION_SECS * 1000));

            float elapsedTime = 0f;
            Color originalColor = fadeTxt.color;

            while (elapsedTime < FADE_TXT_DISPLAY_DURATION_SECS)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / FADE_TXT_DISPLAY_DURATION_SECS);
                fadeTxt.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                await Task.Yield();
            }

            fadeTxt.gameObject.SetActive(false);
            fadeTxt.color = originalColor;
        }
        #endregion // UI Utils (!Called directly from UI)
        

        #region Logic OnCallbacks (!Called directly from UI)
        /// <summary>We just logged in</summary>
        private void OnAuthSuccess()
        {
            setShowAuthTxt("<b>Client Logged In</b> (Anonymously)");
            setInitLobbyUi(true);
        }
        
        /// <summary>Show "Login Failed" status txt with optional +description</summary>
        /// <param name="_extraErr">Appends to "Login Failed"</param>
        private void OnAuthFailed(string _extraErr = "")
        {
            bool hasExtraErr = !string.IsNullOrEmpty(_extraErr); 
            setShowAuthTxt(hasExtraErr
                ? $"<color=orange>Login Failed: {_extraErr}</color>"
                :  "<color=orange>Login Failed</color>");
        }

        public void OnCreatedOrJoinedLobbyFail()
        {
            setInitLobbyUi(true);
            setShowCreateOrJoinLobbyErrTxt("<color=orange>Failed to Get Lobby info - see logs</color>");
        }

        private void onGetActiveConnectionInfoSuccess(ConnectionInfoV2 _connectionInfo)
        {
            Debug.Log(
                $"[HathoraNetUiBase] onGetActiveConnectionInfoSuccess: " +
                $"{HathoraClientSession.GetServerInfoIpPort()} ({_connectionInfo.ExposedPort.TransportType})");
            
            // ####################
            // ServerInfo:
            // 127.0.0.1:7777 (UDP)
            // ####################
            setServerInfoTxt($"{headerBoldColorBegin}ServerInfo{headerBoldColorEnd}:\n" +
                $"{_connectionInfo.ExposedPort.Host}<color=yellow><b>:</b></color>{_connectionInfo.ExposedPort.Port}\n" +
                $"(<color=yellow>{_connectionInfo.ExposedPort.TransportType}</color>)");
            
            copyServerInfoBtn.gameObject.SetActive(true);
            joinLobbyAsClientBtn.gameObject.SetActive(true);
        }
        
        private void onGetActiveConnectionInfoFail()
        {
            getServerInfoBtn.gameObject.SetActive(true);
            setGetServerInfoErrTxt("<color=orange>Failed to Get Server Info - see logs</color>");
        }

        private void onCreatedOrJoinedLobbySuccess(string _roomId, string _friendlyRegionStr)
        {
            // Hide all init lobby UI except the txt + view lobbies
            setInitLobbyUi(false);
            setShowLobbyTxt($"{headerBoldColorBegin}RoomId{headerBoldColorEnd}:\n{_roomId}\n\n" +
                $"{headerBoldColorBegin}Region{headerBoldColorEnd}: {_friendlyRegionStr}");

            // We can now show the lobbies and ServerConnectionInfo copy btn
            copyLobbyRoomIdBtn.gameObject.SetActive(true);
            viewLobbiesBtn.gameObject.SetActive(true);
            getServerInfoBtn.gameObject.SetActive(true);
        }

        private void onInvalidClientConfig(HathoraClientConfig _config)
        {
            if (authBtn != null)
                authBtn.gameObject.SetActive(false); // Prevent UI overlap
            
            // Core issue
            string netComponentPathFriendlyStr = " HathoraManager (GameObject)'s " +
                $"{nameof(clientMgrBase)} component";
            
            if (_config == null)
            {
                authBtn.gameObject.SetActive(false);
                invalidConfigPnl.SetActive(true);

                throw new Exception($"[{nameof(clientMgrBase)}] !{nameof(HathoraClientConfig)} - " +
                    $"Serialize one at {netComponentPathFriendlyStr}");
            }
            
            if (!_config.HasAppId)
            {
                invalidConfigPnl.SetActive(true);
                throw new Exception($"[{nameof(clientMgrBase)}] !HathoraClientConfig.AppId - " +
                    "Set one at Assets/Hathora/HathoraClientConfig. **Headless servers may ignore this**");
            }
            
            bool isTemplate = _config.name.Contains(".template");
            if (!isTemplate)
                return;
            
            authBtn.gameObject.SetActive(false);
            invalidConfigTemplatePnl.SetActive(true);
                
            throw new Exception("[HathoraNetUiBase.SetInvalidConfig] Error: " +
                "Using template Config! Create a new one via top menu `Hathora/Config Finder`");
        }
        #endregion // Logic OnCallbacks (!Called directly from UI)
        
        
        #region Cleanup
        private void UnsubToClientMgrEvents()
        {
            HathoraClientMgrBase.OnAuthLoginDoneEvent -= OnAuthLoginDone;
            HathoraClientMgrBase.OnGetActivePublicLobbiesDoneEvent -= OnGetActivePublicLobbiesDone;
            HathoraClientMgrBase.OnCreateLobbyDoneEvent -= OnCreateLobbyDone;
        }

        private void OnDestroy() => UnsubToClientMgrEvents();
        #endregion // Cleanup
    }
}

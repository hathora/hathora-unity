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
        #region Vars -> Serialized Fields
        [FormerlySerializedAs("sdkDemoUi")]
        [SerializeField]
        private HathoraClientMgrUiBaseContainer uiContainer;
        protected HathoraClientMgrUiBaseContainer UIContainer => uiContainer;
        
        [SerializeField]
        private HathoraClientMgrHelloWorldDemoUi helloWorldDemoUi;
        protected HathoraClientMgrHelloWorldDemoUi HelloWorldDemoUi => helloWorldDemoUi;
        #endregion // Vars -> Serialized Fields

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
            uiContainer.ViewLobbiesSeeLogsFadeTxt.text = "See Logs";

            try
            {
                await ShowFadeTxtThenFadeAsync(uiContainer.ViewLobbiesSeeLogsFadeTxt);
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
            uiContainer.ViewLobbiesBtn.interactable = true;
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
        
        /// <summary>Failed, after a callback from clicking a "Client" net code btn.</summary>
        /// <param name="_friendlyErr"></param>
        protected virtual void OnStartClientFail(string _friendlyErr)
        {
            Debug.Log($"[HathoraNetUiBase] OnNetStartClientFail: {_friendlyErr}");

            uiContainer.JoiningLobbyStatusTxt.gameObject.SetActive(false);
            uiContainer.JoinLobbyAsClientBtn.gameObject.SetActive(true);

            if (string.IsNullOrEmpty(_friendlyErr))
                return;
            
#if UNITY_WEBGL && UNITY_EDITOR
            _friendlyErr += " (Unity !supports WebSocket in Editor)";
#endif
            
            uiContainer.JoiningLobbyStatusErrTxt.text = $"<color=orange>{_friendlyErr}</color>";
            uiContainer.JoiningLobbyStatusErrTxt.gameObject.SetActive(true);
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
            uiContainer.GetLobbyInfoInput.text = "";

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
            uiContainer.ViewLobbiesSeeLogsFadeTxt.text = "<color=yellow>Getting Lobbies...</color>";

            try
            {
                await ShowFadeTxtThenFadeAsync(uiContainer.ViewLobbiesSeeLogsFadeTxt);
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
                uiContainer.ViewLobbiesBtn.interactable = true;
            }
        }
        
        /// <summary>(!) Clipboard !works in webgl</summary>
        public async void OnCopyLobbyRoomIdBtnClick()
        {
            GUIUtility.systemCopyBuffer = HathoraClientSession.RoomId; // Copy to clipboard
            
            // Show + Fade
            try
            {
                await ShowFadeTxtThenFadeAsync(uiContainer.CopiedRoomIdFadeTxt);
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
                await ShowFadeTxtThenFadeAsync(uiContainer.CopiedServerInfoFadeTxt);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
        }

        /// <summary>
        /// Clicked a "Client" net code btn.
        /// TODO: Rename to OnNetJoinLobbyAsClientBtnClick()
        /// </summary>
        public virtual void OnJoinLobbyAsClientBtnClick()
        {
            Debug.Log("[HathoraNetUiBase] OnJoinLobbyAsClientBtnClick");

            uiContainer.JoinLobbyAsClientBtn.gameObject.SetActive(false);
            uiContainer.JoiningLobbyStatusErrTxt.gameObject.SetActive(false);
            
            uiContainer.JoiningLobbyStatusTxt.text = "<color=yellow>Joining Lobby...</color>";
            uiContainer.JoiningLobbyStatusTxt.gameObject.SetActive(true);
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
            uiContainer.AuthTxt.text = _authStr;
            uiContainer.AuthTxt.gameObject.SetActive(true);
        }
        
        /// <summary>Sets the status txt next to create/get lobby(s)</summary>
        /// <param name="_roomId"></param>
        private void setShowLobbyTxt(string _roomId)
        {
            uiContainer.LobbyRoomIdTxt.text = _roomId;
            uiContainer.LobbyRoomIdTxt.gameObject.SetActive(true);
        }

        private void setShowCreateOrJoinLobbyErrTxt(string friendlyErrStr)
        {
            uiContainer.CreateOrGetLobbyInfoErrTxt.text = friendlyErrStr;
            uiContainer.CreateOrGetLobbyInfoErrTxt.gameObject.SetActive(true);
        }

        private void setGetServerInfoErrTxt(string friendlyErrStr)
        {
            uiContainer.GetServerInfoErrTxt.text = friendlyErrStr;
            uiContainer.GetServerInfoErrTxt.gameObject.SetActive(true);
        }

        private void setServerInfoTxt(string serverInfo)
        {
            Debug.Log($"[HathoraClientMgrDemoUi] setServerInfoTxt: {serverInfo}");

            uiContainer.GetServerInfoTxt.text = serverInfo;
            uiContainer.GetServerInfoTxt.gameObject.SetActive(true);
        }

        /// <summary>
        /// This also resets interactable
        /// </summary>
        /// <param name="show"></param>
        private void setInitLobbyUi(bool show)
        {
            uiContainer.CreateLobbyBtn.interactable = show;
            uiContainer.GetLobbyInfoBtn.interactable = show;
            
            uiContainer.CreateLobbyBtn.gameObject.SetActive(show);
            uiContainer.GetLobbyInfoBtn.gameObject.SetActive(show);
            
            // On or off: If this is resetting, we'll hide it. 
            // This also hides the cancel btn
            uiContainer.LobbyRoomIdTxt.gameObject.SetActive(false); // Behind 'Create Lobby' btn
            uiContainer.GetLobbyInfoInput.gameObject.SetActive(false);
            uiContainer.CopyLobbyRoomIdBtn.gameObject.SetActive(false);
            uiContainer.CreateOrGetLobbyInfoErrTxt.gameObject.SetActive(false);
            uiContainer.CopiedRoomIdFadeTxt.gameObject.SetActive(false);
            uiContainer.ViewLobbiesSeeLogsFadeTxt.gameObject.SetActive(false);
        }

        private string GetLobbyInfoInputStr() =>
            uiContainer.GetLobbyInfoInput.text.Trim();
        
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
            
            uiContainer.CopyServerInfoBtn.gameObject.SetActive(true);
            uiContainer.JoinLobbyAsClientBtn.gameObject.SetActive(true);
        }
        
        private void onGetActiveConnectionInfoFail()
        {
            uiContainer.GetServerInfoBtn.gameObject.SetActive(true);
            setGetServerInfoErrTxt("<color=orange>Failed to Get Server Info - see logs</color>");
        }

        private void onCreatedOrJoinedLobbySuccess(string _roomId, string _friendlyRegionStr)
        {
            // Hide all init lobby UI except the txt + view lobbies
            setInitLobbyUi(false);
            setShowLobbyTxt($"{headerBoldColorBegin}RoomId{headerBoldColorEnd}:\n{_roomId}\n\n" +
                $"{headerBoldColorBegin}Region{headerBoldColorEnd}: {_friendlyRegionStr}");

            // We can now show the lobbies and ServerConnectionInfo copy btn
            uiContainer.CopyLobbyRoomIdBtn.gameObject.SetActive(true);
            uiContainer.ViewLobbiesBtn.gameObject.SetActive(true);
            uiContainer.GetServerInfoBtn.gameObject.SetActive(true);
        }

        private void onInvalidClientConfig(HathoraClientConfig _config)
        {
            if (uiContainer.AuthBtn != null)
                uiContainer.AuthBtn.gameObject.SetActive(false); // Prevent UI overlap
            
            // Core issue
            string netComponentPathFriendlyStr = " HathoraManager (GameObject)'s " +
                $"{nameof(clientMgrBase)} component";
            
            if (_config == null)
            {
                uiContainer.AuthBtn.gameObject.SetActive(false);
                uiContainer.InvalidConfigPnl.SetActive(true);

                throw new Exception($"[{nameof(clientMgrBase)}] !{nameof(HathoraClientConfig)} - " +
                    $"Serialize one at {netComponentPathFriendlyStr}");
            }
            
            if (!_config.HasAppId)
            {
                uiContainer.InvalidConfigPnl.SetActive(true);
                throw new Exception($"[{nameof(clientMgrBase)}] !HathoraClientConfig.AppId - " +
                    "Set one at Assets/Hathora/HathoraClientConfig. **Headless servers may ignore this**");
            }
            
            bool isTemplate = _config.name.Contains(".template");
            if (!isTemplate)
                return;
            
            uiContainer.AuthBtn.gameObject.SetActive(false);
            uiContainer.InvalidConfigTemplatePnl.SetActive(true);
                
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

// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Client;
using Hathora.Core.Scripts.Runtime.Client.Config;
using Hathora.Demos._1_FishNetDemo.Scripts.Client.ClientMgr;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hathora.Demos.Shared.Scripts.Client
{
    /// <summary>
    /// Handles the non-Player UI so we can keep the logic separate.
    /// Generally, this is going to be pre-connection UI such as create/join lobbies.
    /// UI OnEvent entry points from Buttons start here.
    /// </summary>
    public class NetUI : MonoBehaviour
    {
        #region Serialized Fields
        [FormerlySerializedAs("InvalidConfigPnl")]
        [Header("Help")]
        [SerializeField]
        private GameObject InvalidConfigTemplatePnl;
        
        [SerializeField]
        private GameObject InvalidConfigPnl;
        
        [Header("CLI")]
        [SerializeField]
        private TextMeshProUGUI debugMemoTxt;
        
        [Header("Auth")]
        [SerializeField]
        private Button authBtn;
        [SerializeField]
        private TextMeshProUGUI authTxt;

        [Header("Lobby (Before Exists)")]
        [SerializeField]
        private Button createLobbyBtn;
        [SerializeField]
        private Button getLobbyInfoBtn;
        [SerializeField]
        private TMP_InputField getLobbyInfoInput;

        [Header("Lobby (After Exists)")]
        [SerializeField]
        private TextMeshProUGUI lobbyRoomIdTxt;
        [SerializeField]
        private Button viewLobbiesBtn;
        [SerializeField]
        private Button copyLobbyRoomIdBtn;
        [SerializeField]
        private TextMeshProUGUI copiedRoomIdFadeTxt;
        [SerializeField]
        private TextMeshProUGUI createOrGetLobbyInfoErrTxt;
        [SerializeField]
        private TextMeshProUGUI viewLobbiesSeeLogsFadeTxt;
        
        [Header("Room (Get Server/Connection Info)")]
        [SerializeField]
        private Button getServerInfoBtn;
        [SerializeField]
        private TextMeshProUGUI getServerInfoTxt;
        [SerializeField]
        private Button copyServerInfoBtn;
        [SerializeField]
        private TextMeshProUGUI copiedServerInfoFadeTxt;
        [SerializeField]
        private TextMeshProUGUI getServerInfoErrTxt;
        
        [Header("Transport (Fishnet): Join Lobby [as Client]")]
        [SerializeField]
        private Button joinLobbyAsClientBtn;
        
        [SerializeField]
        private TextMeshProUGUI joiningLobbyStatusTxt;
        #endregion // Serialized Fields

        public static NetUI Singleton { get; private set; }

        private const float FADE_TXT_DISPLAY_DURATION_SECS = 0.5f;
        private const string HATHORA_VIOLET_COLOR_HEX = "#EEDDFF";
        static string headerBoldColorBegin => $"<b><color={HATHORA_VIOLET_COLOR_HEX}>";
        const string headerBoldColorEnd = "</color></b>";
        private static NetHathoraClient hathoraClient => NetHathoraClient.Singleton;
        private static NetSession netSession => NetSession.Singleton;

        
        #region Init
        private void Awake()
        {
            setSingleton();
        }

        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[NetUI]**ERR @ setSingleton: Destroying dupe");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }
        #endregion // Init
        
        
        #region UI Interactions
        public void OnAuthLoginBtnClick()
        {
            hathoraClient.AssertUsingValidNetConfig();
                
            SetShowAuthTxt("<color=yellow>Logging in...</color>");
            _ = hathoraClient.AuthLoginAsync(); // !await
        }

        public void OnCreateLobbyBtnClick()
        {
            SetShowLobbyTxt("<color=yellow>Creating Lobby...</color>");

            // (!) Region Index starts at 1 (not 0) // TODO: Get from UI
            const Region _region = Region.WashingtonDC;
            
            _ = hathoraClient.CreateLobbyAsync(_region); // !await // public lobby
        }

        /// <summary>
        /// The player pressed ENTER || unfocused the ServerConnectionInfo input.
        /// </summary>
        public void OnGetLobbyInfoInputEnd()
        {
            ShowGettingLobbyInfoUi();
            string roomIdInputStr = GetLobbyInfoInputStr();
            getLobbyInfoInput.text = "";

            if (string.IsNullOrEmpty(roomIdInputStr))
            {
                OnAuthedLoggedIn();
                return;
            }
            
            _ = hathoraClient.GetLobbyInfoAsync(roomIdInputStr); // !await
        }

        /// <summary>
        /// Btn disabled OnClick via inspector: Restore on done
        /// </summary>
        public async void OnViewLobbiesBtnClick()
        {
            viewLobbiesSeeLogsFadeTxt.text = "<color=yellow>Getting Lobbies...</color>";
            _ = ShowFadeTxtThenFadeAsync(viewLobbiesSeeLogsFadeTxt); // !await

            // TODO: Get region from UI // TODO: Confirm null region returns ALL regions?
            Region? region = null;
            
            try
            {
                await hathoraClient.ViewPublicLobbies(region);
            }
            catch (Exception e)
            {
                viewLobbiesBtn.interactable = true;
            }
        }
        
        public void OnCopyLobbyRoomIdBtnClick()
        {
            GUIUtility.systemCopyBuffer = netSession.RoomId; // Copy to clipboard
            
            // Show + Fade
            _ = ShowFadeTxtThenFadeAsync(copiedRoomIdFadeTxt); // !await
        }

        /// <summary>
        /// We should only call this if we already have the lobby info (ServerConnectionInfo).
        /// </summary>
        public void OnGetServerInfoBtnClick()
        {
            SetServerInfoTxt("<color=yellow>Getting server connection info...</color>");
            
            // The ServerConnectionInfo should already be cached
            _ = hathoraClient.GetActiveConnectionInfo(netSession.RoomId); // !await
        }
        
        /// <summary>
        /// Copies as "ip:port"
        /// </summary>
        public void OnCopyServerInfoBtnClick()
        {
            string serverInfo = netSession.GetServerInfoIpPort(); // "ip:port"
            GUIUtility.systemCopyBuffer = serverInfo; // Copy to clipboard
            
            // Show + Fade
            _ = ShowFadeTxtThenFadeAsync(copiedServerInfoFadeTxt); // !await
        }

        /// <summary>Component OnClick hides joinLobbyAsClientBtn</summary>
        public void OnJoinLobbyAsClientBtnClick()
        {
            Debug.Log("[NetHathoraClient] OnJoinLobbyAsClientBtnClick");

            joinLobbyAsClientBtn.gameObject.SetActive(false);
            
            joiningLobbyStatusTxt.text = "<color=yellow>Joining Lobby...</color>";
            joiningLobbyStatusTxt.gameObject.SetActive(true);
            
            _ = hathoraClient.ConnectAsync();
        }
        
        public void OnJoinLobbySuccess()
        {
            Debug.Log("[NetHathoraClient] OnJoinLobbySuccess");
            
            joiningLobbyStatusTxt.text = "<color=green>Joined Lobby</color>";
            // Player stats should be updated via NetHathoraPlayer.OnStartClient
        }

        public void OnJoinLobbyFailed(string _friendlyErr)
        {
            Debug.Log($"[NetHathoraClient] OnJoinLobbyFailed: {_friendlyErr}");

            joiningLobbyStatusTxt.gameObject.SetActive(false);
            joinLobbyAsClientBtn.gameObject.SetActive(true);

            if (string.IsNullOrEmpty(_friendlyErr))
                return;
            
            joiningLobbyStatusTxt.text = $"<color=orange>{_friendlyErr}</color>";
        }
        #endregion // UI Interactions
        

        #region Dynamic UI
        public void OnAuthedLoggedIn()
        {
            SetShowAuthTxt("<b>Client Logged In</b> (Anonymously)");
            showInitLobbyUi(true);
        }
        
        public void OnAuthFailed()
        {
            SetShowAuthTxt("<color=orange>Login Failed</color>");
        }
        
        /// <summary>
        /// (!) Don't reset roomIdInputStr.text here
        /// </summary>
        public void ShowGettingLobbyInfoUi()
        {
            // Hide all of lobby EXCEPT the room id text
            SetShowLobbyTxt("<color=yellow>Getting Lobby Info...</color>");
            showInitLobbyUi(false);
        }
        
        /// <summary>Shows arbitrary text at the bottom of the screen</summary>
        /// <param name="memoStr"></param>
        public void SetShowDebugMemoTxt(string memoStr)
        {
            debugMemoTxt.text = memoStr;
            debugMemoTxt.gameObject.SetActive(true);
            Debug.Log($"[NetUI] Debug Memo: '{memoStr}'");
        }

        public void SetShowAuthTxt(string authStr)
        {
            authTxt.text = authStr;
            authTxt.gameObject.SetActive(true);
        }
        
        public void SetShowLobbyTxt(string roomId)
        {
            lobbyRoomIdTxt.text = roomId;
            lobbyRoomIdTxt.gameObject.SetActive(true);
        }

        public void SetShowCreateOrJoinLobbyErrTxt(string friendlyErrStr)
        {
            createOrGetLobbyInfoErrTxt.text = friendlyErrStr;
            createOrGetLobbyInfoErrTxt.gameObject.SetActive(true);
        }
        
        public void SetGetServerInfoErrTxt(string friendlyErrStr)
        {
            getServerInfoErrTxt.text = friendlyErrStr;
            getServerInfoErrTxt.gameObject.SetActive(true);
        }
        
        public void SetServerInfoTxt(string serverInfo)
        {
            getServerInfoTxt.text = serverInfo;
            getServerInfoTxt.gameObject.SetActive(true);
        }

        /// <summary>
        /// This also resets interactable
        /// </summary>
        /// <param name="show"></param>
        private void showInitLobbyUi(bool show)
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

        public void OnCreatedOrJoinedLobbyFail()
        {
            showInitLobbyUi(true);
            SetShowCreateOrJoinLobbyErrTxt("<color=orange>Failed to Get Lobby info - see logs</color>");
        }

        public void OnGetServerInfoSuccess(ConnectionInfoV2 connectionInfo)
        {
            Debug.Log(
                $"[NetUI] OnGetServerInfoSuccess: {netSession.GetServerInfoIpPort()} " +
                $"({connectionInfo.ExposedPort.TransportType})");
            
            // ####################
            // ServerInfo:
            // 127.0.0.1:7777 (UDP)
            // ####################
            SetServerInfoTxt($"{headerBoldColorBegin}ServerInfo{headerBoldColorEnd}:\n" +
                $"{connectionInfo.ExposedPort.Host}<color=yellow><b>:</b></color>{connectionInfo.ExposedPort.Port}\n" +
                $"(<color=yellow>{connectionInfo.ExposedPort.TransportType}</color>)");
            
            copyServerInfoBtn.gameObject.SetActive(true);
            joinLobbyAsClientBtn.gameObject.SetActive(true);
        }
        
        public void OnGetServerInfoFail()
        {
            getServerInfoBtn.gameObject.SetActive(true);
            SetGetServerInfoErrTxt("<color=orange>Failed to Get Server Info - see logs</color>");
        }
        
        public void OnCreatedOrJoinedLobby(string _roomId, string _friendlyRegionStr)
        {
            // Hide all init lobby UI except the txt + view lobbies
            showInitLobbyUi(false);
            SetShowLobbyTxt($"{headerBoldColorBegin}RoomId{headerBoldColorEnd}:\n{_roomId}\n\n" +
                $"{headerBoldColorBegin}Region{headerBoldColorEnd}: {_friendlyRegionStr}");

            // We can now show the lobbies and ServerConnectionInfo copy btn
            copyLobbyRoomIdBtn.gameObject.SetActive(true);
            viewLobbiesBtn.gameObject.SetActive(true);
            getServerInfoBtn.gameObject.SetActive(true);
        }

        public string GetLobbyInfoInputStr() =>
            getLobbyInfoInput.text.Trim();
        
        public void OnViewLobbies(List<Lobby> lobbies)
        {
            viewLobbiesSeeLogsFadeTxt.text = "See Logs";
            _ = ShowFadeTxtThenFadeAsync(viewLobbiesSeeLogsFadeTxt); // !await
            
            foreach (Lobby lobby in lobbies)
            {
                Debug.Log($"[NetPlayerUI] OnViewLobbies - lobby found: " +
                    $"RoomId={lobby.RoomId}, CreatedAt={lobby.CreatedAt}, CreatedBy={lobby.CreatedBy}");
            }
            
            // TODO: Create a UI view for these servers
            viewLobbiesBtn.interactable = true;
        }
        
        private async Task ShowFadeTxtThenFadeAsync(TextMeshProUGUI fadeTxt)
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
        
        public void SetInvalidConfig(HathoraClientConfig _config)
        {
            if (authBtn != null)
                authBtn.gameObject.SetActive(false); // Prevent UI overlap
            
            // Core issue
            string netComponentPathFriendlyStr = $" HathoraManager (GameObject)'s " +
                $"{nameof(NetHathoraClient)} component";
            
            if (_config == null)
            {
                InvalidConfigPnl.SetActive(true);

                throw new Exception($"[{nameof(NetHathoraClient)}] !{nameof(HathoraClientConfig)} - " +
                    $"Serialize one at {netComponentPathFriendlyStr}");
            }
            
            if (!_config.HasAppId)
            {
                InvalidConfigPnl.SetActive(true);

                throw new Exception($"[{nameof(NetHathoraClient)}] !AppId - " +
                    $"Set one at {netComponentPathFriendlyStr} (See top menu `Hathora/Configuration` - your " +
                    "ServerConfig's AppId should match your ClientConfig's AppId)");
            }
            
            bool isTemplate = _config.name.Contains(".template");
            if (!isTemplate)
                return;
            
            authBtn.gameObject.SetActive(false);
            InvalidConfigTemplatePnl.SetActive(true);
                
            throw new Exception("[NetUI.SetInvalidConfig] Error: " +
                "Using template Config! Create a new one via top menu `Hathora/Config Finder`");
        }
        #endregion /Dynamic UI
    }
}

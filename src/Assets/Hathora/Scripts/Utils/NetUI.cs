// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Client;
using Hathora.Scripts.Net.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hathora.Scripts.Utils
{
    /// <summary>
    /// Handles the non-Player UI so we can keep the logic separate.
    /// Generally, this is going to be pre-connection UI such as create/join lobbies.
    /// UI OnEvent entry points from Buttons start here.
    /// </summary>
    public class NetUI : MonoBehaviour
    {
        #region Serialized Fields
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
        
        [Header("Transport (Fishnet): Join Lobby as Client")]
        [SerializeField]
        private Button joinLobbyAsClientBtn;
        #endregion // Serialized Fields

        public static NetUI Singleton { get; private set; }

        private const float FADE_TXT_DISPLAY_DURATION_SECS = 0.5f;
        private const string HATHORA_VIOLET_COLOR_HEX = "#EEDDFF";
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
            SetShowAuthTxt("<color=yellow>Logging in...</color>");
            hathoraClient.AuthLoginAsync();
        }

        public void OnCreateLobbyBtnClick()
        {
            SetShowLobbyTxt("<color=yellow>Creating Lobby...</color>");
            hathoraClient.CreateLobbyAsync(); // public lobby
        }

        /// <summary>
        /// The player pressed ENTER || unfocused the ServerInfo input.
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
            
            hathoraClient.GetLobbyInfoAsync(roomIdInputStr);
        }

        public void OnViewLobbiesBtnClick()
        {
            viewLobbiesSeeLogsFadeTxt.text = "<color=yellow>Getting Lobbies...</color>";
            ShowFadeTxtThenFadeAsync(viewLobbiesSeeLogsFadeTxt);
            hathoraClient.ViewPublicLobbies();
        }
        
        public void OnCopyLobbyRoomIdBtnClick()
        {
            GUIUtility.systemCopyBuffer = netSession.RoomId; // Copy to clipboard
            
            // Show + Fade
            ShowFadeTxtThenFadeAsync(copiedRoomIdFadeTxt);
        }

        /// <summary>
        /// We should only call this if we already have the lobby info (ServerInfo).
        /// </summary>
        public void OnGetServerInfoBtnClick()
        {
            SetServerInfoTxt("<color=yellow>Getting server connection info...</color>");
            
            // The ServerInfo should already be cached
            hathoraClient.GetActiveConnectionInfo(netSession.RoomId);
        }
        
        /// <summary>
        /// Copies as "ip:port"
        /// </summary>
        public void OnCopyServerInfoBtnClick()
        {
            string serverInfo = netSession.GetServerInfoIpPort(); // "ip:port"
            GUIUtility.systemCopyBuffer = serverInfo; // Copy to clipboard
            
            // Show + Fade
            ShowFadeTxtThenFadeAsync(copiedServerInfoFadeTxt);
        }

        public void OnJoinLobbyAsClientBtnClick()
        {
            throw new NotImplementedException("TODO");
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
        
        public void SetShowDebugMemoTxt(string memoStr)
        {
            debugMemoTxt.text = memoStr;
            debugMemoTxt.gameObject.SetActive(true);
            Debug.Log($"[NetCmdLine] Debug Memo: '{memoStr}'");
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
            lobbyRoomIdTxt.gameObject.SetActive(show); // When showing, it's behind the btns ^
            
            // On or off: If this is resetting, we'll hide it. 
            // This also hides the cancel btn
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

        public void OnGetServerInfoSuccess(ActiveConnectionInfo serverInfo)
        {
            // ####################
            // ServerInfo:
            // 127.0.0.1:7777
            // (UDP)
            // ####################
            SetServerInfoTxt($"<b><color={HATHORA_VIOLET_COLOR_HEX}>ServerInfo</color></b>:\n" +
                $"{serverInfo.Host}<color=yellow><b>:</b></color>{serverInfo.Port}\n(<color=yellow>{serverInfo.TransportType}</color>)");
            
            copyServerInfoBtn.gameObject.SetActive(true);
            joinLobbyAsClientBtn.gameObject.SetActive(true);
        }

        [Obsolete("TODO: Delete once SDK bug is resolved")]
        private void mockGetServerInfoSuccess()
        {
            Debug.Log("[NetUI]<color=yellow>**MOCKING SUCCESS**</color>");
            
            ActiveConnectionInfo serverInfo = new(
                0,
                TransportType.Udp,
                7777,
                "127.0.0.1",
                netSession.RoomId
            );

            netSession.ServerInfo = serverInfo; // We still need to set session for copy btn
            OnGetServerInfoSuccess(serverInfo);
        }
        
        public void OnGetServerInfoFail()
        {
            mockGetServerInfoSuccess();
            return;
            
            getServerInfoBtn.gameObject.SetActive(true);
            SetGetServerInfoErrTxt("<color=orange>Failed to Get Server Info - see logs</color>");
        }
        
        public void OnCreatedOrJoinedLobby(string roomId)
        {
            // Hide all init lobby UI except the txt + view lobbies
            showInitLobbyUi(false);
            SetShowLobbyTxt($"<b><color={HATHORA_VIOLET_COLOR_HEX}>RoomId</color></b>:\n{roomId}");

            // We can now show the lobbies and ServerInfo copy btn
            copyLobbyRoomIdBtn.gameObject.SetActive(true);
            viewLobbiesBtn.gameObject.SetActive(true);
            getServerInfoBtn.gameObject.SetActive(true);
        }

        public string GetLobbyInfoInputStr() =>
            getLobbyInfoInput.text.Trim();
        
        public void OnViewLobbies(List<Lobby> lobbies)
        {
            viewLobbiesSeeLogsFadeTxt.text = "See Logs";
            ShowFadeTxtThenFadeAsync(viewLobbiesSeeLogsFadeTxt);
            
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
        #endregion /Dynamic UI

    }
}

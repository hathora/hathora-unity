// Created by dylan@hathora.dev

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Hathora.Net.Client
{
    /// <summary>
    /// Handles the Player UI so we can keep the logic separate.
    /// </summary>
    public class NetPlayerUI : MonoBehaviour
    {
        [Header("Auth")]
        [SerializeField]
        private Button authBtn;
        [SerializeField]
        private TextMeshProUGUI authTxt;
        
        [Header("Room")]
        // (!) The name RoomName is deprecated for RoomId.
        [SerializeField]
        private Button createRoomBtn;
        [SerializeField]
        private Button joinRoomBtn;
        [SerializeField]
        private TextMeshProUGUI roomNameTxt;
        [SerializeField]
        private Button copyRoomNameBtn;
        [SerializeField]
        private TextMeshProUGUI copiedRoomNameFadeTxt;
        
        [Header("Lobby")]
        // (!) The name LobbyId actually comes from Lobby.RoomId (bug?)
        [SerializeField]
        private Button createLobbyBtn;
        [SerializeField]
        private Button joinLobbyBtn;
        [SerializeField]
        private Button viewLobbiesBtn;
        [SerializeField]
        private TextMeshProUGUI lobbyIdTxt;
        [SerializeField]
        private Button copyLobbyIdBtn;
        [SerializeField]
        private TextMeshProUGUI copiedLobbyIdFadeTxt;
        
        private const int FADE_TXT_DISPLAY_DURATION_SECS = 3;
        private const string HATHORA_VIOLET_COLOR_HEX = "#B873FF";
        
        public void SetShowAuthTxt(string authStr)
        {
            authTxt.text = authStr;
            authTxt.gameObject.SetActive(true);
        }
        
        public void SetShowRoomTxt(string roomName)
        {
            roomNameTxt.text = roomName;
            roomNameTxt.gameObject.SetActive(true);
        }
        
        /// <param name="lobbyId">BUG: Currently called roomId</param>
        public void SetShowLobbyTxt(string lobbyId)
        {
            lobbyIdTxt.text = lobbyId;
            lobbyIdTxt.gameObject.SetActive(true);
        }

        public void OnLoggedIn()
        {
            SetShowAuthTxt("Logged In");
            createRoomBtn.gameObject.SetActive(true);
            joinRoomBtn.gameObject.SetActive(true);
        }

        public void OnJoinedOrCreatedRoom(string roomName)
        {
            SetShowRoomTxt($"<color={HATHORA_VIOLET_COLOR_HEX}>RoomId</color>:\n{roomName}");
            copyRoomNameBtn.gameObject.SetActive(true);
            
            createLobbyBtn.gameObject.SetActive(true);
            joinLobbyBtn.gameObject.SetActive(true);
        }
        
        /// <param name="lobbyId">BUG: Currently called roomId</param>
        public void OnJoinedOrCreatedLobby(string lobbyId)
        {
            SetShowLobbyTxt($"<color={HATHORA_VIOLET_COLOR_HEX}>LobbyId</color>:\n{lobbyId}");
            copyLobbyIdBtn.gameObject.SetActive(true);
            
            viewLobbiesBtn.gameObject.SetActive(true);
        }

        public void OnCopyRoomNameBtnClick()
        {
            string roomNameFriendlyStr = roomNameTxt.text;
            
            // We need just the {RoomName} from "<color=yellow>RoomId:</color>\n{RoomName}"
            string roomName = roomNameFriendlyStr.Split('\n')[1].Trim();
            GUIUtility.systemCopyBuffer = roomName;
            
            // Show + Fade
            ShowAndFadeCopiedTextAsync(copiedRoomNameFadeTxt);
        }
        
        public void OnCopyLobbyIdBtnClick()
        {
            string lobbyIdFriendlyStr = lobbyIdTxt.text;
            
            // We need just the {RoomName} from "<color=yellow>LobbyId:</color>\n{LobbyId}"
            string lobbyId = lobbyIdFriendlyStr.Split('\n')[1].Trim();
            GUIUtility.systemCopyBuffer = lobbyId;
            
            // Show + Fade
            ShowAndFadeCopiedTextAsync(copiedLobbyIdFadeTxt);
        }
        
        private async Task ShowAndFadeCopiedTextAsync(TextMeshProUGUI fadeTxt)
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
    }
}

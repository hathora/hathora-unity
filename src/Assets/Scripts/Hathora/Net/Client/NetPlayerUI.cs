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

        [Header("Lobby (Client Room)")]
        [SerializeField]
        private Button createLobbyBtn;
        [SerializeField]
        private Button joinLobbyBtn;
        [SerializeField]
        private Button viewLobbiesBtn;
        [SerializeField]
        private TextMeshProUGUI lobbyRoomIdTxt;
        [SerializeField]
        private Button copyLobbyRoomIdBtn;
        [SerializeField]
        private TextMeshProUGUI copiedLobbyRoomIdFadeTxt;
        [SerializeField]
        private TMP_InputField joinLobbyInput;
        
        private const int FADE_TXT_DISPLAY_DURATION_SECS = 3;
        private const string HATHORA_VIOLET_COLOR_HEX = "#B873FF";
        
        
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

        public void OnLoggedIn()
        {
            SetShowAuthTxt("<b>Client Logged In</b> (Anonymously)");
            showInitLobbyUi(true);
        }

        private void showInitLobbyUi(bool show)
        {
            createLobbyBtn.gameObject.SetActive(show);
            joinLobbyBtn.gameObject.SetActive(show);
            lobbyRoomIdTxt.gameObject.SetActive(show); // When showing, it's behind the btns ^
            
            // On or off: If this is resetting, we'll hide it. 
            // This also hides the cancel btn
            joinLobbyInput.gameObject.SetActive(false);
            copyLobbyRoomIdBtn.gameObject.SetActive(!show);
        }
        
        /// <summary>
        /// Returns the RoomId input string.
        /// </summary>
        /// <returns></returns>
        public string OnJoinLobbyBtnClickGetRoomId()
        {
            // Hide all of lobby EXCEPT the room id text
            SetShowLobbyTxt("<color=yellow>Getting Lobby Info...</color>");
            string lobbyInputStr = GetJoinLobbyInputStr();
            joinLobbyInput.text = "";

            showInitLobbyUi(false);

            return lobbyInputStr;
        }

        public void OnJoinedOrCreatedLobby(string roomId)
        {
            // Hide all init lobby UI except the txt + view lobbies
            showInitLobbyUi(false);
            SetShowLobbyTxt($"<b><color={HATHORA_VIOLET_COLOR_HEX}>RoomId</color></b>:\n{roomId}");
            
            // We can now show the lobbies and roomId copy btn
            copyLobbyRoomIdBtn.gameObject.SetActive(true);
            viewLobbiesBtn.gameObject.SetActive(true);
        }

        public void OnCopyLobbyRoomIdBtnClick()
        {
            string lobbyRoomIdFriendlyStr = lobbyRoomIdTxt.text;
            
            // We need just the {RoomId} from "<color=yellow>RoomId:</color>\n{RoomId}"
            string roomId = lobbyRoomIdFriendlyStr.Split('\n')[1].Trim();
            GUIUtility.systemCopyBuffer = roomId;
            
            // Show + Fade
            ShowAndFadeCopiedTextAsync(copiedLobbyRoomIdFadeTxt);
        }

        public string GetJoinLobbyInputStr() =>
            joinLobbyInput.text;
        
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

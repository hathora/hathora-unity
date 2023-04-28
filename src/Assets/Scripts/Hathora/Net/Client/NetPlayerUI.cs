// Created by dylan@hathora.dev

using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField]
        private Button createRoomBtn;
        [SerializeField]
        private Button joinRoomBtn;
        [SerializeField]
        private TextMeshProUGUI roomTxt;
        
        [Header("Lobby")]
        [SerializeField]
        private Button createLobbyBtn;
        [SerializeField]
        private Button joinLobbyBtn;
        [SerializeField]
        private Button viewLobbiesBtn;
        [SerializeField]
        private TextMeshProUGUI lobbyTxt;
        
        public void SetShowAuthTxt(string authStr)
        {
            authTxt.text = authStr;
            authTxt.gameObject.SetActive(true);
        }
        
        public void SetShowRoomTxt(string roomName)
        {
            roomTxt.text = roomName;
            roomTxt.gameObject.SetActive(true);
        }
        
        /// <param name="lobbyId">BUG: Currently called roomId</param>
        public void SetShowLobbyTxt(string lobbyId)
        {
            lobbyTxt.text = lobbyId;
            lobbyTxt.gameObject.SetActive(true);
        }

        public void OnLoggedIn()
        {
            SetShowAuthTxt("<color=green>Logged In</color>");
            createRoomBtn.gameObject.SetActive(true);
        }

        public void OnJoinedOrCreatedRoom(string roomName)
        {
            SetShowRoomTxt($"<color=green>{roomName}</color>");
            createLobbyBtn.gameObject.SetActive(true);
            joinLobbyBtn.gameObject.SetActive(true);
        }
        
        /// <param name="lobbyId">BUG: Currently called roomId</param>
        public void OnJoinedOrCreatedLobby(string lobbyId)
        {
            viewLobbiesBtn.gameObject.SetActive(true);
        }
    }
}

// Created by dylan@hathora.dev

using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hathora.Net
{
    /// <summary>
    /// Handles the Player UI so we can keep the logic separate.
    /// </summary>
    public class NetPlayerUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI authTxt;
        [SerializeField]
        private Button createRoomBtn;
        [SerializeField]
        private Button createLobbyBtn;

        
        public void SetShowAuthTxt(string authStr)
        {
            authTxt.text = authStr;
            authTxt.gameObject.SetActive(true);
        }

        public void OnLoggedIn()
        {
            SetShowAuthTxt("<color=green>Logged In</color>");
            createRoomBtn.gameObject.SetActive(true);
        }

        public void OnCreatedRoom(string roomName)
        {
            // TODO: Show [copyable] room name
            createLobbyBtn.gameObject.SetActive(true);
        }
    }
}

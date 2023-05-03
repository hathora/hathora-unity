// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Net
{
    public class NetSession : MonoBehaviour
    {
        /// <summary>
        /// Client: From AuthV1 via NetHathoraPlayer.
        /// </summary>
        public string PlayerAuthToken { get; private set; }
        public bool IsAuthed => !string.IsNullOrEmpty(PlayerAuthToken);

        /// <summary>
        /// Creating a lobby is actually just the client-side way of creating a Room.
        /// This is why a Lobby has a RoomId. 
        /// </summary>
        public string RoomId { get; set; }

        public Lobby Lobby { get; set; }

        /// <summary>
        /// Server sets - PlayerAuthToken and RoomId are SyncVar'd.
        /// Resets the session to new auth tokens; clears other cache, such as rooms.
        /// </summary>
        /// <param name="playerAuthToken">
        /// Server: Dev token, serialized from HathoraServerConfig file.
        /// Client: From AuthV1 via NetHathoraPlayer.
        /// </param>
        public void InitNetSession(string playerAuthToken)
        {
            this.PlayerAuthToken = playerAuthToken;
            this.RoomId = null;
        }
    }
}

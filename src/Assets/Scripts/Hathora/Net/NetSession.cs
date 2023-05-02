// Created by dylan@hathora.dev

using FishNet.Object;
using FishNet.Object.Synchronizing;
using Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Net
{
    public class NetSession : NetworkBehaviour
    {
        /// <summary>
        /// Client: From AuthV1 via NetHathoraPlayer.
        /// </summary>
        [HideInInspector]
        public string PlayerAuthToken;
        public bool IsAuthed => !string.IsNullOrEmpty(PlayerAuthToken);

        /// <summary>
        /// Creating a lobby is actually just the client-side way of creating a Room.
        /// This is why a Lobby has a RoomId. 
        /// </summary>
        [HideInInspector]
        public string RoomId;

        [HideInInspector]
        public Lobby Lobby; // TODO


        /// <summary>
        /// Server sets - PlayerAuthToken and RoomId are SyncVar'd.
        /// Resets the session to new auth tokens; clears other cache, such as rooms.
        /// </summary>
        /// <param name="playerAuthToken">
        /// Server: Dev token, serialized from HathoraServerConfig file.
        /// Client: From AuthV1 via NetHathoraPlayer.
        /// </param>
        /// <param name="devAuthToken">Server: From HathoraServerConfig file.</param>
        public void InitNetSession(string playerAuthToken)
        {
            if (!IsServer)
            {
                Debug.LogWarning("[NetSession] InitNetSession should only be called on the server.");
                return;
            }

            this.PlayerAuthToken = playerAuthToken;
            this.RoomId = null;
        }
    }
}

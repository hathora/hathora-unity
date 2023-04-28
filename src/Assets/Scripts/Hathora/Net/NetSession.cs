// Created by dylan@hathora.dev

using FishNet.Object;
using FishNet.Object.Synchronizing;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Server.Models;
using UnityEngine;

namespace Hathora.Net
{
    public class NetSession : NetworkBehaviour
    {
        #region Sync'd
        /// <summary>
        /// Client: From AuthV1 via NetHathoraPlayer.
        /// </summary>
        [HideInInspector, SyncVar]
        public string AuthToken;

        [HideInInspector, SyncVar]
        public string RoomName;

        // [HideInInspector]
        // public SyncLobby Lobby; // TODO
        #endregion // Sync'd

        
        /// <summary>
        /// Server: From HathoraServerConfig file.
        /// </summary>
        public string DevAuthToken { get; set; }
        
        public bool IsAuthed => !string.IsNullOrEmpty(AuthToken);

        /// <summary>
        /// Server sets - AuthToken and RoomName are SyncVar'd.
        /// Resets the session to new auth tokens; clears other cache, such as rooms.
        /// </summary>
        /// <param name="playerAuthToken">
        /// Server: Dev token, serialized from HathoraServerConfig file.
        /// Client: From AuthV1 via NetHathoraPlayer.
        /// </param>
        /// <param name="devAuthToken">Server: From HathoraServerConfig file.</param>
        public void InitNetSession(string playerAuthToken, string devAuthToken = null)
        {
            if (!IsServer)
            {
                Debug.LogWarning("[NetSession] InitNetSession should only be called on the server.");
                return;
            }

            this.AuthToken = playerAuthToken;
            this.RoomName = null;
            
            #if UNITY_SERVER || DEBUG
            this.DevAuthToken = devAuthToken;
            #endif
        }
    }
}

// Created by dylan@hathora.dev

using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Net
{
    public class NetSession : NetworkBehaviour
    {
        /// <summary>
        /// Server only.
        /// </summary>
        [FormerlySerializedAs("AuthToken")]
        [HideInInspector]
        public string PlayerAuthToken;

        [HideInInspector, SyncVar]
        public string CachedRoomName;

        public bool IsAuthed => !string.IsNullOrEmpty(PlayerAuthToken);

        
        /// <summary>
        /// Resets and clears the session.
        /// </summary>
        /// <param name="authToken"></param>
        public void InitNetSession(string authToken)
        {
            if (!IsServer)
            {
                Debug.LogWarning("[NetSession] InitNetSession should only be called on the server.");
                return;
            }

            this.PlayerAuthToken = authToken;
            this.CachedRoomName = null;
        }
    }
}

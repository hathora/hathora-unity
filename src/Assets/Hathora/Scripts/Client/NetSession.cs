// Created by dylan@hathora.dev

using Hathora.Scripts.Sdk.hathora_cloud_sdks.csharp.src.Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Scripts.Client
{
    /// <summary>
    /// Cached net session. Eg: Auth token, last room/lobby joined.
    /// API wrappers will cache here on success.
    /// TODO: Move NetSession to Demo. This would require detaching cache saving from API wrappers.
    /// </summary>
    public class NetSession : MonoBehaviour
    {
        public static NetSession Singleton { get; private set; }
        
        /// <summary>
        /// Client: From AuthV1 via NetHathoraPlayer.
        /// </summary>
        public string PlayerAuthToken { get; private set; }
        public bool IsAuthed => !string.IsNullOrEmpty(PlayerAuthToken);

        public Lobby Lobby { get; set; }
        public string RoomId => Lobby?.RoomId;

        public ConnectionInfoV2 ServerInfo { get; set; }
        public string GetServerInfoIpPort() => 
            $"{ServerInfo?.ExposedPort.Host}:{ServerInfo?.ExposedPort.Port}";

        
        #region Init
        public void Awake()
        {
            setSingleton();
        }
        
        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[NetSession]**ERR @ setSingleton: Destroying dupe");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }

        /// <summary>
        /// For a new Session, we simply update the PlayerAuthToken.
        /// </summary>
        /// <param name="playerAuthToken"></param>
        public void InitNetSession(string playerAuthToken)
        {
            this.PlayerAuthToken = playerAuthToken;
        }
        #endregion // Init
    }
}

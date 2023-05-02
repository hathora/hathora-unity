// Created by dylan@hathora.dev

using FishNet.Object;
using Hathora.Cloud.Sdk.Client;
using Hathora.Net.Server;
using UnityEngine;

namespace Hathora.Net
{
    /// <summary>
    /// This allows the API to view config (eg: appId), set session and auth tokens.
    /// Both Client and Server APIs can inherit from this.
    /// </summary>
    public abstract class NetHathoraApiBase : NetworkBehaviour
    {
        protected Configuration hathoraSdkConfig { get; private set; }
        protected HathoraServerConfig hathoraServerConfig { get; private set; }
        protected NetSession PlayerSession { get; private set; }


        /// <summary>
        /// Init anytime. Client calls use V1 auth token. Server calls use Dev token.
        /// </summary>
        /// <param name="_hathoraSdkConfig">[SDK] Contains DevToken to make server calls.</param>
        /// <param name="_hathoraServerConfig">[Wrapper] Contains general server info, like ApiKey.</param>
        /// <param name="_playerSession">[Player] Session instance for updating cache.</param>
        public virtual void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig,
            NetSession _playerSession)
        {
            if (IsServer && string.IsNullOrEmpty(_hathoraServerConfig?.DevAuthToken))
            {
                Debug.LogWarning("[NetHathoraApiBase]*WARN @ Init: " +
                    "Missing HathoraServerConfig.DevAuthToken - Hathora server SDK calls will fail.");
                return;
            }

            this.hathoraSdkConfig = _hathoraSdkConfig;
            this.hathoraServerConfig = _hathoraServerConfig;
            this.PlayerSession = _playerSession;
        }
    }
}

// Created by dylan@hathora.dev

using FishNet.Object;
using Hathora.Cloud.Sdk.Client;
using UnityEngine;

namespace Hathora.Net.Server.Models
{
    /// <summary>
    /// Inherit from this when using server APIs.
    /// </summary>
    public abstract class HathoraNetServerApiBase : NetworkBehaviour
    {
        protected Configuration hathoraSdkConfig { get; private set; }
        protected HathoraServerConfig hathoraServerConfig { get; private set; }
        protected NetSession PlayerSession { get; private set; }


        /// <summary>
        /// Init anytime - these calls use dev token.
        /// </summary>
        /// <param name="_hathoraSdkConfig">[SDK] Contains DevToken to make server calls.</param>
        /// <param name="_hathoraServerConfig">[Wrapper] Contains general server info, like ApiKey.</param>
        /// <param name="_playerSession">[Player] Session instance for updating cache.</param>
        public virtual void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig,
            NetSession _playerSession)
        {
            if (string.IsNullOrEmpty(_hathoraServerConfig?.DevAuthToken))
            {
                Debug.LogError("[NetHathoraServerRoom]**ERR @ Init: " +
                    "Missing HathoraServerConfig.DevAuthToken.");
                return;
            }

            this.hathoraSdkConfig = _hathoraSdkConfig;
            this.hathoraServerConfig = _hathoraServerConfig;
            this.PlayerSession = _playerSession;
        }
    }
}

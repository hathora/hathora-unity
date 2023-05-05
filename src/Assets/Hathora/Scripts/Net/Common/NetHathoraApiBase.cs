// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Client;
using Hathora.Scripts.Net.Server;
using UnityEngine;

namespace Hathora.Scripts.Net.Common
{
    /// <summary>
    /// This allows the API to view UserConfig (eg: AppId), set session and auth tokens.
    /// Both Client and Server APIs can inherit from this.
    /// </summary>
    public abstract class NetHathoraApiBase : MonoBehaviour
    {
        protected Configuration hathoraSdkConfig { get; private set; }
        protected HathoraServerConfig hathoraServerConfig { get; private set; }
        protected NetSession NetSession { get; private set; }


        /// <summary>
        /// Init anytime. Client calls use V1 auth token. Server calls use Dev token.
        /// </summary>
        /// <param name="_hathoraSdkConfig">[SDK] Contains DevToken to make server calls.</param>
        /// <param name="_hathoraServerConfig">[Wrapper] Contains general server info, like ApiKey.</param>
        /// <param name="_netSession">[Player] Session instance for updating cache.</param>
        public virtual void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig,
            NetSession _netSession)
        {
            this.hathoraSdkConfig = _hathoraSdkConfig;
            this.hathoraServerConfig = _hathoraServerConfig;
            this.NetSession = _netSession;
        }
    }
}

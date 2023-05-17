// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Client;
using Hathora.Scripts.Net.Common;
using UnityEngine;

namespace Hathora.Scripts.Net.Client.ApiWrapper
{
    /// <summary>
    /// This allows the API to view UserConfig (eg: AppId), set session and auth tokens.
    /// Both Client and Server APIs can inherit from this.
    /// </summary>
    public abstract class NetHathoraApiBase : MonoBehaviour
    {
        protected Configuration hathoraSdkConfig { get; private set; }
        protected NetHathoraConfig NetHathoraConfig { get; private set; }
        protected NetSession NetSession { get; private set; }


        /// <summary>
        /// Init anytime. Client calls use V1 auth token.
        /// </summary>
        /// <param name="_hathoraSdkConfig">SDK config that we pass to Hathora API calls</param>
        /// <param name="_netHathoraConfig">Find via Unity editor top menu: Hathora >> Find Configs</param>
        /// <param name="_netSession">Client (not player) session instance for updating cache.</param>
        public virtual void Init(
            Configuration _hathoraSdkConfig, 
            NetHathoraConfig _netHathoraConfig,
            NetSession _netSession)
        {
            this.hathoraSdkConfig = _hathoraSdkConfig;
            this.NetHathoraConfig = _netHathoraConfig;
            this.NetSession = _netSession;
        }
    }
}

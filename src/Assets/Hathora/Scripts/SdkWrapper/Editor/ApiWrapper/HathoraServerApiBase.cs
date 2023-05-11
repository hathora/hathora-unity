// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Client;
using Hathora.Scripts.Net.Server;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Editor.ApiWrapper
{
    /// <summary>
    /// This allows the API to view UserConfig (eg: AppId), set session and auth tokens.
    /// Both Client and Server APIs can inherit from this.
    /// </summary>
    public abstract class HathoraServerApiBase : MonoBehaviour
    {
        protected Configuration hathoraSdkConfig { get; private set; }
        protected NetHathoraConfig NetHathoraConfig { get; private set; }


        /// <summary>
        /// Init anytime. Server calls use Dev token.
        /// </summary>
        /// <param name="_hathoraSdkConfig">SDK config that we pass to Hathora API calls</param>
        /// <param name="_netHathoraConfig">Find via Unity editor top menu: Hathora >> Find Configs</param>
        public virtual void Init(
            Configuration _hathoraSdkConfig, 
            NetHathoraConfig _netHathoraConfig)
        {
            this.hathoraSdkConfig = _hathoraSdkConfig;
            this.NetHathoraConfig = _netHathoraConfig;
        }
    }
}

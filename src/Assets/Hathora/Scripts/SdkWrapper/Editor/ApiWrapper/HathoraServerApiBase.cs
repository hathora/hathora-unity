// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using Configuration = Hathora.Cloud.Sdk.Client.Configuration;

namespace Hathora.Scripts.SdkWrapper.Editor.ApiWrapper
{
    /// <summary>
    /// This allows the API to view UserConfig (eg: AppId), set session and auth tokens.
    /// Both Client and Server APIs can inherit from this.
    /// Unlike Client API wrappers (since !Mono), we init via Constructor instead of Init().
    /// </summary>
    public abstract class HathoraServerApiBase
    {
        protected Configuration HathoraSdkConfig { get; private set; }
        protected NetHathoraConfig NetHathoraConfig { get; private set; }


        /// <summary>
        /// Server calls use Dev token.
        /// </summary>
        /// <param name="_hathoraSdkConfig">SDK config that we pass to Hathora API calls</param>
        /// <param name="_netHathoraConfig">Find via Unity editor top menu: Hathora >> Find Configs</param>
        protected HathoraServerApiBase(
            Configuration _hathoraSdkConfig, 
            NetHathoraConfig _netHathoraConfig)
        {
            this.HathoraSdkConfig = _hathoraSdkConfig;
            this.NetHathoraConfig = _netHathoraConfig;
        }
    }
}

// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Client;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.Utils;
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
        
        // Shortcuts
        protected string AppId => NetHathoraConfig.HathoraCoreOpts.AppId;


        /// <summary>
        /// Server calls use Dev token.
        /// </summary>
        /// <param name="_netHathoraConfig">
        /// Find via Unity editor top menu: Hathora >> Find Configs
        /// </param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in children.
        /// </param>
        protected HathoraServerApiBase(
            NetHathoraConfig _netHathoraConfig,
            Configuration _hathoraSdkConfig = null)
        {
            this.NetHathoraConfig = _netHathoraConfig;
            this.HathoraSdkConfig = _hathoraSdkConfig ?? 
                HathoraUtils.GenerateSdkConfig(_netHathoraConfig);
        }

        protected static void HandleServerApiException(
            string _className, 
            string _funcName,
            ApiException _apiException)
        {
            UnityEngine.Debug.LogError($"[{_className}.{_funcName}] API Error: " +
                $"{_apiException.ErrorCode} {_apiException.ErrorContent} | {_apiException.Message}");

            throw _apiException;
        }
    }
}

// Created by dylan@hathora.dev

using Hathora.Core.Scripts.Runtime.Common.Models;
using HathoraSdk;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
    /// <summary>
    /// This allows the API to view UserConfig (eg: AppId), set session and auth tokens.
    /// Server APIs can inherit from this.
    /// Unlike Client API wrappers (since !Mono), we init via Constructor instead of Init().
    /// </summary>
    public abstract class HathoraServerApiWrapperBase : IHathoraApiBase
    {
        public SDKConfig HathoraSdkConfig { get; set; }
        protected HathoraServerConfig HathoraServerConfig { get; private set; }

        /// <summary>Pulls "DevAuthToken" from HathoraServerConfig</summary>
        protected string Auth0DevToken => 
            HathoraServerConfig.HathoraCoreOpts.DevAuthOpts.DevAuthToken;

        // Shortcuts
        public string AppId
        {
            get {
                if (HathoraServerConfig == null)
                {
                    Debug.LogError("[HathoraServerApiWrapper.AppId.get] !HathoraServerConfig: " +
                        "Did you forget to add init a newly-added API @ HathoraServerMgr.initApis()?" +
                        "**For Non-host Clients (or Servers that don't have runtime Server API calls), you may ignore this**");
                    return null;
                }

                if (HathoraServerConfig.HathoraCoreOpts == null)
                {
                    Debug.LogError("[HathoraServerApiWrapper.AppId.get] HathoraServerConfig exists, " +
                        "but !HathoraServerConfig.HathoraCoreOpts");
                    return null;
                }

                if (string.IsNullOrEmpty(HathoraServerConfig.HathoraCoreOpts.AppId))
                {
                    Debug.LogError("[HathoraServerApiWrapper.AppId.get] " +
                        "!HathoraServerConfig.HathoraCoreOpts.AppId -- " +
                        "Did you configure your HathoraServerConfig?");
                    return null;
                }

                return HathoraServerConfig.HathoraCoreOpts.AppId;
            }
        }
        

        #region Init
        /// <summary>
        /// Server calls use Dev token. Unlike ClientMgr, Server !inherits
        /// from Mono (so we init via constructor).
        /// </summary>
        /// <param name="_hathoraServerConfig">
        /// Find via Unity editor top menu: Hathora >> Find Configs
        /// </param>
        protected HathoraServerApiWrapperBase(
            HathoraServerConfig _hathoraServerConfig,
            SDKConfig _hathoraSdkConfig = null)
        {
            this.HathoraServerConfig = _hathoraServerConfig;
            this.HathoraSdkConfig = _hathoraSdkConfig ?? GenerateSdkConfig();
        }
        
        public SDKConfig GenerateSdkConfig() => new()
        {
            // TODO: The new `SDKConfig` is empty, but previously passed AccessToken; how does the new SDK handle this if we don't pass the token?
            // AccessToken = HathoraServerConfig.HathoraCoreOpts.DevAuthOpts.DevAuthToken,
        };
        #endregion // Init

        
        #region Utils
        public string ToJson<T>(T Obj, bool prettify = true)
        {
            Formatting formatting = prettify ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(Obj, formatting);
        }
        #endregion // Utils
    }
}

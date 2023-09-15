// Created by dylan@hathora.dev

using HathoraSdk;
using Newtonsoft.Json;

namespace Hathora.Core.Scripts.Runtime.Common.ApiWrapper
{
    /// <summary>
    /// Common high-level helper of common utils for Client/Server API wrappers.
    /// - Stores HathoraSDK
    /// - Shortcut to commonly-used AppId
    /// - ToJson helper to serialize [+prettify] API requests/results. 
    /// </summary>
    public abstract class HathoraApiWrapperBase
    {
        #region Vars
        private HathoraSDK HathoraSdk { get; set; }

        /// <summary>Common shortcut to HathoraSdk.Config.AppId</summary>
        protected string AppId => HathoraSdk.Config.AppId;
        #endregion // Vars
        

        #region Init
        /// <summary>
        /// Init anytime before calling an API to ensure AppId + Auth is set.
        /// - Clients auth via Client Auth API
        /// - Servers auth via Auth0 HathoraDevToken
        /// </summary>
        /// <param name="_hathoraSdk">Leave null to get default from ClientMgr</param>
        protected HathoraApiWrapperBase(HathoraSDK _hathoraSdk) =>
            this.HathoraSdk = _hathoraSdk;
        #endregion // Init
        
        
        #region Utils
        protected string ToJson<T>(T Obj, bool prettify = true)
        {
            Formatting formatting = prettify ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(Obj, formatting);
        }
        #endregion // Utils
    }
}

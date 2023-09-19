// Created by dylan@hathora.dev

using HathoraCloud;
using Newtonsoft.Json;

namespace Hathora.Core.Scripts.Runtime.Common.ApiWrapper
{
    /// <summary>
    /// Common high-level helper of common utils for Client/Server API wrappers.
    /// - Stores HathoraCloudSDK
    /// - Shortcut to commonly-used AppId
    /// - ToJson helper to serialize [+prettify] API requests/results. 
    /// </summary>
    public abstract class HathoraApiWrapperBase
    {
        #region Vars
        private HathoraCloudSDK HathoraSdk { get; set; }

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
        protected HathoraApiWrapperBase(HathoraCloudSDK _hathoraSdk) =>
            this.HathoraSdk = _hathoraSdk;
        #endregion // Init
        
        
        #region Utils
        protected static string ToJson<T>(T Obj, bool prettify = true)
        {
            // Ignore problematic props like ReadTimeout on MemoryQueueBufferStream from UnityWebRequests
            JsonSerializerSettings settings = new()
            {
                Error = (sender, args) =>
                {
                    string member = args.ErrorContext.Member.ToString();
                    bool isDisposableCantSerialize = member is "ReadTimeout" or "WriteTimeout";
                    
                    if (isDisposableCantSerialize)
                        args.ErrorContext.Handled = true;
                },
            };
            
            Formatting formatting = prettify ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(Obj, formatting, settings);
        }
        #endregion // Utils
    }
}

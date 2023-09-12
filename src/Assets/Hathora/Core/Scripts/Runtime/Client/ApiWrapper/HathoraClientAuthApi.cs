// Created by dylan@hathora.dev

using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Client.Config;
using Hathora.Core.Scripts.Runtime.Client.Models;
using HathoraSdk;
using HathoraSdk.Models.Shared;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Client.ApiWrapper
{
    /// <summary>
    /// High-level API wrapper for the low-level Hathora SDK's Auth API.
    /// * Caches SDK Config and HathoraClientConfig for API use. 
    /// * Try/catches async API calls and [Base] automatically handlles API Exceptions.
    /// * Due to code autogen, the SDK exposes too much: This simplifies and minimally exposes.
    /// * Due to code autogen, the SDK sometimes have nuances: This provides fixes/workarounds.
    /// * Call Init() to pass HathoraClientConfig + Hathora SDK Config (see HathoraClientMgr).
    /// * Does not handle UI (see HathoraClientMgrUi).
    /// * Does not handle Session caching (see HathoraClientSession).
    /// </summary>
    public class HathoraClientAuthApi : HathoraClientApiWrapperBase
    {
        private AuthV1SDK authApi;

        /// <summary>
        /// </summary>
        /// <param name="_hathoraClientConfig"></param>
        /// <param name="_hathoraSdkConfig"></param>
        public override void Init(
            HathoraClientConfig _hathoraClientConfig,
            SDKConfig _hathoraSdkConfig = null)
        {
            Debug.Log($"[{nameof(HathoraClientAuthApi)}] Initializing API...");
            
            // TODO: `Configuration` is missing in the new SDK - cleanup, if permanently gone.
            // base.Init(_hathoraClientConfig, _hathoraSdkConfig);
            // this.authApi = new AuthV1SDK(base.HathoraSdkConfig);
            
            base.Init(_hathoraClientConfig);
            
            // TODO: Manually init w/out constructor, or add constructor support to model
            this.authApi = new AuthV1SDK();
        }


        #region Client Auth Async Hathora SDK Calls
        /// <param name="_cancelToken"></param>
        /// <returns>Returns AuthResult on success</returns>
        public async Task<AuthResult> ClientAuthAsync(CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraClientAuthApi)}.{nameof(ClientAuthAsync)}]"; 
            Debug.Log($"{logPrefix} Start");
            
            LoginResponse anonLoginResult;
            try
            {
                anonLoginResult = await authApi.LoginAnonymousAsync(
                    HathoraClientConfig.AppId, 
                    _cancelToken);
            }
            catch (ApiException apiException)
            {
                HandleApiException(
                    nameof(HathoraClientAuthApi),
                    nameof(ClientAuthAsync), 
                    apiException);
                return null;
            }

            bool isAuthed = !string.IsNullOrEmpty(anonLoginResult?.Token); 
            
            
#if UNITY_EDITOR
            // For security, we probably only want to log this in the editor
            
            //// TODO: `ToJson()` no longer exists in request/response models, but should soon make a return?
            // Debug.Log($"{logPrefix} isAuthed: {isAuthed}, " +
            //     $"<color=yellow>anonLoginResult: {anonLoginResult?.ToJson()}</color>");
            Debug.Log($"{logPrefix} isAuthed: {isAuthed}");
#else
            Debug.Log($"{logPrefix} isAuthed: {isAuthed}");
#endif
            
            
            return isAuthed
                ? new AuthResult(anonLoginResult.Token)
                : null;

        }
        #endregion // Server Auth Async Hathora SDK Calls
    }
}

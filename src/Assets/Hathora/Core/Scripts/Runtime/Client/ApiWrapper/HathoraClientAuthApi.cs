// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Client.Config;
using HathoraSdk;
using HathoraSdk.Models.Operations;
using HathoraSdk.Models.Shared;
using HathoraSdk.Utils;
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
            base.Init(_hathoraClientConfig, _hathoraSdkConfig);
            
            // TODO: Overloading VxSDK constructor with nulls, for now, until we know how to properly construct
            SpeakeasyHttpClient httpClient = null;
            string serverUrl = null;
            this.authApi = new AuthV1SDK(
                httpClient,
                httpClient, 
                serverUrl,
                HathoraSdkConfig);
        }


        #region Client Auth Async Hathora SDK Calls
        /// <param name="_cancelToken">TODO</param>
        /// <returns>Returns AuthResult on success</returns>
        public async Task<LoginResponse> ClientAuthAsync(CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraClientAuthApi)}.{nameof(ClientAuthAsync)}]"; 
            Debug.Log($"{logPrefix} Start");

            LoginAnonymousRequest anonLoginRequest = new() { AppId = HathoraClientConfig.AppId }; 
            LoginAnonymousResponse loginAnonResponse = null;
            
            try
            {
                loginAnonResponse = await authApi.LoginAnonymousAsync(anonLoginRequest);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(authApi.LoginAnonymousAsync)} => Error: {e.Message}");
                return null; // fail
            }

            bool isAuthed = !string.IsNullOrEmpty(loginAnonResponse.LoginResponse?.Token); 
            
            
#if UNITY_EDITOR
            // For security, we probably only want to log this in the editor
            Debug.Log($"{logPrefix} <color=yellow>{nameof(isAuthed)}: {isAuthed}, " +
                $"{nameof(loginAnonResponse)}: {base.ToJson(loginAnonResponse)}</color>");
#else
            Debug.Log($"{logPrefix} {nameof(isAuthed)}: {isAuthed}");
#endif


            return loginAnonResponse.LoginResponse;
        }
        #endregion // Server Auth Async Hathora SDK Calls
    }
}

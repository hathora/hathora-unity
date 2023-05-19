// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Client.Models;
using Hathora.Scripts.Net.Common;
using UnityEngine;

namespace Hathora.Scripts.Net.Client.ApiWrapper
{
    /// <summary>
    /// * Call Init() to pass UserConfig/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientAuthApi : NetHathoraApiBase
    {
        private AuthV1Api authApi;

        
        public override void Init(
            Configuration _hathoraSdkConfig, 
            NetHathoraConfig _netHathoraConfig, 
            NetSession _netSession)
        {
            Debug.Log("[NetHathoraClientAuthApi] Initializing API...");
            base.Init(_hathoraSdkConfig, _netHathoraConfig, _netSession);
            this.authApi = new AuthV1Api(_hathoraSdkConfig);
        }


        #region Client Auth Async Hathora SDK Calls
        /// <returns>Returns AuthResult on success</returns>
        public async Task<AuthResult> ClientAuthAsync()
        {
            LoginResponse anonLoginResult;
            try
            {
                anonLoginResult = await authApi.LoginAnonymousAsync(NetHathoraConfig.HathoraCoreOpts.AppId);
            }
            catch (ApiException apiException)
            {
                HandleClientApiException(
                    nameof(NetHathoraClientAuthApi),
                    nameof(ClientAuthAsync), 
                    apiException);
                return null;
            }

            bool isAuthed = !string.IsNullOrEmpty(anonLoginResult?.Token); 
            Debug.Log($"[NetHathoraClientAuthApi] isAuthed: {isAuthed}");

            if (!isAuthed)
                return null;
            
            NetSession.InitNetSession(anonLoginResult.Token);
            return new AuthResult(anonLoginResult.Token);
        }
        #endregion // Server Auth Async Hathora SDK Calls
    }
}

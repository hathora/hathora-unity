// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Client.Models;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.Net.Server;
using UnityEngine;

namespace Hathora.Scripts.Net.Client.ApiWrapper
{
    /// <summary>
    /// * Call Init() to pass config/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientAuthApi : NetHathoraApiBase
    {
        private AuthV1Api authApi;

        
        public override void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _netSession)
        {
            Debug.Log("[NetHathoraClientAuthApi] Initializing API...");
            base.Init(_hathoraSdkConfig, _hathoraServerConfig, _netSession);
            this.authApi = new AuthV1Api(_hathoraSdkConfig);
        }


        #region Client Auth Async Hathora SDK Calls
        /// <summary>
        /// Calls 
        /// </summary>
        /// <returns>Returns AuthResult on success</returns>
        public async Task<AuthResult> ClientAuthAsync()
        {
            LoginAnonymous200Response anonLoginResult;
            try
            {
                anonLoginResult = await authApi.LoginAnonymousAsync(hathoraServerConfig.AppId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClientAuthApi]**ERR @ ServerAuthAsync " +
                    $"(LoginAnonymousAsync): {e.Message}");
                await Task.FromException<Exception>(e);
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

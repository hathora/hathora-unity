// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Client.Models;
using Hathora.Net.Server;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// * Call Init() to pass config/instances.
    /// * Does not handle UI.
    /// </summary>
    public class NetHathoraClientAuth : NetHathoraApiBase
    {
        private AuthV1Api authApi;

        
        public override void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _playerSession)
        {
            Debug.Log("[NetHathoraClientAuth] Initializing API...");
            base.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
            this.authApi = new AuthV1Api(_hathoraSdkConfig);
        }


        #region Client Auth Async Hathora SDK Calls
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns AuthResult on success</returns>
        public async Task<AuthResult> ClientAuthAsync()
        {
            if (!base.IsClient)
                return null;

            LoginAnonymous200Response anonLoginResult;
            try
            {
                anonLoginResult = await authApi.LoginAnonymousAsync(hathoraServerConfig.AppId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraClientAuth]**ERR @ ServerAuthAsync " +
                    $"(LoginAnonymousAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return null;
            }

            bool isAuthed = !string.IsNullOrEmpty(anonLoginResult?.Token); 
            Debug.Log($"[NetHathoraClientAuth] isAuthed: {isAuthed}");

            if (!isAuthed)
                return null;
            
            PlayerSession.InitNetSession(anonLoginResult.Token);
            return new AuthResult(anonLoginResult.Token);
        }
        #endregion // Server Auth Async Hathora SDK Calls
    }
}

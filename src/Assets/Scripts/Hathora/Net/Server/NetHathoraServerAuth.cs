// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Server.Models;
using UnityEngine;

namespace Hathora.Net.Server
{
    /// <summary>
    /// Call base.Init() to pass dev tokens, etc.
    /// </summary>
    public class NetHathoraServerAuth : HathoraNetServerApiBase
    {
        private AuthV1Api authApi;

        public override void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _playerSession)
        {
            base.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
            this.authApi = new AuthV1Api(_hathoraSdkConfig);
        }


        #region Event Delegates
        /// <summary>=> isSuccess</summary>
        public event EventHandler<bool> AuthComplete;
        #endregion // Event Delegates

        
        #region Server Auth Async Hathora SDK Calls
        public async Task ServerAuthAsync()
        {
            if (!base.IsServer)
                return;

            LoginAnonymous200Response anonLoginResult;
            try
            {
                anonLoginResult = await authApi.LoginAnonymousAsync(hathoraServerConfig.AppId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraServerAuth]**ERR @ ServerAuthAsync " +
                    $"(LoginAnonymousAsync): {e.Message}");
                onServerAuthFail();
                await Task.FromException<Exception>(e);
                return;
            }

            bool isAuthed = !string.IsNullOrEmpty(anonLoginResult?.Token); 
            Debug.Log($"[NetHathoraServerAuth] isAuthed: {isAuthed}");
            
            if (isAuthed)
                onServerAuthSuccess(anonLoginResult.Token);
            else
                onServerAuthFail();
        }
        #endregion // Server Auth Async Hathora SDK Calls
        
        
        #region Success Callbacks
        private void onServerAuthSuccess(string playerAuthToken)
        {
            PlayerSession.InitNetSession(playerAuthToken, hathoraServerConfig.DevAuthToken);
            
            const bool isSucccess = true;
            AuthComplete?.Invoke(this, isSucccess);
        }
        #endregion // Success Callbacks
        
        
        #region Fail Callbacks
        private void onServerAuthFail()
        {
            const bool isSucccess = false;
            AuthComplete?.Invoke(this,isSucccess);
        }
        #endregion // Fail Callbacks
    }
}

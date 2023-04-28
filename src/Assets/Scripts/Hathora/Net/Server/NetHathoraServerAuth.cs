// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
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
        public event EventHandler<bool> CreateAuthComplete;
        #endregion // Event Delegates

        
        #region Server Auth Async Hathora SDK Calls
        public async Task ServerCreateAuthAsync(CreateAuthRequest.VisibilityEnum authVisibility)
        {
            if (!base.IsServer)
                return;

            AuthInitConfig authInitConfig = new();
            CreateAuthRequest request = new CreateAuthRequest(
                authVisibility, 
                authInitConfig, 
                hathoraServerConfig.Region);

            Auth auth;
            try
            {
                auth = await authApi.CreateAuthAsync(
                    hathoraServerConfig.AppId,
                    hathoraServerConfig.DevAuthToken,
                    request);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraPlayer]**ERR @ ServerCreateAuthAsync (CreateAuthAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                onCreateAuthFail();
                return;
            }

            Debug.Log($"[NetHathoraPlayer] ServerCreateAuthAsync => roomId: {auth.RoomId}");

            if (auth != null)
                onServerCreateAuthSuccess(auth);
        }
        #endregion // Server Auth Async Hathora SDK Calls
        
        
        #region Success Callbacks
        private void onServerCreateAuthSuccess(Auth auth)
        {
            Debug.LogWarning("[NetHathoraServerAuth] TODO @ " +
                "onServerCreateAuthSuccess: Cache auth @ session");
            
            // PlayerSession.Auth = auth; // TODO
            CreateAuthComplete?.Invoke(this, auth);
        }
        #endregion // Success Callbacks
        
        
        #region Fail Callbacks
        private void onCreateAuthFail()
        {
            const Auth auth = null;
            CreateAuthComplete?.Invoke(this, auth);
        }
        #endregion // Fail Callbacks
    }
}

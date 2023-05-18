// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor.Auth0;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Editor
{
    /// <summary>
    /// Dev auth to get a dev token. Browser will launch to OAuth (via Auth0) and we'll
    /// tell the server we want a code. After a code, we'll launch a browser for the user
    /// to auth (via UI) and poll every 5s until we are authed.
    /// Timeout is ~5m, supporting cancel tokens. 
    /// </summary>
    public static class HathoraServerAuth
    {
        public static CancellationTokenSource ActiveCts { get; private set; }

        public static bool HasCancellableToken =>
            ActiveCts?.Token is { CanBeCanceled: true };
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_netHathoraConfig"></param>
        public static async Task DevAuthLogin(NetHathoraConfig _netHathoraConfig) 
        {
            // Cancel an old op 1st
            if (ActiveCts != null && ActiveCts.Token.CanBeCanceled)
                ActiveCts.Cancel();
 
            ActiveCts = new CancellationTokenSource(
                TimeSpan.FromMinutes(Auth0Login.PollTimeoutMins));

            Auth0Login auth = new(); 
            string refreshToken = await auth.GetTokenAsync(
                _netHathoraConfig, 
                ActiveCts.Token);
             
            if (string.IsNullOrEmpty(refreshToken))
            {
                if (ActiveCts != null && HasCancellableToken)
                    ActiveCts?.Cancel();
                
                return;
            }
            
            _netHathoraConfig.HathoraCoreOpts.DevAuthOpts.DevAuthToken = refreshToken;
            Debug.Log("[HathoraServerBuild] Dev Auth0 login successful: " +
                "Token set @ NetHathoraConfig");
        }
    }
}

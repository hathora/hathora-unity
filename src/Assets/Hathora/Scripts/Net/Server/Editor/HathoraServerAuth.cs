// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Scripts.Net.Server.Editor.Auth0;
using UnityEngine;

namespace Hathora.Scripts.Net.Server.Editor
{
    /// <summary>
    /// Dev auth to get a dev token. Browser will launch to OAuth (via Auth0) and we'll
    /// tell the server we want a code. After a code, we'll launch a browser for the user
    /// to auth (via UI) and poll every 5s until we are authed.
    /// Timeout is ~5m, supporting cancel tokens. 
    /// </summary>
    public static class HathoraServerAuth
    {
        private static CancellationTokenSource ActiveCts;
        
        public static async Task DevAuthLogin(HathoraServerConfig hathoraServerConfig)
        {
            // Cancel an old op 1st
            if (ActiveCts?.Token is { CanBeCanceled: true })
                ActiveCts.Cancel();
            
            ActiveCts = new CancellationTokenSource();
            ActiveCts.CancelAfter(TimeSpan.FromMinutes(Auth0Login.PollTimeoutMins));

            Auth0Login auth = new();
            string refreshToken = await auth.GetTokenAsync(hathoraServerConfig, ActiveCts.Token); // Refresh token lasts longer

            if (string.IsNullOrEmpty(refreshToken))
            {
                // Debug.LogError("[HathoraServerBuild] Dev Auth0 login failed: " +
                //     "Refresh token is null or empty");
                return;
            }
            
            hathoraServerConfig.SetDevToken(refreshToken);
            Debug.Log("[HathoraServerBuild] Dev Auth0 login successful: " +
                "Token set @ HathoraServerConfig");
        }
    }
}

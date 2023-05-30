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
        /// </summary>
        /// <param name="_netHathoraConfig"></param>
        /// <returns>isSuccess</returns>
        public static async Task<bool> DevAuthLogin(NetHathoraConfig _netHathoraConfig)
        {
            createNewAuthCancelToken();
            Auth0Login auth = new(); 
            string refreshToken = await auth.GetTokenAsync(cancelToken: ActiveCts.Token);
            
            bool isSuccess = onGetTokenDone(
                _netHathoraConfig, 
                refreshToken);
            
            return isSuccess;
        }

        /// <summary>
        /// </summary>
        /// <param name="_netHathoraConfig"></param>
        /// <param name="_refreshToken"></param>
        /// <returns>isSuccess</returns>
        private static bool onGetTokenDone(
            NetHathoraConfig _netHathoraConfig,
            string _refreshToken)
        {
            if (string.IsNullOrEmpty(_refreshToken))
            {
                if (ActiveCts != null && HasCancellableToken)
                    onGetTokenCancelled();

                return false; // !isSuccess
            }
            
            SetAuthToken(_netHathoraConfig, _refreshToken);
            return true; // isSuccess
        }

        private static void onGetTokenCancelled() =>
            ActiveCts?.Cancel();

        private static void createNewAuthCancelToken()
        {
            // Cancel an old op 1st
            if (ActiveCts != null && ActiveCts.Token.CanBeCanceled)
                ActiveCts.Cancel();
 
            ActiveCts = new CancellationTokenSource(
                TimeSpan.FromMinutes(Auth0Login.PollTimeoutMins));
        }

        /// <summary>Shortcut to set dev auth token with log</summary>
        /// <param name="_netHathoraConfig"></param>
        /// <param name="_token">You probably want the refreshToken</param>
        public static void SetAuthToken(NetHathoraConfig _netHathoraConfig, string _token)
        {
            _netHathoraConfig.HathoraCoreOpts.DevAuthOpts.DevAuthToken = _token;
            Debug.Log("[HathoraServerBuild] Dev Auth0 login successful: " +
                "Token set @ NetHathoraConfig");
        }
    }
}

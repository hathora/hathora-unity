// Created by dylan@hathora.dev

using System;
using FishNet.Authenticating;
using FishNet.Connection;
using UnityEngine;

namespace Hathora.Net
{
    /// <summary>
    /// FishNet Authenticator: This enforces the user to authenticate before they can connect to the server.
    /// </summary>
    public class NetAuthenticator : Authenticator
    {
        public override event Action<NetworkConnection, bool> OnAuthenticationResult;

        private void Start()
        {
            OnAuthenticationResult += (connection, isSuccess) =>
            {
                if (isSuccess)
                    Debug.Log($"[NetAuthenticator] OnAuthenticationResult - Connection '{connection}' authenticated.");
                else
                    Debug.Log($"[NetAuthenticator] OnAuthenticationResult - Connection '{connection}' failed authentication.");
            };
        }
    }
}

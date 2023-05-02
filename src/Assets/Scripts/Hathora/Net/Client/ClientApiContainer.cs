// Created by dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Client;
using Hathora.Net.Server;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// API wrapper container to serialize in HathoraPlayer
    /// </summary>
    [Serializable]
    public struct ClientApiContainer
    {
        [SerializeField]
        public NetHathoraClientAuth AuthApi;
        
        [SerializeField]
        public NetHathoraClientLobby LobbyApi;

        
        public void InitAll(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _playerSession)
        {
            AuthApi.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
            LobbyApi.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
        }
    }
}

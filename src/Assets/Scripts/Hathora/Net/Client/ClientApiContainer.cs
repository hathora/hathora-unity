// Created by dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Client;
using Hathora.Net.Server;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// API wrapper container to serialize in HathoraPlayer.
    /// Have a new Hathora API to add?
    /// 1. Serialize it here and add to InitAll().
    /// 2. Open `Player` prefab >> Add new API script component to NetworkManager.
    /// 3. Drag the new API component to the serialized field here.
    /// </summary>
    [Serializable]
    public struct ClientApiContainer
    {
        [SerializeField]
        public NetHathoraClientAuth AuthApi;
        
        [SerializeField]
        public NetHathoraClientLobby LobbyApi;

        [SerializeField]
        public NetHathoraClientRoom RoomApi;
        
        public void InitAll(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _playerSession)
        {
            AuthApi.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
            LobbyApi.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
            RoomApi.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
        }
    }
}

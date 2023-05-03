// Created by dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Client;
using Hathora.Net.Client.ApiWrapper;
using Hathora.Net.Server;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("AuthApi")]
        [SerializeField]
        public NetHathoraClientAuthApi authApiApi;
        
        [FormerlySerializedAs("LobbyApi")]
        [SerializeField]
        public NetHathoraClientLobbyApi lobbyApiApi;

        [FormerlySerializedAs("RoomApi")]
        [SerializeField]
        public NetHathoraClientRoomApi roomApiApi;
        
        public void InitAll(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _netSession)
        {
            authApiApi.Init(_hathoraSdkConfig, _hathoraServerConfig, _netSession);
            lobbyApiApi.Init(_hathoraSdkConfig, _hathoraServerConfig, _netSession);
            roomApiApi.Init(_hathoraSdkConfig, _hathoraServerConfig, _netSession);
        }
    }
}

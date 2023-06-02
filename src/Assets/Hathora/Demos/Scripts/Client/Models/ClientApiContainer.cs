// Created by dylan@hathora.dev

using System;
using Hathora.Scripts.Client;
using Hathora.Scripts.Client.ApiWrapper;
using Hathora.Scripts.Common;
using Hathora.Scripts.Sdk.hathora_cloud_sdks.csharp.src.Hathora.Cloud.Sdk.Client;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Demos.Scripts.Client.Models
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
        [FormerlySerializedAs("authApi")]
        [SerializeField]
        public NetHathoraClientClientAuthApi clientAuthApi;
        
        [FormerlySerializedAs("lobbyApi")]
        [SerializeField]
        public NetHathoraClientClientLobbyApi clientLobbyApi;

        [FormerlySerializedAs("roomApi")]
        [SerializeField]
        public NetHathoraClientClientRoomApi clientRoomApi;
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_netHathoraConfig"></param>
        /// <param name="_netSession"></param>
        /// <param name="_hathoraSdkConfig">We'll automatically create this, if empty</param>
        public void InitAll(
            NetHathoraConfig _netHathoraConfig, 
            NetSession _netSession,
            Configuration _hathoraSdkConfig = null)
        {
            clientAuthApi.Init(_netHathoraConfig, _netSession, _hathoraSdkConfig);
            clientLobbyApi.Init(_netHathoraConfig, _netSession, _hathoraSdkConfig);
            clientRoomApi.Init(_netHathoraConfig, _netSession, _hathoraSdkConfig);
        }
    }
}

// Created by dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Client;
using Hathora.Core.Scripts.Runtime.Client;
using Hathora.Core.Scripts.Runtime.Client.ApiWrapper;
using Hathora.Core.Scripts.Runtime.Client.Config;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Demos.Shared.Scripts.Client.Models
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
        public NetHathoraClientAuthApi clientAuthApi;
        
        [FormerlySerializedAs("lobbyApi")]
        [SerializeField]
        public NetHathoraClientLobbyApi clientLobbyApi;

        [FormerlySerializedAs("roomApi")]
        [SerializeField]
        public NetHathoraClientRoomApi clientRoomApi;
        
        
        /// <summary>
        /// </summary>
        /// <param name="_netHathoraConfig"></param>
        /// <param name="_netSession"></param>
        /// <param name="_hathoraSdkConfig">We'll automatically create this, if empty</param>
        public void InitAll(
            HathoraClientConfig _netHathoraConfig, 
            NetSession _netSession,
            Configuration _hathoraSdkConfig = null)
        {
            clientAuthApi.Init(_netHathoraConfig, _netSession, _hathoraSdkConfig);
            clientLobbyApi.Init(_netHathoraConfig, _netSession, _hathoraSdkConfig);
            clientRoomApi.Init(_netHathoraConfig, _netSession, _hathoraSdkConfig);
        }
    }
}

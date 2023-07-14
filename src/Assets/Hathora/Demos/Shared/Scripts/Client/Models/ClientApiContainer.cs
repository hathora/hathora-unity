// Created by dylan@hathora.dev

using System;
using Hathora.Core.Scripts.Runtime.Client.ApiWrapper;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Demos.Shared.Scripts.Client.Models
{
    /// <summary>
    /// Client API wrapper container to serialize in HathoraPlayer.
    /// Have a new Hathora API to add?
    /// 
    /// 1. Serialize it here, and add to HathoraClientMgrBase.InitApis()
    /// 
    /// 2. Open scene HathoraManager (not prefab) >> Add new API script component
    ///    to Hathora{Platform}ClientMgr.ClientApis >> serialize the script component
    /// </summary>
    [Serializable]
    public struct ClientApiContainer
    {
        [FormerlySerializedAs("authApi")]
        [SerializeField]
        private HathoraNetClientAuthApi clientAuthApi;
        public HathoraNetClientAuthApi ClientAuthApi => clientAuthApi;
        
        [FormerlySerializedAs("lobbyApi")]
        [SerializeField]
        private HathoraNetClientLobbyApi clientLobbyApi;
        public HathoraNetClientLobbyApi ClientLobbyApi => clientLobbyApi;

        [FormerlySerializedAs("roomApi")]
        [SerializeField]
        private HathoraNetClientRoomApi clientRoomApi;
        public HathoraNetClientRoomApi ClientRoomApi => clientRoomApi;
    }
}

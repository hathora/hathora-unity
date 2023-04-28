// Created by dylan@hathora.dev

using System;
using UnityEngine;

namespace Hathora.Net.Server.Models
{
    /// <summary>
    /// API wrapper container to serialize in HathoraPlayer
    /// </summary>
    [Serializable]
    public struct ServerApiContainer
    {
        [SerializeField]
        public NetHathoraServerRoom RoomApi;
    }
}

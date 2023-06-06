// dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Common.Models;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Server.Models
{
    /// <summary>
    /// We just created a Room from ServerConfig, containing Room + ConnectionInfo (v2+).
    /// </summary>
    [Serializable]
    public class HathoraCachedRoomConnection
    {
        [SerializeField]
        private RoomWrapper _roomWrapper;
        public Room Room
        {
            get => _roomWrapper.ToRoomType();
            set => _roomWrapper = new RoomWrapper(value);
        }
        
        [SerializeField]
        private ConnectionInfoV2Wrapper _connectionInfoV2Wrapper;
        public ConnectionInfoV2 ConnectionInfoV2
        {
            get => _connectionInfoV2Wrapper.ToConnectionInfoV2Type();
            set => _connectionInfoV2Wrapper = new ConnectionInfoV2Wrapper(value);
        }
        
        
        public HathoraCachedRoomConnection(
            Room _room, 
            ConnectionInfoV2 _connectionInfoV2)
        {
            this.Room = _room;
            this.ConnectionInfoV2 = _connectionInfoV2;
        }
    }
}

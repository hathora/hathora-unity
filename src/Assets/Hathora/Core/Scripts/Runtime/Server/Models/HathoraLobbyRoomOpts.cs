// dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Common.Models;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Server.Models
{
    [Serializable]
    public class HathoraLobbyRoomOpts
    {
        #region Hathora Region
        /// <summary>
        /// (!) Hathora SDK Enums starts at index 1; not 0: Care of indexes.
        /// Since this Enum isn't alphabatized, also care if you Sort() the list.
        /// </summary>
        [SerializeField]
        private Region _hathoraRegion = Region.Seattle;
        
        /// <summary>
        /// (!) Hathora SDK Enums starts at index 1; not 0: Care of indexes.
        /// Since this Enum isn't alphabatized, also care if you Sort() the list.
        /// </summary>
        public Region HathoraRegion
        { 
            get => _hathoraRegion;
            set => _hathoraRegion = value;
        }

        /// <summary>
        /// (!) Hathora SDK Enums starts at index 1; not 0: Care of indexes.
        /// Since this Enum isn't alphabatized, also care if you Sort() the list.
        /// </summary>
        [SerializeField]
        private int _regionSelectedIndex = (int)Region.Seattle;
        
        /// <summary>
        /// (!) Hathora SDK Enums starts at index 1; not 0: Care of indexes.
        /// Since this Enum isn't alphabatized, also care if you Sort() the list.
        /// </summary>
        public int RegionSelectedIndex
        {
            get => _regionSelectedIndex;
            set => _regionSelectedIndex = value;
        }
        

        [SerializeField]
        private RoomWrapper _lastCreatedRoomInfo;
        public Room LastCreatedRoomInfo
        {
            get => _lastCreatedRoomInfo.ToRoomType();
            set => _lastCreatedRoomInfo = new RoomWrapper(value);
        }
        
        public bool HasLastCreatedRoomInfo => _lastCreatedRoomInfo != null;
        
        
        [SerializeField]
        private ConnectionInfoV2Wrapper _lastCreatedRoomConnectionInfo;
        public ConnectionInfoV2 LastCreatedRoomConnectionInfo
        {
            get => _lastCreatedRoomConnectionInfo.ToConnectionInfoV2Type();
            set => _lastCreatedRoomConnectionInfo = new ConnectionInfoV2Wrapper(value);
        }
        
        public bool HasLastCreatedRoomConnectionInfo => _lastCreatedRoomConnectionInfo != null;
        #endregion // Hathora Region
    }
}
 
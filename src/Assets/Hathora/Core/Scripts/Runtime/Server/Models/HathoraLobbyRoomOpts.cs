// dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace Hathora.Core.Scripts.Runtime.Server.Models
{
    [Serializable] 
    public class HathoraLobbyRoomOpts
    {
        #region Hathora Region
        /// <summary>(!) Hathora SDK Enums starts at index 1; not 0: Care of indexes</summary>
        [FormerlySerializedAs("_hathoraRegionSelectedIndexUi")]
        [SerializeField]
        private int _hathoraRegionSelectedIndex = (int)Region.Seattle;

        /// <summary>(!) Hathora SDK Enums starts at index 1; not 0: Care of indexes</summary>
        public int HathoraRegionSelectedIndex
        {
            get => _hathoraRegionSelectedIndex;
            set => _hathoraRegionSelectedIndex = value;
        }

        /// <summary>
        /// (!) Hathora SDK Enums starts at index 1; not 0: Care of indexes.
        /// Since this Enum isn't alphabatized, also care if you Sort() the list.
        /// </summary>
        public Region SelectedHathoraRegion => 
            (Region)_hathoraRegionSelectedIndex;
        #endregion // Hathora Region
        
        
        #region Cached Room Connection
        // [SerializeField] // While Rooms last only 5m, don't actually persist this
        private HathoraCachedRoomConnection _lastCreatedRoomConnection;
        public HathoraCachedRoomConnection LastCreatedRoomConnection
        {
            get => _lastCreatedRoomConnection;
            set => _lastCreatedRoomConnection = value;
        }

        
        /// <summary>
        /// We check if there's a RoomId, and null checking leading up to it.
        /// </summary>
        public bool HasLastCreatedRoomConnection => 
            !string.IsNullOrEmpty(_lastCreatedRoomConnection?.Room?.RoomId ?? string.Empty) && 
            _lastCreatedRoomConnection?.ConnectionInfoV2?.ExposedPort != null;

        /// <summary>
        /// Checks if room has IsError *only*. Returns false if connection is null.
        /// </summary>
        public bool HasLastCreatedRoomConnectionErr => _lastCreatedRoomConnection?.IsError ?? false;
        #endregion // Cached Room Connection
    }
}
 
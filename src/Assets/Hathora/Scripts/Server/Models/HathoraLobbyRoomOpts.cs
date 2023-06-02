// dylan@hathora.dev

using System;
using UnityEngine;

namespace Hathora.Scripts.Server.Models
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
        private Room _lastCreatedRoomInfo;
        public Room LastCreatedRoomInfo
        {
            get => _lastCreatedRoomInfo;
            set => _lastCreatedRoomInfo = value;
        }
        
        public bool HasLastCreatedRoomInfo => _lastCreatedRoomInfo != null;
        
        #endregion // Hathora Region
    }
}
 
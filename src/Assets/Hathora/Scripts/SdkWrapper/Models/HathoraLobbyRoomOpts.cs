// dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Client.Models;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Models
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
        public int _regionSelectedIndex;
        
        /// <summary>
        /// (!) Hathora SDK Enums starts at index 1; not 0: Care of indexes.
        /// Since this Enum isn't alphabatized, also care if you Sort() the list.
        /// </summary>
        public int RegionSelectedIndex
        {
            get => _regionSelectedIndex;
            set => _regionSelectedIndex = value;
        }
        #endregion // Hathora Region

        
        #region Lobby Settings (Optional)
        [SerializeField]
        private string _initConfigJson = new InitConfigExample().ToString();
        public string InitConfigJson
        {
            get => _initConfigJson;
            set => _initConfigJson = value;
        }
        #endregion // Lobby Settings (Optional)
    }
}
 
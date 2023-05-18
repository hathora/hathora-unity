// dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraLobbyRoomOpts
    {
        [SerializeField]
        private Region _hathoraRegion = Region.Seattle;
        public Region HathoraRegion
        {
            get => _hathoraRegion;
            set => _hathoraRegion = value;
        }
            
        // Public utils
            
        /// <summary>
        /// Explicit typings for FindNestedProperty() calls
        /// </summary>
        public struct SerializedFieldNames
        {
            public static string HathoraRegion => nameof(_hathoraRegion);
        }
    }
}

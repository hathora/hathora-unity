// dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Models
{
    public class HathoraLobbyRoomOpts
    {
        [SerializeField]
        private Region _hathoraRegion = Region.Seattle;
        public Region HathoraRegion
        { 
            get => _hathoraRegion;
            set => _hathoraRegion = value;
        }

        [SerializeField]
        public int _regionSelectedIndex;
        public int RegionSelectedIndex
        {
            get => _regionSelectedIndex;
            set => _regionSelectedIndex = value;
        }
    }
}
 
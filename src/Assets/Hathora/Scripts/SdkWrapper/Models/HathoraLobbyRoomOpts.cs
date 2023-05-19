// dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;

namespace Hathora.Scripts.SdkWrapper.Models
{
    public class HathoraLobbyRoomOpts
    {
        private Region _hathoraRegion = Region.Seattle;
        public Region HathoraRegion
        { 
            get => _hathoraRegion;
            set => _hathoraRegion = value;
        }
    }
}
 
// dylan@hathora.dev

using System;
using System.Globalization;
using Hathora.Core.Scripts.Runtime.Common.Extensions;
using Hathora.Core.Scripts.Runtime.Common.Models;
using Hathora.Core.Scripts.Runtime.Common.Utils;
using HathoraSdk.Models.Shared;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Core.Scripts.Runtime.Server.Models
{
    /// <summary>
    /// We just created a Room from ServerConfig, containing Room + ConnectionInfo (v2+).
    /// </summary>
    [Serializable]
    public class HathoraCachedRoomConnection
    {
        [SerializeField]
        private Region _hathoraRegion = Region.Seattle;
        public Region HathoraRegion
        {
            get => _hathoraRegion;
            set => _hathoraRegion = value;
        }

        /// <summary>WashingtonDC => "Washington DC"</summary>
        public string GetFriendlyRegionStr() => 
            Enum.GetName(typeof(Region), _hathoraRegion)?.SplitPascalCase();
        
        [FormerlySerializedAs("_roomWrapper")]
        [SerializeField]
        private Room _room;
        public Room Room
        {
            get => _room;
            set => _room = value;
        }
        
        [FormerlySerializedAs("_connectionInfoV2Wrapper")]
        [SerializeField]
        private ConnectionInfoV2 _connectionInfoV2;
        public ConnectionInfoV2 ConnectionInfoV2
        {
            get => _connectionInfoV2;
            set => _connectionInfoV2 = value;
        }

        public bool IsError { get; set; }
        public string ErrReason { get; set; }

        public HathoraCachedRoomConnection(
            Region _region,
            Room _room, 
            ConnectionInfoV2 _connectionInfoV2)
        {
            // (!) We use `public` setters in case there are SDK wrapper workarounds
            this.HathoraRegion = _region;
            this.Room = _room;
            this.ConnectionInfoV2 = _connectionInfoV2;
        }
        
        /// <summary>
        /// Use this to mock the obj for a failure.
        /// </summary>
        public HathoraCachedRoomConnection()
        {
        }

        /// <summary>
        /// Returns a prettified "host:port".
        /// </summary>
        /// <returns></returns>
        public string GetConnInfoStr()
        {
            string hostStr = _connectionInfoV2 == null
                ? "<MissingHost>"
                : _connectionInfoV2?.ExposedPort?.Host ?? "<MissingHost>";

            double portDbl = _connectionInfoV2 == null
                ? 0
                : _connectionInfoV2?.ExposedPort?.Port ?? 0;
            
            string portStr = portDbl > 0 
                ? portDbl.ToString(CultureInfo.InvariantCulture)
                : "<MissingPort>";

            return $"{hostStr}:{portStr}";
        }
    }
}

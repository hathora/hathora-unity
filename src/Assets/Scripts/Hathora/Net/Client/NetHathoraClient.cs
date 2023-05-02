// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Client;
using Hathora.Net.Server;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Net.Client
{
    /// <summary>
    /// The gateway to all client-side Hathora SDK APIs.
    /// To add scripts, add to ClientApiContainer.
    /// </summary>
    public class NetHathoraClient : MonoBehaviour
    {
        [SerializeField]
        private HathoraServerConfig hathoraServerConfig;
        
        [SerializeField]
        public ClientApiContainer ClientApis;
        
        private Configuration hathoraSdkConfig;

        private NetSession playerSession { get; set; }
        private NetPlayerUI netPlayerUI { get; set; }

        
        /// <summary>
        /// Inits all the Client SDKs.
        /// </summary>
        /// <param name="_playerSession"></param>
        /// <param name="_netPlayerUI"></param>
        public void Init(NetSession _playerSession, NetPlayerUI _netPlayerUI)
        {
            this.playerSession = _playerSession;
            this.netPlayerUI = _netPlayerUI;
            this.hathoraSdkConfig = new Configuration();
            
            ClientApis.InitAll(hathoraSdkConfig, hathoraServerConfig, playerSession);
        }
    }
}

// Created by dylan@hathora.dev

using FishNet;
using FishNet.Transporting;
using FishNet.Transporting.Bayou;
using FishNet.Transporting.Tugboat;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Runtime.Server;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hathora.Demos._1_FishNetDemo.HathoraScripts.Server
{
    public class HathoraFishnetServerMgr : HathoraServerMgrBase
    {
        /// <summary>Shortcuts to the selected Transport instance</summary>
        private static Transport transport
        {
            get => InstanceFinder.TransportManager.Transport;
            set => InstanceFinder.TransportManager.Transport = value;
        }
        
        /// <summary>User-facing friendly Transport options</summary>
        enum UserTransportType
        {
            /// <summary>Tugboat (UDP) Transport</summary>
            UdpDefault,

            /// <summary>Bayou WebGL (TCP/WS) Transport</summary>
            TcpWslWebgl,
            
            /// <summary>Selected HathoraServerConfig transport from Deploy Settings</summary>
            HathoraServerConfigSource,
        }

        [Header("UDP || TCP (WebGL/WS)?")]
        [SerializeField, Tooltip("Set UDP || TCP here, or get from HathoraServerConfig?")]
        private UserTransportType userServerTransportType;
        
        public static HathoraServerMgrBase Singleton { get; private set; }

        protected override void OnAwake()
        {
            Debug.Log("[HathoraFishnetServerMgr] OnAwake");
            base.OnAwake();
            setSingleton();
        }
    
        private void setSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraServerMgrBase.setSingleton] Error: " +
                    "setSingleton: Destroying dupe");
            
                Destroy(gameObject);
                return;
            }
    
            Singleton = this;
        }

        /// <summary>
        /// Each platform sets their Transport in a different way with different Transport scripts.
        /// In FishNet, we generally use `Tugboat` for UDP (default), and `Bayou` for TCP (WS/WebGL).
        /// </summary>
        /// <param name="_configTransportType"></param>
        protected override void SetServerTransport(TransportType _configTransportType)
        {
            base.SetServerTransport(_configTransportType);
            
            switch (userServerTransportType)
            {
                case UserTransportType.UdpDefault:
                    Debug.Log("[HathoraFishnetServerMgr.SetServerTransport] UserTransportType.UdpDefault");
                    return; // Already set default in most NetCode
                
                case UserTransportType.TcpWslWebgl:
                    Debug.Log("[HathoraFishnetServerMgr.SetServerTransport] UserTransportType.TcpWslWebgl");
                    setBayouTransport();
                    break;
                
                case UserTransportType.HathoraServerConfigSource:
                    Debug.Log("[HathoraFishnetServerMgr.SetServerTransport] UserTransportType.HathoraServerConfigSource");
                    setServerTransportViaHathoraConfig(_configTransportType);
                    break;
                    
                default:
                    Debug.LogError("[HathoraServerMgrBase.setServerTransport] (!) " +
                        $"Unsupported transport type: `{userServerTransportType}`");
                    return;
            }
        }

        /// <summary>Transport set from HathoraServerConfig (default UDP)</summary>
        private static void setServerTransportViaHathoraConfig(TransportType _configTransport)
        {
            switch (_configTransport)
            {
                case TransportType.Udp:
                    break; // Already set as default
                    
                case TransportType.Tcp:
                    setBayouTransport();
                    break;
            }
        }

        /// <summary>
        /// Validate + set Bayou Transport as default, then delete the other default.
        /// Bayou == TCP/WS/WebGL.
        /// TODO: Use Multipass and we can support both at same time. Req's 2nd open port (eg: 7778).
        /// TODO: Consider other protocols.
        /// </summary>
        private static void setBayouTransport()
        {
            Tugboat tugboatUdpTransport = InstanceFinder.NetworkManager.GetComponent<Tugboat>();
            Bayou bayouWebglTcpWsTransport = InstanceFinder.NetworkManager.GetComponent<Bayou>();
            
            Assert.IsNotNull(bayouWebglTcpWsTransport, "Expected `Bayou` webgl component in NetworkManager");
            Assert.IsTrue(bayouWebglTcpWsTransport.enabled, "Expected `Bayou` webgl component to be enabled in NetworkManager");

            transport = bayouWebglTcpWsTransport;
            if (tugboatUdpTransport != null)
                Destroy(tugboatUdpTransport); // Prevent conflicts, just in case - possibly set to same port
        }
    }
}
// Created by dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Scripts.Net.Common.Models
{
    /// <summary>
    /// Set transpport configurations for where the server will listen.
    /// </summary>
    [Serializable]
    public class TransportInfo
    {
        [SerializeField, Tooltip("Default: UDP. UDP is recommended for realtime games: Faster, but less reliable.")]
        private TransportType _transportType = TransportType.Udp;
        public TransportType TransportType
        {
            get => _transportType;
            set => _transportType = value;
        }    

        [SerializeField, Range(1024, 65535), Tooltip("Default: 7777; or use 1024~65535")]
        private int _portNumber = 7777;
        public int PortNumber
        {
            get => _portNumber;
            set => _portNumber = value;
        }

        // Public utils
        
        /// <summary>
        /// Override this if you want the name to be custom
        /// </summary>
        /// <returns></returns>
        public virtual string GetTransportNickname() => "default";
    }
}

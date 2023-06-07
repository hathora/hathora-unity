// Created by dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Common.Models
{
    /// <summary>
    /// Set transpport configurations for where the server will listen.
    /// --- 
    /// This is a wrapper for Hathora SDK's `ExposedPort` model.
    /// We'll eventually replace this with a [Serializable] revamp of the model.
    /// </summary>
    [Serializable]
    public class ExposedPortWrapper : ContainerPortWrapper
    {
        [SerializeField, JsonProperty("host")]
        private string _host;
        public string Host
        {
            get => _host;
            set => _host = value;
        }
        
        
        public ExposedPortWrapper(ExposedPort _exposedPort)
            : base(_exposedPort)
        {
            if (_exposedPort == null)
                return;
            
            this.Host = _exposedPort.Host;
        }
        
        public virtual ExposedPort ToExposedPortType()
        {
            ExposedPort exposedPort = null;
            string containerName = this.GetTransportNickname(); // Should be "default"
            
            try
            {
                exposedPort = new ExposedPort(
                    this.TransportType,
                    this.PortNumber,
                    this.Host,
                    containerName
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }

            return exposedPort;
        }

    }
}

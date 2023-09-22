// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using HathoraCloud.Models.Shared;
using Newtonsoft.Json;
using UnityEngine;

namespace Hathora.Core.Scripts.Editor.Server.SerializedWrappers
{
    /// <summary>
    /// Hathora SDK model wrapper to allow serializable class/fields.
    /// 
    /// This is a wrapper for Hathora SDK's `ConnectionInfoV2` model.
    /// TODO: Upgrade SDK models to natively support serialization
    /// </summary>
    [Serializable]
    public class ConnectionInfoV2Serializable
    {
        [SerializeField, JsonProperty("status")]
        private ConnectionInfoV2Status _statusEnum;
        public ConnectionInfoV2Status StatusEnum
        {
            get => _statusEnum;
            set => _statusEnum = value;
        }
        
        [SerializeField, JsonProperty("exposedPort")]
        private ExposedPortSerializable _exposedPort;
        public ExposedPort ExposedPort
        {
            get => _exposedPort.ToExposedPortType();
            set => _exposedPort = new ExposedPortSerializable(value);
        }
        
        [SerializeField, JsonProperty("additionalExposedPorts")]
        private List<ExposedPortSerializable> _additionalExposedPorts;
        public List<ExposedPort> AdditionalExposedPorts
        {
            get => _additionalExposedPorts?.ConvertAll(wrapper => wrapper.ToExposedPortType());
            set => _additionalExposedPorts = value?.ConvertAll(val => 
                new ExposedPortSerializable(val));
        }
        
        [SerializeField, JsonProperty("roomId")]
        private string _roomId;
        public string RoomId
        {
            get => _roomId;
            set => _roomId = value;
        }


        public ConnectionInfoV2Serializable(ConnectionInfoV2 _connectionInfoV2)
        {
            if (_connectionInfoV2 == null)
                return;
            
            this.AdditionalExposedPorts = _connectionInfoV2.AdditionalExposedPorts;
            this.ExposedPort = _connectionInfoV2.ExposedPort;
            this.StatusEnum = _connectionInfoV2.Status;
            this.RoomId = _connectionInfoV2.RoomId;
        }

        public ConnectionInfoV2 ToConnectionInfoV2Type()
        {
            // (!) SDK constructor throws on req'd val == null
            
            ConnectionInfoV2 room = null;
            try
            {
                room = new ConnectionInfoV2(
                    this.AdditionalExposedPorts,
                    this.ExposedPort,
                    this.StatusEnum,
                    this.RoomId
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
            
            return room;
        }
    }
}

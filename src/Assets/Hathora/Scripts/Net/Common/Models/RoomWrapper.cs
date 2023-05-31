// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using Hathora.Cloud.Sdk.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Hathora.Scripts.Net.Common.Models
{
    /// <summary>
    /// This is a wrapper for Hathora SDK's `Room` model.
    /// We'll eventually replace this with a [Serializable] revamp of the model.
    /// </summary>
    [Serializable]
    public class RoomWrapper
    {
        [SerializeField, JsonProperty("status")]
        private RoomStatus _status = RoomStatus.Destroyed;
        public RoomStatus Status
        {
            get => _status;
            set => _status = value;
        }
        
        [SerializeField, JsonProperty("currentAllocation")]
        private RoomAllocation _currentAllocation = new();
        public RoomAllocation CurrentAllocation
        {
            get => _currentAllocation;
            set => _currentAllocation = value;
        }

        [SerializeField, JsonProperty("allocations")]
        private List<RoomAllocation> _allocations = new();
        public List<RoomAllocation> Allocations
        {
            get => _allocations;
            set => _allocations = value;
        }

        [SerializeField, JsonProperty("roomId")]
        private string _roomId;
        public string RoomId
        {
            get => _roomId;
            set => _roomId = value;
        }

        [SerializeField, JsonProperty("appId")]
        private string _appId;
        public string AppId
        {
            get => _appId;
            set => _appId = value;
        }
    }
}

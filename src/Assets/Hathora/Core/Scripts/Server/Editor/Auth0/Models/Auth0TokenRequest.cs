// Created by dylan@hathora.dev

using System;
using Newtonsoft.Json;

namespace Hathora.Core.Scripts.Server.Editor.Auth0.Models
{
    [Serializable]
    public class Auth0TokenRequest
    {
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }

        [JsonProperty("device_code")]
        public string DeviceCode { get; set; }
        
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
    }
}
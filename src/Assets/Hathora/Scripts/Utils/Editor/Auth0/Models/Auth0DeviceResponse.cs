// Created by dylan@hathora.dev

using System;
using Newtonsoft.Json;

namespace Hathora.Scripts.Utils.Editor.Auth0.Models
{
    [Serializable]
    public class Auth0DeviceResponse
    {
        [JsonProperty("device_code")]
        public string DeviceCode { get; set; }

        [JsonProperty("user_code")]
        public string UserCode { get; set; }

        [JsonProperty("verification_uri")]
        public string VerificationUri { get; set; }

        [JsonProperty("verification_uri_complete")]
        public string VerificationUriComplete { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("interval")]
        public int Interval { get; set; }
    }
}

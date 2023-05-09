// Created by dylan@hathora.dev

namespace Hathora.Scripts.Utils.Editor.Auth0.Models
{
    public class Auth0DeviceRequest
    {
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public string Audience { get; set; }
    }
}

// Created by dylan@hathora.dev

namespace Hathora.Net.Client.Models
{
    public class AuthResult
    {
        public readonly string PlayerAuthToken;
        public bool IsSuccess => !string.IsNullOrEmpty(PlayerAuthToken);
        
        public AuthResult(string _playerAuthToken)
        {
            this.PlayerAuthToken = _playerAuthToken;
        }
    }
}

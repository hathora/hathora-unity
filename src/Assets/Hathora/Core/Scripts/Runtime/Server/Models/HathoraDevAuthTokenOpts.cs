// Created by dylan@hathora.dev

using System;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Server.Models
{
    [Serializable]
    public class HathoraDevAuthTokenOpts
    {
        /// <summary>TODO: To be renamed to `HathoraDevToken` to parity the new `SDKConfig.HathoraDevToken`</summary>
        [SerializeField]
        private string _devAuthToken;

        /// <summary>TODO: To be renamed to `HathoraDevToken` to parity the new `SDKConfig.HathoraDevToken`</summary>
        public string DevAuthToken
        {
            get => _devAuthToken;
            set => _devAuthToken = value;
        }

        [SerializeField, Tooltip("Deletes an existing refresh_token, if exists from cached file")]
        private bool _forceNewToken = false;
        public bool ForceNewToken
        {
            get => _forceNewToken;
            set => _forceNewToken = value;
        }
        
        /// <summary>Temp var that triggers refreshing apps list after init auth</summary>
        public bool RecentlyAuthed { get; set; }
        
        
        // Public utils
        public bool HasAuthToken => !string.IsNullOrEmpty(_devAuthToken);
    }
}

// Created by dylan@hathora.dev

using System;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraDevAuthTokenOpts
    {
        [SerializeField]
        private string _devAuthToken;
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

        
        // Public utils
        public bool HasAuthToken => !string.IsNullOrEmpty(_devAuthToken);

        /// <summary>
        /// Explicit typings for FindNestedProperty() calls
        /// </summary>
        public struct SerializedFieldNames
        {
            public static string DevAuthToken => nameof(_devAuthToken);
            public static string ForceNewToken => nameof(_forceNewToken);
        }
    }
}

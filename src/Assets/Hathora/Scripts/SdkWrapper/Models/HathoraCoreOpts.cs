// Created by dylan@hathora.dev

using System;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraCoreOpts
    {
        [SerializeField, Tooltip("Get from your Hathora dashboard")]
        private string _appId;
        public string AppId
        {
            get => _appId;
            set => _appId = value;
        }
        
       
#if UNITY_SERVER || DEBUG
        /// <summary>
        /// Doc | https://hathora.dev/docs/guides/generate-admin-token
        /// </summary>
        [SerializeField, Tooltip("Set earlier from log in button")]
        private HathoraDevAuthTokenOpts _devAuthOpts;
        public HathoraDevAuthTokenOpts DevAuthOpts
        {
            get => _devAuthOpts;
            set => _devAuthOpts = value;
        }
#endif // UNITY_SERVER || DEBUG

        
        // public utils

        /// <summary>
        /// Explicit typings for FindNestedProperty() calls
        /// </summary>
        public struct SerializedFieldNames
        {
            public static string AppId => nameof(_appId);
            public static string DevAuthOpts => nameof(_devAuthOpts);
        }
    }
}

// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using Hathora.Cloud.Sdk.Model;
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
        
        /// <summary>Not shown to User</summary>
        private int _existingAppsSelectedIndex;
        public int ExistingAppsSelectedIndex
        {
            get => _existingAppsSelectedIndex;
            set => _existingAppsSelectedIndex = value;
        }

        /// <summary>Not shown to User: pulled from App API.</summary>
        public List<string> GetExistingAppNames() =>
            _existingApps?.Select(app => app.AppName).ToList()
            ?? new List<string>(); // Default to empty list
        
        // Not shown to user
        private List<ApplicationWithDeployment> _existingApps;
        public List<ApplicationWithDeployment> ExistingApps
        {
            get => _existingApps;
            set => _existingApps = value;
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
            // public static string GetExistingAppNames => nameof(_existing);
            // public static string ExistingAppNamesSelectedIndex => nameof(_existingAppsSelectedIndex);
        }
    }
}

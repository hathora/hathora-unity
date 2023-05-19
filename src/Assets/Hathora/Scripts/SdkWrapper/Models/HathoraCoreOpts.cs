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
        /// <summary>Get from your Hathora dashboard</summary>
        private string _appId;

        /// <summary>Get from your Hathora dashboard</summary>
        public string AppId
        {
            get => _appId;
            set => _appId = value;
        }
        
        private int _existingAppsSelectedIndex;
        public int ExistingAppsSelectedIndex
        {
            get => _existingAppsSelectedIndex;
            set => _existingAppsSelectedIndex = value;
        }

        /// <summary>Cached from App API</summary>
        public List<string> GetExistingAppNames() =>
            _existingApps?.Select(app => app.AppName).ToList()
            ?? new List<string>(); // Default to empty list
        
        private List<ApplicationWithDeployment> _existingApps;
        public List<ApplicationWithDeployment> ExistingApps
        {
            get => _existingApps;
            set => _existingApps = value;
        }
        
       
#if UNITY_SERVER || DEBUG
        /// <summary>Doc | https://hathora.dev/docs/guides/generate-admin-token</summary>
        [SerializeField, Tooltip("Set earlier from log in button")]
        private HathoraDevAuthTokenOpts _devAuthOpts;
        
        /// <summary>Doc | https://hathora.dev/docs/guides/generate-admin-token</summary>
        public HathoraDevAuthTokenOpts DevAuthOpts
        {
            get => _devAuthOpts;
            set => _devAuthOpts = value;
        }
#endif // UNITY_SERVER || DEBUG
    }
}

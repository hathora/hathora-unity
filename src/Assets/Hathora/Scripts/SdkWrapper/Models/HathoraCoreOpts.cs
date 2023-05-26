// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common.Models;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraCoreOpts
    {
        /// <summary>Get from your Hathora dashboard</summary>
        [SerializeField]
        private string _appId;

        /// <summary>Get from your Hathora dashboard</summary>
        public string AppId => _existingAppsSelectedIndex < 0
            ? null
            : ExistingAppsWithDeployment?[_existingAppsSelectedIndex]?.AppId;
        
        [SerializeField]
        private int _existingAppsSelectedIndex = -1;
        public int ExistingAppsSelectedIndex
        {
            get => _existingAppsSelectedIndex;
            set => _existingAppsSelectedIndex = value;
        }

        /// <summary>Ported from `ApplicationWithDeployment`</summary>
        [SerializeField]
        private List<ApplicationWithDeploymentWrapper> _existingAppsWithDeploymentWrapper = new();
        
        public List<ApplicationWithDeploymentWrapper> ExistingAppsWithDeploymentWrapper
        {
            get => _existingAppsWithDeploymentWrapper;
            set => _existingAppsWithDeploymentWrapper = value;
        }

        /// <summary>Ported from `ApplicationWithDeployment`</summary>
        public List<ApplicationWithDeployment> ExistingAppsWithDeployment
        {
            get {
                if (this._existingAppsWithDeploymentWrapper == null)
                    return new List<ApplicationWithDeployment>();
                
                List<ApplicationWithDeployment> appsWithDeployment = this._existingAppsWithDeploymentWrapper
                    .Select(app => app.ToApplicationWithDeploymentType())
                    .ToList();

                return appsWithDeployment;
            }
            
            set  
            {
                if (value == null)
                    return;
                
                List<ApplicationWithDeploymentWrapper> parsedList = _existingAppsWithDeploymentWrapper = value
                    .Select(app => new ApplicationWithDeploymentWrapper(app))
                    .ToList();

                this._existingAppsWithDeploymentWrapper = parsedList;
            }
        }

        /// <summary>Cached from App API</summary>
        public List<string> GetExistingAppNames() => _existingAppsWithDeploymentWrapper?
                .Select(app => app.AppName)
                .ToList() 
            ?? new List<string>(); // Default to empty list
        
        
#if UNITY_SERVER || DEBUG
        /// <summary>Doc | https://hathora.dev/docs/guides/generate-admin-token</summary>
        [SerializeField, Tooltip("Set earlier from log in button")]
        private HathoraDevAuthTokenOpts _devAuthOpts = new();
        
        /// <summary>Doc | https://hathora.dev/docs/guides/generate-admin-token</summary>
        public HathoraDevAuthTokenOpts DevAuthOpts
        {
            get => _devAuthOpts;
            set => _devAuthOpts = value;
        }
#endif // UNITY_SERVER || DEBUG
    }
}

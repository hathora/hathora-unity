// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hathora.Scripts.Common.Models
{
    [Serializable]
    public class HathoraCoreOpts
    {
        /// <summary>Get from your Hathora dashboard.</summary>
        [SerializeField]
        private string _appId;

        /// <summary>Get from your Hathora dashboard</summary>
        public string AppId => _existingAppsSelectedIndex < 0
            ? null
            : ExistingAppsWithDeployment?[_existingAppsSelectedIndex]?.AppId;
        
        public bool HasAppId => !string.IsNullOrEmpty(AppId);
        
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

        /// <summary>Cached from App API.</summary>
        /// <param name="_prependDummyIndex0Str">
        /// (!) Hathora SDK Enums starts at index 1; not 0: Care of indexes.
        /// </param>
        public List<string> GetExistingAppNames(string _prependDummyIndex0Str)
        {
            if (_existingAppsWithDeploymentWrapper == null)
                return new List<string>();

            IEnumerable<string> enumerable = _existingAppsWithDeploymentWrapper?
                .Select(app => app.AppName);

            if (!string.IsNullOrEmpty(_prependDummyIndex0Str))
            {
                if (HathoraUtils.SDK_ENUM_STARTING_INDEX == 0)
                {
                    Debug.LogWarning("HathoraUtils.SDK_ENUM_STARTING_INDEX == 0, " +
                        "but you are using a _prependDummyIndex0Str: Intentional?");
                }    
                
                enumerable = enumerable.Prepend(_prependDummyIndex0Str);
            }      

            return enumerable.ToList();
        }
        
        
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

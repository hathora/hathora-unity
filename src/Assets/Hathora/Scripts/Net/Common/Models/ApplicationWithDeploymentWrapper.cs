// Created by dylan@hathora.dev

using System;
using System.Globalization;
using Hathora.Cloud.Sdk.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Scripts.Net.Common.Models
{
    /// <summary>
    /// This is a wrapper for Hathora SDK's `ApplicationWithDeployment` model.
    /// We'll eventually replace this with a [Serializable] revamp of the model.
    /// </summary>
    [Serializable]
    public class ApplicationWithDeploymentWrapper
    {
        [SerializeField]
        private string _appId;
        public string AppId 
        { 
            get => _appId;
            set => _appId = value;
        }
        
        /// <summary>Serialized from `DateTime`.</summary>
        [SerializeField]
        private string _createdAtWrapper;
        
        /// <summary>Serialized from `DateTime`.</summary>
        public DateTime CreatedAt
        {
            get => DateTime.TryParse(_createdAtWrapper, out DateTime parsedDateTime) 
                ? parsedDateTime 
                : DateTime.MinValue;

            set => _createdAtWrapper = value.ToString(CultureInfo.InvariantCulture);
        }
        
        [SerializeField]
        private string _createdBy;
        public string CreatedBy 
        { 
            get => _createdBy;
            set => _createdBy = value;
        }
        
        [SerializeField]
        private string _deletedBy;
        public string DeletedBy 
        { 
            get => _deletedBy;
            set => _deletedBy = value;
        }


        /// <summary>Serialized from `DateTime`.</summary>
        [SerializeField]
        private string _deletedAtWrapper;
        
        /// <summary>Serialized from `DateTime`.</summary>
        public DateTime? DeletedAt
        {
            get => DateTime.TryParse(_deletedAtWrapper, out DateTime parsedDateTime)
                ? parsedDateTime
                : null;
            
            set => _deletedAtWrapper = value.ToString();
        }
 
        [SerializeField]
        private string _orgId;
        public string OrgId 
        { 
            get => _orgId;
            set => _orgId = value;
        }
        
        [SerializeField]
        private ApplicationAuthConfigurationWrapper _authConfigurationWrapper;
        public ApplicationAuthConfiguration AuthConfiguration 
        { 
            get => _authConfigurationWrapper.ToApplicationAuthConfigurationType();
            set => _authConfigurationWrapper = new ApplicationAuthConfigurationWrapper(value);
        }
        
        [SerializeField]
        private string _appSecret;
        public string AppSecret 
        { 
            get => _appSecret;
            set => _appSecret = value;
        }
        
        [SerializeField]
        private string _appName;
        public string AppName 
        { 
            get => _appName;
            set => _appName = value;
        }
        
        
        [SerializeField]
        private DeploymentWrapper _deploymentWrapper;
        public Deployment Deployment 
        { 
            get => _deploymentWrapper?.ToDeploymentType();
            set => _deploymentWrapper = value == null 
                ? null 
                : new DeploymentWrapper(value);
        }
        
        
        // [SerializeField] // TODO
        // private IDictionary<string, object> _additionalProperties;
        // public IDictionary<string, object> AdditionalProperties 
        // { 
        //     get => _additionalProperties;
        //     set => _additionalProperties = value;
        // }

        
        public ApplicationWithDeploymentWrapper(ApplicationWithDeployment _appWithDeployment)
        {
            if (_appWithDeployment == null)
                return;

            this.AppId = _appWithDeployment.AppId;
            this.AppName = _appWithDeployment.AppName;
            this.CreatedAt = _appWithDeployment.CreatedAt;
            this.DeletedBy = _appWithDeployment.DeletedBy; 
            this.DeletedAt = _appWithDeployment.DeletedAt;
            this.AppSecret = _appWithDeployment.AppSecret;
            this.OrgId = _appWithDeployment.OrgId;
            this.Deployment = _appWithDeployment.Deployment;
            this.AuthConfiguration = _appWithDeployment.AuthConfiguration;
            // this.AdditionalProperties = _appWithDeployment.AdditionalProperties; // TODO
        }

        private void setMissingDefaults()
        {
            this.CreatedBy ??= "";
            this.DeletedBy ??= "";
            this.AppSecret ??= "";
            this.OrgId ??= "";
            this.AuthConfiguration ??= new ApplicationAuthConfiguration();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public ApplicationWithDeployment ToApplicationWithDeploymentType()
        {
            // (!) Throws on req'd val == null
            setMissingDefaults();

            ApplicationWithDeployment appWithDeploy = null;
            try
            {
                appWithDeploy = new ApplicationWithDeployment(
                    DeletedBy,
                    DeletedAt,
                    CreatedAt,
                    CreatedBy,
                    OrgId,
                    AuthConfiguration,
                    AppSecret,
                    AppId,
                    AppName, 
                    Deployment
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
            
            // appWithDeploy.AdditionalProperties = this.AdditionalProperties; // TODO

            return appWithDeploy;
        }
    }
}

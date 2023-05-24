// Created by dylan@hathora.dev

using System;
using System.Globalization;
using Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Scripts.Net.Common.Models
{
    /// <summary>
    /// This is a wrapper for Hathora SDK's `ApplicationWithDeployment` model.
    /// We'll eventually replace this with a [Serializable] revamp of the model.
    /// </summary>
    [Serializable]
    public class ApplicationWithDeploymentWrapper
    {
        /// <summary>Serialized from `DateTime`.</summary>
        [SerializeField]
        private string _createdAtWrapper;
        
        /// <summary>Serialized from `DateTime`.</summary>
        public DateTime CreatedAt
        {
            get => DateTime.Parse(_createdAtWrapper);
            set => _createdAtWrapper = value.ToString(CultureInfo.InvariantCulture);
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
            get => DateTime.Parse(_deletedAtWrapper);
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
            get => _deploymentWrapper.ToDeploymentType();
            set => _deploymentWrapper = new DeploymentWrapper(value);
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
            this.CreatedAt = _appWithDeployment.CreatedAt;
            this.DeletedBy = _appWithDeployment.DeletedBy;
            this.DeletedAt = _appWithDeployment.DeletedAt;
            this.AppSecret = _appWithDeployment.AppSecret;
            this.AppName = _appWithDeployment.AppName;
            this.OrgId = _appWithDeployment.OrgId;
            this.Deployment = _appWithDeployment.Deployment;
            this.AuthConfiguration = _appWithDeployment.AuthConfiguration; // TODO
            // this.AdditionalProperties = _appWithDeployment.AdditionalProperties; // TODO
        }

        public ApplicationWithDeployment ToApplicationWithDeploymentType() => new()
        {
            DeletedBy = this.DeletedBy,
            DeletedAt = this.DeletedAt,
            CreatedAt = this.CreatedAt,
            AppSecret = this.AppSecret,
            AppName = this.AppName,
            OrgId = this.OrgId,
            Deployment = this.Deployment,
            AuthConfiguration = this.AuthConfiguration, // TODO
            // AdditionalProperties = this.AdditionalProperties, // TODO
        };
    }
}

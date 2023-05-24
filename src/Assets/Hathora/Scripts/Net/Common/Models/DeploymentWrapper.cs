// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hathora.Cloud.Sdk.Model;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements.Experimental;

namespace Hathora.Scripts.Net.Common.Models
{
    /// <summary>
    /// This is a wrapper for Hathora SDK's `Deployment` model.
    /// We'll eventually replace this with a [Serializable] revamp of the model.
    /// </summary>
    [Serializable]
    public class DeploymentWrapper
    {
        [SerializeField]
        private PlanName _planName;
        public PlanName PlanName 
        { 
            get => _planName;
            set => _planName = value;
        }
        
        [SerializeField]
        private TransportType _transportType;
        public TransportType TransportType 
        { 
            get => _transportType;
            set => _transportType = value;
        }
        
        // [SerializeField] // TODO
        // private List<DeploymentEnvInner> _env;
        // public List<DeploymentEnvInner> Env 
        // { 
        //     get => _env;
        //     set => _env = value;
        // }
        
        [SerializeField]
        private double _roomsPerProcess;
        public double RoomsPerProcess 
        { 
            get => _roomsPerProcess;
            set => _roomsPerProcess = value;
        }
        
        
        /// <summary>Parses from List of ContainerPort</summary>
        [SerializeField]
        private List<ExtraContainerPortWrapper> _additionalContainerPorts;

        /// <summary>Parses from List of ContainerPort</summary>
        public List<ContainerPort> AdditionalContainerPorts
        {
            get => _additionalContainerPorts
                .Select(port => port.ToContainerPortType())
                .ToList();

            set => _additionalContainerPorts = value
                .Select(port => new ExtraContainerPortWrapper(port))
                .ToList();
        }

        /// <summary>Parses from ContainerPort</summary>
        [SerializeField]
        private ContainerPortWrapper defaultContainerPortWrapperWrapper;
        
        /// <summary>Parses from ContainerPort</summary>
        public ContainerPortWrapper DefaultContainerPortWrapper 
        { 
            get => defaultContainerPortWrapperWrapper;
            set => defaultContainerPortWrapperWrapper = value;
        }
        
        
        [SerializeField]
        private ContainerPortWrapper _containerPortWrapper;
        public ContainerPort ContainerPort 
        { 
            get => _containerPortWrapper.ToContainerPortType();
            set => _containerPortWrapper = new ContainerPortWrapper(value);
        }
        
        
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
        private string _createdBy;
        public string CreatedBy
        {
            get => _createdBy;
            set => _createdBy = value;
        }
        
        [SerializeField]
        private double _requestedMemoryMB;
        public double RequestedMemoryMB 
        { 
            get => _requestedMemoryMB;
            set => _requestedMemoryMB = value;
        }
        
        [SerializeField]
        private double _requestedCPU;
        public double RequestedCPU 
        { 
            get => _requestedCPU;
            set => _requestedCPU = value;
        }
        
        [SerializeField]
        private double _deploymentId;
        public double DeploymentId 
        { 
            get => _deploymentId;
            set => _deploymentId = value;
        }
        
        [SerializeField]
        private double _buildId;
        public double BuildId 
        { 
            get => _buildId;
            set => _buildId = value;
        }
        
        [SerializeField]
        private string _appId;

        public string AppId 
        { 
            get => _appId;
            set => _appId = value;
        }
        
        // // [SerializeField] // TODO
        // private IDictionary<string, object> _additionalProperties;
        // public IDictionary<string, object> AdditionalProperties 
        // { 
        //     get => _additionalProperties;
        //     set => _additionalProperties = value;
        // }
        
        
        public DeploymentWrapper(Deployment _deployment)
        {
            if (_deployment == null)
                return;
            
            this.PlanName = _deployment.PlanName;
            this.TransportType = _deployment.TransportType;
            this.RoomsPerProcess = _deployment.RoomsPerProcess;
            this.DefaultContainerPortWrapper = new ContainerPortWrapper(_deployment.DefaultContainerPort);
            this.AdditionalContainerPorts = _deployment.AdditionalContainerPorts;
            this.CreatedAt = _deployment.CreatedAt;
            this.CreatedBy = _deployment.CreatedBy;
            this.RequestedMemoryMB = _deployment.RequestedMemoryMB;
            this.RequestedCPU = _deployment.RequestedCPU;
            this.DeploymentId = _deployment.DeploymentId;
            this.BuildId = _deployment.BuildId;
            this.AppId = _deployment.AppId;
            // Env = _deployment.Env;
            // AdditionalProperties = _deployment.AdditionalProperties;
        }

        public Deployment ToDeploymentType()
        {
            // (!) Throws on missing req'd arg
            List<DeploymentEnvInner> emptyEnv = new();
            
            return new(
                env: emptyEnv,
                createdAt: this.CreatedAt,
                createdBy: this.CreatedBy,
                planName: this.PlanName,
                transportType: this.TransportType,
                roomsPerProcess: this.RoomsPerProcess,
                additionalContainerPorts: this.AdditionalContainerPorts,
                defaultContainerPort: this.ContainerPort,
                requestedMemoryMB: this.RequestedMemoryMB,
                requestedCPU: this.RequestedCPU,
                deploymentId: this.DeploymentId,
                buildId: this.BuildId,
                appId: this.AppId // Env = this.Env,
                // AdditionalProperties = this.AdditionalProperties
            );   
        }
    }
}

// dylan@hathora.dev

using System;
using System.Collections.Generic;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common.Models;
using Hathora.Scripts.Utils;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraDeployOpts
    {
        /// <summary>Default: 1. How many rooms do you want to support per server?</summary>
        [SerializeField]
        private int _roomsPerProcess = 1;

        /// <summary>Default: 1. How many rooms do you want to support per server?</summary>
        public int RoomsPerProcess
        {
            get => _roomsPerProcess;
            set => _roomsPerProcess = value;
        }

        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        [SerializeField]
        private PlanName _planName = PlanName.Tiny;

        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        public PlanName PlanName
        {
            get => _planName;
            set => _planName = value;
        }

        [SerializeField]
        private int _planSizeSelectedIndex = (int)PlanName.Tiny;
        public int PlanSizeSelectedIndex
        {
            get => _planSizeSelectedIndex;
            set => _planSizeSelectedIndex = value;
        }

        
        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        [SerializeField]
        private ContainerPortWrapper _containerPortWrapper = new();

        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        public ContainerPortWrapper ContainerPortWrapper
        {
            get => _containerPortWrapper;
            set => _containerPortWrapper = value;
        }
        
        [SerializeField]
        private int _transportTypeSelectedIndex = (int)TransportType.Udp;
        public int TransportTypeSelectedIndex
        {
            get => _transportTypeSelectedIndex;
            set => _transportTypeSelectedIndex = value;
        }
        

        /// <summary>(!) Like an `.env` file, these are all strings.</summary>
        [SerializeField]
        private List<HathoraEnvVars> _envVars = new();

        /// <summary>(!) Like an `.env` file, these are all strings.</summary>
        public List<HathoraEnvVars> EnvVars
        {
            get => _envVars;
            set => _envVars = value;
        }

        /// <summary>You probably don't need to touch these, unless debugging</summary>
        [SerializeField]
        private HathoraDeployAdvancedOpts _advancedDeployOpts = new();

        /// <summary>You probably don't need to touch these, unless debugging</summary>
        public HathoraDeployAdvancedOpts AdvancedDeployOpts
        {
            get => _advancedDeployOpts;
            set => _advancedDeployOpts = value;
        }

        /// <summary>If you deployed something, we set the cached result</summary>
        // [SerializeField] // TODO: Make serializable. For now, this won't persist between Unity sessions.
        private DeploymentWrapper _lastDeployment;
        
        /// <summary>If you deployed something, we set the cached result</summary>
        public DeploymentWrapper LastDeployment
        {
            get => _lastDeployment;
            set => _lastDeployment = value;
        }
    }
}

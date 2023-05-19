// dylan@hathora.dev

using System;
using System.Collections.Generic;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common.Models;
using Hathora.Scripts.Utils;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraDeployOpts
    {
        /// <summary>Default: 1. How many rooms do you want to support per server?</summary>
        private int _roomsPerProcess = 1;

        /// <summary>Default: 1. How many rooms do you want to support per server?</summary>
        public int RoomsPerProcess
        {
            get => _roomsPerProcess;
            set => _roomsPerProcess = value;
        }

        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        private PlanName _planName = PlanName.Tiny;

        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        public PlanName PlanName
        {
            get => _planName;
            set => _planName = value;
        }

        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        private ContainerPortWrapper _containerPortWrapper;

        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        public ContainerPortWrapper ContainerPortWrapper
        {
            get => _containerPortWrapper;
            set => _containerPortWrapper = value;
        }

        /// <summary>(!) Like an `.env` file, these are all strings.</summary>
        private List<HathoraEnvVars> _envVars;

        /// <summary>(!) Like an `.env` file, these are all strings.</summary>
        public List<HathoraEnvVars> EnvVars
        {
            get => _envVars;
            set => _envVars = value;
        }

        /// <summary>You probably don't need to touch these, unless debugging</summary>
        private HathoraDeployAdvancedOpts _advancedDeployOpts;

        /// <summary>You probably don't need to touch these, unless debugging</summary>
        public HathoraDeployAdvancedOpts AdvancedDeployOpts
        {
            get => _advancedDeployOpts;
            set => _advancedDeployOpts = value;
        }
    }
}

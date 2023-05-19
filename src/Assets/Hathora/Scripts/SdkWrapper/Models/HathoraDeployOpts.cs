// dylan@hathora.dev

using System;
using System.Collections.Generic;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common.Models;
using Hathora.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraDeployOpts
        {
            [SerializeField, Tooltip("Default: 1. How many rooms do you want to support per server?")]
            private int _roomsPerProcess = 1;
            public int RoomsPerProcess
            {
                get => _roomsPerProcess;
                set => _roomsPerProcess = value;
            }

            [SerializeField, Tooltip("Default: Tiny. Billing Option: You only get charged for active rooms.")]
            private PlanName _planName = PlanName.Tiny;
            public PlanName PlanName
            {
                get => _planName;
                set => _planName = value;
            }

            [FormerlySerializedAs("containerPortWrapper")]
            [FormerlySerializedAs("_transportInfo")]
            [SerializeField, Tooltip("Default: Tiny. Billing Option: You only get charged for active rooms.")]
            private ContainerPortWrapper _containerPortWrapper;
            public ContainerPortWrapper ContainerPortWrapper
            {
                get => _containerPortWrapper;
                set => _containerPortWrapper = value;
            }

            [SerializeField, Tooltip("(!) Like an `.env` file, these are all strings. ")]
            private List<HathoraEnvVars> _envVars;
            public List<HathoraEnvVars> EnvVars
            {
                get => _envVars;
                set => _envVars = value;
            }

            [FormerlySerializedAs("advancedDeployOpts")]
            [SerializeField, Tooltip("You probably don't need to touch these, unless debugging")]
            private HathoraDeployAdvancedOpts _advancedDeployOpts;
            public HathoraDeployAdvancedOpts AdvancedDeployOpts
            {
                get => _advancedDeployOpts;
                set => _advancedDeployOpts = value;
            }

            // Public utils
            
            /// <summary>
            /// Explicit typings for FindNestedProperty() calls
            /// </summary>
            public struct SerializedFieldNames
            {
                public static string RoomsPerProcess => nameof(_roomsPerProcess);
                public static string PlanName => nameof(_planName);
                public static string TransportInfo => nameof(_containerPortWrapper);
                public static string EnvVars => nameof(_envVars);
                public static string AdvancedDeployOpts => nameof(_advancedDeployOpts);
            }
        }
}

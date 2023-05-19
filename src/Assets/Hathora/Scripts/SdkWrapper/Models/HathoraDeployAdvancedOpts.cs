// dylan@hathora.dev

using System;
using System.Collections.Generic;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common.Models;
using Hathora.Scripts.Utils.Extensions;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraDeployAdvancedOpts
    {
        // Private Serialized
        [SerializeField, Tooltip("Perhaps you are curious *what* we're uploading " +
             "via the `.hathora` temp dir; or want to edit the Dockerfile?")]
        private bool _keepTempDir;
        public bool KeepTempDir
        {
            get => _keepTempDir;
            set => _keepTempDir = value;
        }
            
        [SerializeField, Tooltip("In rare cases, you may want to provide multiple (up to 2 more) transports. " +
             "Leave the nickname empty and we'll ignore this. Ensure the port differs from the others.")]
        public ExtraContainerPortWrapper _extraTransportInfo1;
        public ExtraContainerPortWrapper ExtraTransportInfo1
        {
            get => _extraTransportInfo1;
            set => _extraTransportInfo1 = value;
        }
        
        [SerializeField, Tooltip("In rare cases, you may want to provide multiple (up to 2 more) transports. " +
             "Leave the nickname empty and we'll ignore this. Ensure the port differs from the others.")]
        public ExtraContainerPortWrapper _extraTransportInfo2;
        public ExtraContainerPortWrapper ExtraTransportInfo2
        {
            get => _extraTransportInfo2;
            set => _extraTransportInfo2 = value;
        }
            
            
        // Public utils

        /// <summary>Useful for CreateDeploymentAsync() via HathoraDeployOpts.</summary>
        public bool CheckHasAdditionalContainerPorts() =>
            !string.IsNullOrEmpty(_extraTransportInfo1?.TransportNickname) ||
            !string.IsNullOrEmpty(_extraTransportInfo2?.TransportNickname); 

        /// <summary>
        /// Returns a List of Hathora SDK ContainerPort.
        /// </summary>
        /// <returns>Returns an init'd but empty list, if nothing or invalid.</returns>
        public List<ContainerPort> GetExtraContainerPorts()
        {
            if (!CheckHasAdditionalContainerPorts())
                return null;
            
            List<ContainerPort> extraContainerPorts = new();
            
            if (checkIsValidExtraTransportInfo(_extraTransportInfo1))
                extraContainerPorts.AddIfNotNull(ParseToSdkContainerToPort(_extraTransportInfo1));
            
            if (checkIsValidExtraTransportInfo(_extraTransportInfo2))
                extraContainerPorts.AddIfNotNull(ParseToSdkContainerToPort(_extraTransportInfo2));

            return extraContainerPorts;
        }

        private bool checkIsValidExtraTransportInfo(ExtraContainerPortWrapper containerPortWrapper) =>
            !string.IsNullOrEmpty(containerPortWrapper?.TransportNickname) &&
            containerPortWrapper.PortNumber != 7777;

        public ContainerPort ParseToSdkContainerToPort(ExtraContainerPortWrapper containerPortWrapper)
        {
            if (containerPortWrapper == null)
                return null;
            
            return new ContainerPort(
                containerPortWrapper.TransportType,
                containerPortWrapper.PortNumber,
                containerPortWrapper.GetTransportNickname());
        }
        
        /// <summary>
        /// Explicit typings for FindNestedProperty() calls
        /// </summary>
        public struct SerializedFieldNames
        {
            public static string KeepTempDir => nameof(_keepTempDir);
            public static string ExtraTransportInfo1 => nameof(_extraTransportInfo1);
            public static string ExtraTransportInfo2 => nameof(_extraTransportInfo2);
        }
    }
}

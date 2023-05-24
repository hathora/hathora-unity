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
        /// <summary>
        /// Perhaps you are curious *what* we're uploading via the
        /// `.hathora` temp dir; or want to edit the Dockerfile?
        /// </summary>
        [SerializeField] 
        private bool _keepTempDir;
        
        /// <summary>
        /// Perhaps you are curious *what* we're uploading via the
        /// `.hathora` temp dir; or want to edit the Dockerfile?
        /// </summary>
        public bool KeepTempDir
        {
            get => _keepTempDir;
            set => _keepTempDir = value;
        }
            
        /// <summary>
        /// In rare cases, you may want to provide multiple (up to 2 more) transports.
        /// Leave the nickname empty and we'll ignore this.
        /// Ensure the port differs from the others.
        /// </summary>
        [SerializeField] 
        private ExtraContainerPortWrapper _extraTransportInfo1 = new();
        
        /// <summary>
        /// In rare cases, you may want to provide multiple (up to 2 more) transports.
        /// Leave the nickname empty and we'll ignore this.
        /// Ensure the port differs from the others.
        /// </summary>
        public ExtraContainerPortWrapper ExtraTransportInfo1
        {
            get => _extraTransportInfo1;
            set => _extraTransportInfo1 = value;
        }
        
        /// <summary>
        /// In rare cases, you may want to provide multiple (up to 2 more) transports.
        /// Leave the nickname empty and we'll ignore this.
        /// Ensure the port differs from the others.
        /// </summary>
        [SerializeField] 
        private ExtraContainerPortWrapper _extraTransportInfo2 = new();
        
        /// <summary>
        /// In rare cases, you may want to provide multiple (up to 2 more) transports.
        /// Leave the nickname empty and we'll ignore this.
        /// Ensure the port differs from the others.
        /// </summary>
        public ExtraContainerPortWrapper ExtraTransportInfo2
        {
            get => _extraTransportInfo2;
            set => _extraTransportInfo2 = value;
        }
        

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
    }
}

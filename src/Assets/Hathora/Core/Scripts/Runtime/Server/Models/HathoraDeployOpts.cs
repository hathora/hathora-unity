// dylan@hathora.dev

using System;
using System.Text;
using Hathora.Core.Scripts.Runtime.Server.Models.SerializedWrappers;
using HathoraCloud.Models.Shared;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Core.Scripts.Runtime.Server.Models
{
    [Serializable]
    public class HathoraDeployOpts
    {
        #region Rooms Per Process
        /// <summary>Default: 1. How many rooms do you want to support per server?</summary>
        [SerializeField]
        private int _roomsPerProcess = 1;

        /// <summary>Default: 1. How many rooms do you want to support per server?</summary>
        public int RoomsPerProcess
        {
            get => _roomsPerProcess;
            set => _roomsPerProcess = value;
        }
        #endregion // Rooms Per Process
        
        
        #region Plan Name
        /// <summary>(!) Hathora SDK Enums starts at index 1; not 0: Care of indexes</summary>
        [SerializeField]
        private int _planNameSelectedIndex = (int)PlanName.Tiny;

        /// <summary>(!) Hathora SDK Enums starts at index 1; not 0: Care of indexes</summary>
        public int PlanNameSelectedIndex
        {
            get => _planNameSelectedIndex;
            set => _planNameSelectedIndex = value;
        }

        /// <summary>(!) Hathora SDK Enums starts at index 1; not 0: Care of indexes</summary>
        public PlanName SelectedPlanName => 
            (PlanName)_planNameSelectedIndex;
        #endregion // Plan Name
        
        
        #region Container Port
        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        [FormerlySerializedAs("containerPortSerializableSerializable")]
        [FormerlySerializedAs("_containerPortSerializable")]
        [SerializeField]
        private ContainerPortSerializable _containerPortSerializableSerializable = new();

        /// <summary>Default: Tiny. Billing Option: You only get charged for active rooms.</summary>
        public ContainerPortSerializable ContainerPortSerializable
        {
            get => _containerPortSerializableSerializable;
            set => _containerPortSerializableSerializable = value;
        }
        #endregion // Container Port


        #region Transport Type 
        /// <summary>(!) Hathora SDK Enums starts at index 1; not 0: Care of indexes</summary>
        [SerializeField]
        private int _transportTypeSelectedIndex = (int)TransportType.Udp;
        
        /// <summary>(!) Hathora SDK Enums starts at index 1; not 0: Care of indexes</summary>
        public TransportType SelectedTransportType => 
            (TransportType)_transportTypeSelectedIndex;
       
        /// <summary>(!) Hathora SDK Enums starts at index 1; not 0: Care of indexes</summary>
        public int TransportTypeSelectedIndex
        {
            get => _transportTypeSelectedIndex;
            set => _transportTypeSelectedIndex = value;
        }
        #endregion // Transport Type 

        
        #region Last Deployment
        /// <summary>If you deployed something, we set the cached result</summary>
        // [SerializeField] // TODO: Make serializable. For now, this won't persist between Unity sessions.
        private Deployment _lastDeployment;
        
        /// <summary>If you deployed something, we set the cached result</summary>
        public Deployment LastDeployment
        {
            get => _lastDeployment;
            set => _lastDeployment = value;
        }
        
        private StringBuilder _lastDeployLogsStrb = new();
        public StringBuilder LastDeployLogsStrb
        {
            get => _lastDeployLogsStrb; 
            set => _lastDeployLogsStrb = value;
        }
        public bool HasLastDeployLogsStrb => 
            LastDeployLogsStrb?.Length > 0;
        #endregion // Last Deployment
    }
}

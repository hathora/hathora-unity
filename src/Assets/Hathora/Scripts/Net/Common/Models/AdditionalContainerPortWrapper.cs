// Created by dylan@hathora.dev

using System;
using Hathora.Cloud.Sdk.Model;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hathora.Scripts.Net.Common.Models
{
    /// <summary>
    /// Set transpport configurations for where the server will listen.
    /// Unlike ContainerPortWrapper, here, you can customize the nickname (instead of "default").
    /// Leave the nickname null and we'll ignore this class.
    /// ---
    /// This is a wrapper for Hathora SDK's `ApplicationWithDeployment` model.
    /// We'll eventually replace this with a [Serializable] revamp of the model.
    /// </summary>
    [Serializable]
    public class AdditionalContainerPortWrapper : ContainerPortWrapper
    {
        [SerializeField, Tooltip("Choose an arbitrary name to identify this transpport easier. " +
             "`Default` is reserved. Leave this empty and we'll ignore this class.")]
        private string _transportNickname;
        public string TransportNickname
        {
            get => GetTransportNickname();
            set => _transportNickname = value;
        }
        
        public AdditionalContainerPortWrapper()
        {
        }

        public AdditionalContainerPortWrapper(ContainerPort _containerPort)
            : base(_containerPort)
        {
            this._transportNickname = _containerPort.Name;
        }

        /// <summary>
        /// Override this if you want the name to be custom
        /// </summary>
        /// <returns></returns>
        public override string GetTransportNickname()
        {
            Assert.IsFalse(_transportNickname == "default", 
                "Extra Transport nickname cannot be 'default' (reserved)");
            
            return _transportNickname;   
        }

        public override ContainerPort ToContainerPortType()
        {
            ContainerPort containerPort = base.ToContainerPortType();
            containerPort.Name = GetTransportNickname();

            return containerPort;
        }
    }
}

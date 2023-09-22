// Created by dylan@hathora.dev

using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Server.Models.SerializableWrappers
{
    /// <summary>
    /// Serializable wrapper for Hathora SDK's `ApplicationWithDeployment`.
    /// This allows for persistence in HathoraServerConfig.
    /// </summary>
    public class ApplicationWithDeploymentSerializable
    {
        [SerializeField]
        public string AppId;
        
        [SerializeField]
        public string AppName;
        
        [SerializeField]
        public string AppDescription;
    }
}

// Created by dylan@hathora.dev

using HathoraSdk;

namespace Hathora.Core.Scripts.Runtime.Common.Models
{
    /// <summary>
    /// Common reqs for HathoraServerApiWrapperBase || HathoraClientApiWrapperBase.
    /// </summary>
    public interface IHathoraApiBase
    {
        SDKConfig HathoraSdkConfig { get; set; }
        string AppId { get; }
        
        /// <summary>Creates a default SDK Config</summary>
        SDKConfig GenerateSdkConfig();
    }
}

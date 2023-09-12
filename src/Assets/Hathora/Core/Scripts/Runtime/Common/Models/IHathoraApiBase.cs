// Created by dylan@hathora.dev

namespace Hathora.Core.Scripts.Runtime.Common.Models
{
    /// <summary>
    /// Common reqs for HathoraServerApiWrapperBase || HathoraClientApiWrapperBase.
    /// </summary>
    public interface IHathoraApiBase
    {
        //// TODO: `Configuration` is missing in the new SDK - cleanup, if permanently gone.
        // Configuration HathoraSdkConfig { get; set; }
        string AppId { get; }
        
        //// TODO: `Configuration` is missing in the new SDK - cleanup, if permanently gone.
        // Configuration GenerateSdkConfig();
    }
}

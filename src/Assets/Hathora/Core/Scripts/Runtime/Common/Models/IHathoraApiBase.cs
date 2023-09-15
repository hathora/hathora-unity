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

        /// <summary>
        /// Serialize the object to a readable [optionally prettified/indented] format.
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="prettify">Add indention formatting for clarity?</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string ToJson<T>(T Obj, bool prettify = true);
    }
}

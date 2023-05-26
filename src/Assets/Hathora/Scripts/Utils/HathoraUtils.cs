// Created by dylan@hathora.dev

using System.IO;
using System.Runtime.InteropServices;
using Hathora.Cloud.Sdk.Client;
using Hathora.Scripts.Net.Common;
using Application = UnityEngine.Application;

namespace Hathora.Scripts.Utils
{
    public static class HathoraUtils
    {
        /// <summary>
        /// (!) Hathora SDK Enums starts at index 1; not 0: Care of indexes
        /// TODO: If this ever becomes 0, delete this const and update all refs.
        /// </summary>
        public const int SDK_ENUM_STARTING_INDEX = 1;

        public static string NormalizePath(string _path) =>
            Path.GetFullPath(_path);

        /// <summary>
        /// Gets path to Unity proj root, then normalizes the/path/slashes.
        /// </summary>
        /// <returns></returns>
        public static string GetNormalizedPathToProjRoot()
        {
            string dirtyPathToUnityProjRoot = Directory.GetParent(Application.dataPath)?.ToString();
            return dirtyPathToUnityProjRoot == null 
                ? null 
                : NormalizePath(dirtyPathToUnityProjRoot);
        }

        public static Configuration GenerateSdkConfig(NetHathoraConfig _netHathoraConfig) => new()
        {
            AccessToken = _netHathoraConfig.HathoraCoreOpts.DevAuthOpts.DevAuthToken,
        };
    }
}
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
// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.IO;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Server;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using Application = UnityEngine.Application;

namespace Hathora.Scripts.Utils
{
    public static class HathoraUtils
    {
        #region UserConfig Serializable Containers
        [Serializable]
        public class ConfigCoreOpts
        {
            [SerializeField, Tooltip("Get from your Hathora dashboard")]
            public string AppId;

            [SerializeField, Tooltip("Required (for Rooms/Lobbies)")]
            public Region region = Region.Seattle;


#if UNITY_SERVER || DEBUG
            /// <summary>
            /// Doc | https://hathora.dev/docs/guides/generate-admin-token 
            /// </summary>
            [SerializeField, Tooltip("Get from npm pkg `@hathora/cli` via `hathora-cloud login` cmd. " +
                 "Required for server calls, such as auto-deploying to Hathora from the UserConfig file.")]
            public string DevAuthToken;
#endif
        }

        [Serializable]
        public class AutoBuildOpts
        {
            [SerializeField, Tooltip("This will auto-fill with the active scene on !focus, if you leave blank.")]
            public string BuildSceneName;

            [SerializeField]
            public string ServerBuildDirName = "Build-Server";

            [SerializeField]
            public string ServerBuildExeName = "Hathora-Unity-LinuxServer.x86_64";
        }

        [Serializable]
        public struct ConfigAdvancedBuildOpts
        {
            [SerializeField, Tooltip("Perhaps you are curious *what* we're uploading, " +
                 "kept at Unity project root .uploadToHathora")]
            public bool KeepTempDir;
        }

        [Serializable]
        public class HathoraDeployOpts
        {
            [SerializeField, Tooltip("Default: 1. How many rooms do you want to support per server?")]
            public int RoomsPerProcess = 1;

            [SerializeField, Tooltip("Default: Tiny. Billing Option: You only get charged for active rooms.")]
            public HathoraServerConfig.HathoraPlanSize PlanSizeSize =
                HathoraServerConfig.HathoraPlanSize.Tiny;

            [SerializeField, Tooltip("Default: UDP. UDP is recommended for realtime games: Faster, but less reliable.")]
            public TransportType TransportType = TransportType.Udp;

            [SerializeField, Tooltip("Default: 7777; minimum 1024")]
            public int PortNumber = 7777;

            [SerializeField, Tooltip("(!) Like an `.env` file, these are all strings. ")]
            public List<HathoraEnvVars> EnvVars;

            [SerializeField, Tooltip("You probably don't need to touch these, unless debugging")]
            public HathoraUtils.ConfigAdvancedBuildOpts AdvancedBuildOpts;
        }
        #endregion // UserConfig Serializable Containers


        /// <summary>
        /// Container for all the many paths to prep/upload/deploy a Hathora server build.
        /// </summary>
        public class HathoraDeployPaths
        {
            public const string hathoraConsoleAppBaseUrl = "https://console.hathora.dev/application/";

            public HathoraServerConfig UserConfig;
            public string UnityProjRootPath;
            public string TempDirPath;
            public string ArchiveName;
            public string CompressedArchiveName;
            public string PathToCompressedArchive;
            public string PathToBuildExe;
            public string StringifiedEnvKeyValues;

            private HathoraDeployPaths() { }

            public HathoraDeployPaths(HathoraServerConfig userConfig)
            {
                this.UserConfig = userConfig;
                this.UnityProjRootPath = GetNormalizedPathToProjRoot(); // Path slashes normalized
                this.TempDirPath = Path.Combine(UnityProjRootPath, ".uploadToHathora");
                this.ArchiveName = $"{UserConfig.LinuxAutoBuildOpts.ServerBuildExeName}.tar";
                this.CompressedArchiveName = $"{ArchiveName}.gz";
                this.PathToCompressedArchive = Path.Combine(TempDirPath, CompressedArchiveName);
                this.PathToBuildExe = UserConfig.GetPathToBuildExe();
                this.StringifiedEnvKeyValues = JsonConvert.SerializeObject(UserConfig.EnvVars);
            }
        }

        /// <summary>
        /// Gets path to Unity proj root, then normalizes the/path/slashes.
        /// </summary>
        /// <returns></returns>
        public static string GetNormalizedPathToProjRoot()
        {
            var dirtyPathToUnityProjRoot = Directory.GetParent(Application.dataPath)?.ToString();
            return dirtyPathToUnityProjRoot == null 
                ? null 
                : Path.GetFullPath(dirtyPathToUnityProjRoot);
        }

    }
}
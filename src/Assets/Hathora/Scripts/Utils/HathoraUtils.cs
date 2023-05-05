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
            // Serialized
            [SerializeField, Tooltip("Get from your Hathora dashboard")]
            private string _appId;

            [SerializeField, Tooltip("Required (for Rooms/Lobbies)")]
            private Region _region = Region.Seattle;
            public Region Region => _region;
            
            /// <summary>
            /// Doc | https://hathora.dev/docs/guides/generate-admin-token 
            /// </summary>
            [SerializeField, Tooltip("Get from npm pkg `@hathora/cli` via `hathora-cloud login` cmd. " +
                 "Required for server calls, such as auto-deploying to Hathora from the UserConfig file.")]
            private string _devAuthToken;

            // Public Getters
            public string AppId => _appId;

#if UNITY_SERVER || DEBUG
            public string DevAuthToken => _devAuthToken;
#endif
        }

        [Serializable]
        public class AutoBuildOpts
        {
            // Private Serialized
            [SerializeField, Tooltip("Default: HathoraDemoScene. This will auto-fill " +
                 "with the active scene on !focus, if you leave blank.")]
            private string _buildSceneName;

            [SerializeField, Tooltip("Default: Build-Server")]
            private string _serverBuildDirName = "Build-Server";

            [SerializeField, Tooltip("Default: Hathora-Unity-LinuxServer.x86_64")]
            private string _serverBuildExeName = "Hathora-Unity-LinuxServer.x86_64";

            [SerializeField, Tooltip("The same as checking 'Developer Build' in build opts")]
            private bool _isDevBuild = true;
            
            // Public Getters
            public string BuildSceneName => _buildSceneName;
            public string ServerBuildDirName => _serverBuildDirName; 
            public string ServerBuildExeName => _serverBuildExeName;
            public bool IsDevBuild => _isDevBuild;

            // Public Setters
            public string SetBuildSceneName(string _val) =>
                this._buildSceneName = _val;
        }

        [Serializable]
        public struct ConfigAdvancedBuildOpts
        {
            // Private Serialized
            [SerializeField, Tooltip("Perhaps you are curious *what* we're uploading, " +
                 "kept at Unity project root .uploadToHathora")]
            private bool _keepTempDir;
            
            [SerializeField, Tooltip("Include CLI logs, which may get bloaty")]
            private bool _verboseLogs;
            
            // Public Getters
            public bool KeepTempDir => _keepTempDir;
            public bool VerboseLogs => _verboseLogs;
        }

        [Serializable]
        public class HathoraDeployOpts
        {
            // Priovate Serialized
            [SerializeField, Tooltip("Default: 1. How many rooms do you want to support per server?")]
            private int _roomsPerProcess = 1;

            [SerializeField, Tooltip("Default: Tiny. Billing Option: You only get charged for active rooms.")]
            private HathoraServerConfig.HathoraPlanSize _planSizeSize =
                HathoraServerConfig.HathoraPlanSize.Tiny;

            [SerializeField, Tooltip("Default: UDP. UDP is recommended for realtime games: Faster, but less reliable.")]
            private TransportType _transportType = TransportType.Udp;

            [SerializeField, Tooltip("Default: 7777; minimum 1024")]
            private int _portNumber = 7777;

            [SerializeField, Tooltip("(!) Like an `.env` file, these are all strings. ")]
            private List<HathoraEnvVars> _envVars;

            [SerializeField, Tooltip("You probably don't need to touch these, unless debugging")]
            private ConfigAdvancedBuildOpts _advancedBuildOpts;

            // Public Getters
            public int RoomsPerProcess => _roomsPerProcess;
            public HathoraServerConfig.HathoraPlanSize PlanSizeSize => _planSizeSize;
            public TransportType TransportType => _transportType;
            public int PortNumber => _portNumber;
            public List<HathoraEnvVars> EnvVars => _envVars;
            public ConfigAdvancedBuildOpts AdvancedBuildOpts => _advancedBuildOpts;
        }
        #endregion // UserConfig Serializable Containers


        /// <summary>
        /// Container for all the many paths to prep/upload/deploy a Hathora server build.
        /// </summary>
        public class HathoraDeployPaths
        {
            public const string hathoraConsoleAppBaseUrl = "https://console.hathora.dev/application/";

            public readonly HathoraServerConfig UserConfig;
            public readonly string UnityProjRootPath;
            public readonly string TempDirPath;
            public readonly string ArchiveName;
            public readonly string CompressedArchiveName;
            public readonly string PathToCompressedArchive;
            public readonly string PathToBuildExe;
            public readonly string StringifiedEnvKeyValues;

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
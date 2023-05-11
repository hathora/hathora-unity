// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FishNet.Transporting;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common.Models;
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
        public class DevAuthTokenOpts
        {
            // Private serialized
            [SerializeField]
            private string _devAuthToken;
            
            [SerializeField, Tooltip("Deletes an existing refresh_token, if exists from cached file")]
            private bool _forceNewToken = false;

            // Public getters
            public string DevAuthToken => _devAuthToken;
            public bool ForceNewToken => _forceNewToken;
            public bool HasAuthToken => !string.IsNullOrEmpty(_devAuthToken);
            
            // Public setters
            public void SetDevAuthToken(string newToken) => 
                _devAuthToken = newToken;
        }
        
        [Serializable]
        public class ConfigCoreOpts
        {
            // Serialized
            [SerializeField, Tooltip("Get from your Hathora dashboard")]
            private string _appId;

            [SerializeField, Tooltip("Required (for Rooms/Lobbies)")]
            private Region _region = Region.Seattle;
            
            #if UNITY_SERVER || DEBUG
            /// <summary>
            /// Doc | https://hathora.dev/docs/guides/generate-admin-token
            /// </summary>
            [SerializeField, Tooltip("Get from npm pkg `@hathora/cli` via `hathora-cloud login` cmd. " +
                 "Required for server calls, such as auto-deploying to Hathora from the UserConfig file.")]
            private DevAuthTokenOpts _devAuthOpts;
            #endif // UNITY_SERVER || DEBUG

            // Public Getters
            public string AppId => _appId;
            public Region Region => _region;

#if UNITY_SERVER || DEBUG
            public DevAuthTokenOpts DevAuthOpts => _devAuthOpts;
#endif
        }

        [Serializable]
        public class AutoBuildOpts
        {
            // Private Serialized
            [SerializeField, Tooltip("Default: Build-Server")]
            private string _serverBuildDirName = "Build-Server";

            [SerializeField, Tooltip("Default: Hathora-Unity-LinuxServer.x86_64")]
            private string _serverBuildExeName = "Hathora-Unity-LinuxServer.x86_64";

            [SerializeField, Tooltip("The same as checking 'Developer Build' in build opts")]
            private bool _isDevBuild = true;
            
            [SerializeField, Tooltip("If an old build exists, first delete this dir?")]
            private bool _cleanBuildDir = true;
            
            // Public Getters
            public string ServerBuildDirName => _serverBuildDirName; 
            public string ServerBuildExeName => _serverBuildExeName;
            public bool IsDevBuild => _isDevBuild;
            public bool CleanBuildDir => _cleanBuildDir;
        }

        [Serializable]
        public struct ConfigAdvancedDeployOpts
        {
            // Private Serialized
            [SerializeField, Tooltip("Perhaps you are curious *what* we're uploading " +
                 "via the `.hathora` temp dir; or want to edit the Dockerfile?")]
            private bool _keepTempDir;
            
            [SerializeField, Tooltip("In rare cases, you may want to provide multiple (up to 2 more) transports. " +
                 "Leave the nickname empty and we'll ignore this. Ensure the port differs from the others.")]
            public ExtraTransportInfo _extraTransportInfo1;
        
            [SerializeField, Tooltip("In rare cases, you may want to provide multiple (up to 2 more) transports. " +
                 "Leave the nickname empty and we'll ignore this. Ensure the port differs from the others.")]
            public ExtraTransportInfo _extraTransportInfo2;
            
            // Public Getters
            public bool KeepTempDir => _keepTempDir;

            public ExtraTransportInfo ExtraTransportInfo1 => _extraTransportInfo1;
            public ExtraTransportInfo ExtraTransportInfo2 => _extraTransportInfo2;
        }

        [Serializable]
        public class HathoraDeployOpts
        {
            // Private Serialized
            [SerializeField, Tooltip("Default: 1. How many rooms do you want to support per server?")]
            private int _roomsPerProcess = 1;

            [SerializeField, Tooltip("Default: Tiny. Billing Option: You only get charged for active rooms.")]
            private PlanName _planName = PlanName.Tiny;

            [SerializeField, Tooltip("Default: Tiny. Billing Option: You only get charged for active rooms.")]
            private TransportInfo _transportInfo;

            [SerializeField, Tooltip("(!) Like an `.env` file, these are all strings. ")]
            private List<HathoraEnvVars> _envVars;

            [FormerlySerializedAs("_advancedBuildOpts")]
            [SerializeField, Tooltip("You probably don't need to touch these, unless debugging")]
            private ConfigAdvancedDeployOpts advancedDeployOpts;

            // Public Getters
            public int RoomsPerProcess => _roomsPerProcess;
            public PlanName PlanName => _planName;
            public TransportInfo TransportInfo => _transportInfo;
            public List<HathoraEnvVars> EnvVars => _envVars;
            public ConfigAdvancedDeployOpts AdvancedDeployOpts => advancedDeployOpts;
        }
        #endregion // UserConfig Serializable Containers


        /// <summary>
        /// Container for all the many paths to prep/upload/deploy a Hathora server build.
        /// </summary>
        public class HathoraDeployPaths
        {
            public const string hathoraConsoleAppBaseUrl = "https://console.hathora.dev/application/";

            public readonly NetHathoraConfig UserConfig;
            public readonly string UnityProjRootPath;
            public readonly string TempDirPath;
            public readonly string PathToBuildExe;
            public readonly string PathTo7zCliExe;

            public string PathToDockerfile => NormalizePath($"{TempDirPath}/Dockerfile");
            public string ExeBuildName => UserConfig.LinuxAutoBuildOpts.ServerBuildExeName;
            
            private string pathTo7z64bitDir => NormalizePath($"{TempDirPath}/7zip/x64");
            private string pathTo7zForWindows => NormalizePath($"{pathTo7z64bitDir}/7za.exe");
            private string pathTo7zForMac => NormalizePath($"{pathTo7z64bitDir}/7zz-mac");
            private string pathTo7zForLinux => NormalizePath($"{pathTo7z64bitDir}/7zz-linux");

            private HathoraDeployPaths() { }

            public HathoraDeployPaths(NetHathoraConfig userConfig)
            {
                this.UserConfig = userConfig;
                this.UnityProjRootPath = GetNormalizedPathToProjRoot(); // Path slashes normalized
                this.TempDirPath = NormalizePath(Path.Combine(UnityProjRootPath, ".hathora"));
                this.PathToBuildExe = UserConfig.GetNormalizedPathToBuildExe();
                
                // Determine the correct 7z executable to use based on the platform
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    this.PathTo7zCliExe = pathTo7zForWindows;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    this.PathTo7zCliExe = pathTo7zForMac;
                else
                    this.PathTo7zCliExe = pathTo7zForLinux;
            }
        }
        
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

    }
}
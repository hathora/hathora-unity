// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.Net.Common.Models;
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
            [SerializeField]
            private string _devAuthToken;
            public string DevAuthToken
            {
                get => _devAuthToken;
                set => _devAuthToken = value;
            }
            
            [SerializeField, Tooltip("Deletes an existing refresh_token, if exists from cached file")]
            private bool _forceNewToken = false;
            public bool ForceNewToken
            {
                get => _forceNewToken;
                set => _forceNewToken = value;
            }
            
            // Public utils
            public bool HasAuthToken => !string.IsNullOrEmpty(_devAuthToken);
            
            /// <summary>
            /// Explicit typings for FindNestedProperty() calls
            /// </summary>
            public struct SerializedFieldNames
            {
                public static string DevAuthToken => nameof(_devAuthToken);
                public static string ForceNewToken => nameof(_forceNewToken);
            }
        }
        
        
        [Serializable]
        public class ConfigCoreOpts
        {
            [SerializeField, Tooltip("Get from your Hathora dashboard")]
            private string _appId;
            public string AppId
            {
                get => _appId;
                set => _appId = value;
            }
       
            #if UNITY_SERVER || DEBUG
            /// <summary>
            /// Doc | https://hathora.dev/docs/guides/generate-admin-token
            /// </summary>
            [SerializeField, Tooltip("Set earlier from log in button")]
            private DevAuthTokenOpts _devAuthOpts;
            #endif // UNITY_SERVER || DEBUG
            

#if UNITY_SERVER || DEBUG
            public DevAuthTokenOpts DevAuthOpts => _devAuthOpts;
#endif
            
            /// <summary>
            /// Explicit typings for FindNestedProperty() calls
            /// </summary>
            public struct SerializedFieldNames
            {
                public static string AppId => nameof(_appId);
                public static string DevAuthOpts => nameof(_devAuthOpts);
            }
        }

        
        [Serializable]
        public class AutoBuildOpts
        {
            // Private Serialized
            [SerializeField, Tooltip("Default: Build-Server")]
            private string _serverBuildDirName = "Build-Server";
            public string ServerBuildDirName
            {
                get => _serverBuildDirName;
                set => _serverBuildDirName = value;
            }

            [SerializeField, Tooltip("Default: Hathora-Unity-LinuxServer.x86_64")]
            private string _serverBuildExeName = "Hathora-Unity-LinuxServer.x86_64";
            public string ServerBuildExeName
            {
                get => _serverBuildExeName;
                set => _serverBuildExeName = value;
            }

            [SerializeField, Tooltip("The same as checking 'Developer Build' in build opts")]
            private bool _isDevBuild = true;
            public bool IsDevBuild
            {
                get => _isDevBuild;
                set => _isDevBuild = value;
            }
            
            [SerializeField, Tooltip("If an old build exists, first delete this dir?")]
            private bool _cleanBuildDir = true;
            public bool CleanBuildDir
            {
                get => _cleanBuildDir;
                set => _cleanBuildDir = value;
            }


            // Public utils
            /// <summary>
            /// Explicit typings for FindNestedProperty() calls
            /// </summary>
            public struct SerializedFieldNames
            {
                public static string ServerBuildDirName => nameof(_serverBuildDirName);
                public static string ServerBuildExeName => nameof(_serverBuildExeName);
                public static string IsDevBuild => nameof(_isDevBuild);
                public static string CleanBuildDir => nameof(_cleanBuildDir);
            }
        }

        
        [Serializable]
        public struct ConfigAdvancedDeployOpts
        {
            // Private Serialized
            [SerializeField, Tooltip("Perhaps you are curious *what* we're uploading " +
                 "via the `.hathora` temp dir; or want to edit the Dockerfile?")]
            private bool _keepTempDir;
            public bool KeepTempDir
            {
                get => _keepTempDir;
                set => _keepTempDir = value;
            }
            
            [SerializeField, Tooltip("In rare cases, you may want to provide multiple (up to 2 more) transports. " +
                 "Leave the nickname empty and we'll ignore this. Ensure the port differs from the others.")]
            public ExtraTransportInfo _extraTransportInfo1;
            public ExtraTransportInfo ExtraTransportInfo1
            {
                get => _extraTransportInfo1;
                set => _extraTransportInfo1 = value;
            }
        
            [SerializeField, Tooltip("In rare cases, you may want to provide multiple (up to 2 more) transports. " +
                 "Leave the nickname empty and we'll ignore this. Ensure the port differs from the others.")]
            public ExtraTransportInfo _extraTransportInfo2;
            public ExtraTransportInfo ExtraTransportInfo2
            {
                get => _extraTransportInfo2;
                set => _extraTransportInfo2 = value;
            }
            
            
            // Public utils
            
            /// <summary>
            /// Explicit typings for FindNestedProperty() calls
            /// </summary>
            public struct SerializedFieldNames
            {
                public static string KeepTempDir => nameof(_keepTempDir);
                public static string ExtraTransportInfo1 => nameof(_extraTransportInfo1);
                public static string ExtraTransportInfo2 => nameof(_extraTransportInfo2);
            }
        }
        

        [Serializable]
        public class HathoraDeployOpts
        {
            [SerializeField, Tooltip("Default: 1. How many rooms do you want to support per server?")]
            private int _roomsPerProcess = 1;
            public int RoomsPerProcess
            {
                get => _roomsPerProcess;
                set => _roomsPerProcess = value;
            }

            [SerializeField, Tooltip("Default: Tiny. Billing Option: You only get charged for active rooms.")]
            private PlanName _planName = PlanName.Tiny;
            public PlanName PlanName
            {
                get => _planName;
                set => _planName = value;
            }

            [SerializeField, Tooltip("Default: Tiny. Billing Option: You only get charged for active rooms.")]
            private TransportInfo _transportInfo;
            public TransportInfo TransportInfo
            {
                get => _transportInfo;
                set => _transportInfo = value;
            }

            [SerializeField, Tooltip("(!) Like an `.env` file, these are all strings. ")]
            private List<HathoraEnvVars> _envVars;
            public List<HathoraEnvVars> EnvVars
            {
                get => _envVars;
                set => _envVars = value;
            }

            [FormerlySerializedAs("advancedDeployOpts")]
            [SerializeField, Tooltip("You probably don't need to touch these, unless debugging")]
            private ConfigAdvancedDeployOpts _advancedDeployOpts;
            public ConfigAdvancedDeployOpts AdvancedDeployOpts
            {
                get => _advancedDeployOpts;
                set => _advancedDeployOpts = value;
            }

            // Public utils
            
            /// <summary>
            /// Explicit typings for FindNestedProperty() calls
            /// </summary>
            public struct SerializedFieldNames
            {
                public static string RoomsPerProcess => nameof(_roomsPerProcess);
                public static string PlanName => nameof(_planName);
                public static string TransportInfo => nameof(_transportInfo);
                public static string EnvVars => nameof(_envVars);
                public static string AdvancedDeployOpts => nameof(_advancedDeployOpts);
            }
        }

        
        [Serializable]
        public class HathoraLobbyRoomOpts
        {
            private Region _hathoraRegion = Region.Seattle;
            public Region HathoraRegion
            {
                get => _hathoraRegion;
                set => _hathoraRegion = value;
            }
            
            // Public utils
            
            /// <summary>
            /// Explicit typings for FindNestedProperty() calls
            /// </summary>
            public struct SerializedFieldNames
            {
                public static string HathoraRegion => nameof(_hathoraRegion);
            }
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
            public string ExeBuildDir => UserConfig.LinuxAutoBuildOpts.ServerBuildDirName;
            
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
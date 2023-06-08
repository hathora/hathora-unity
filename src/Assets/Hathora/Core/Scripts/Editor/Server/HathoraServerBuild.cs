// Created by dylan@hathora.dev

using System.IO;
using System.Linq;
using Hathora.Core.Scripts.Runtime.Common.Utils;
using Hathora.Core.Scripts.Runtime.Server;
using Hathora.Core.Scripts.Runtime.Server.Models;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Hathora.Core.Scripts.Editor.Server
{
    /// <summary>
    /// Contains build + deploy methods for Hathora Server.
    /// Trigger these from HathoraServerConfig ScriptableObject buttons. 
    /// </summary>
    public static class HathoraServerBuild
    {
        /// <summary>
        /// Builds with HathoraServerConfig opts.
        /// </summary>
        /// <param name="_serverConfig">Find via menu `Hathora/Find UserConfig(s)`</param>
        /// <returns>isSuccess</returns>
        public static BuildReport BuildHathoraLinuxServer(HathoraServerConfig _serverConfig)
        {
            // Set your build options
            string projRoot = HathoraUtils.GetNormalizedPathToProjRoot();
            string serverBuildDirPath = Path.Combine(projRoot, _serverConfig.LinuxHathoraAutoBuildOpts.ServerBuildDirName);
            string serverBuildExeName = _serverConfig.LinuxHathoraAutoBuildOpts.ServerBuildExeName;
            string serverBuildExeFullPath = Path.Combine(serverBuildDirPath, serverBuildExeName);

            // Create the build directory if it does not exist
            cleanCreateBuildDir(_serverConfig, serverBuildDirPath);

            BuildPlayerOptions buildPlayerOptions = generateBuildPlayerOptions(
                _serverConfig,
                serverBuildExeFullPath);

            // Build the server
            BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            
            // Generate the Dockerfile: Paths will be different for each collaborator
            HathoraServerPaths
            string dockerFileContent = HathoraDocker.GenerateDockerFileStr(_serverPaths:);
            await writeDockerFileAsync(
                serverDeployPaths.PathToDockerfile,
                dockerFileContent,
                _cancelToken);
            
            // Open the build directory
            if (buildReport.summary.result == BuildResult.Succeeded)
                EditorUtility.RevealInFinder(serverBuildExeFullPath);

            return buildReport;
        }

        private static BuildPlayerOptions generateBuildPlayerOptions(
            HathoraServerConfig _serverConfig, 
            string _serverBuildExeFullPath)
        {
            EditorBuildSettingsScene[] scenesInBuildSettings = EditorBuildSettings.scenes; // From build settings
            string[] scenePaths = scenesInBuildSettings?.Select(scene => scene.path).ToArray();
            
            return new BuildPlayerOptions
            {
                // For demo purposes, we're only assuming one scene
                scenes = scenePaths,
                locationPathName = _serverBuildExeFullPath,
                target = BuildTarget.StandaloneLinux64,
                options = BuildOptions.EnableHeadlessMode | // Adds `-batchmode -nographics` 
                    (_serverConfig.LinuxHathoraAutoBuildOpts.IsDevBuild ? BuildOptions.Development : 0),
            };
        }

        private static void cleanCreateBuildDir(
            HathoraServerConfig _serverConfig,
            string _serverBuildDirPath)
        {
            bool targetBuildDirExists = Directory.Exists(_serverBuildDirPath);

            if (_serverConfig.LinuxHathoraAutoBuildOpts.CleanBuildDir && targetBuildDirExists)
            {
                Debug.Log("[HathoraServerBuild] Found old build dir && " +
                    "_serverConfig.LinuxHathoraAutoBuildOpts.CleanBuildDir: Deleting...");
                Directory.Delete(_serverBuildDirPath, recursive: true);
            }
                
            if (!targetBuildDirExists)
                Directory.CreateDirectory(_serverBuildDirPath);
        }
    }
}

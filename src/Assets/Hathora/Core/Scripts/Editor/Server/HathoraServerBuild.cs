// Created by dylan@hathora.dev

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="_overwriteExistingDockerfile">
        /// Some devs have a custom Dockerfile and don't want a new autogen
        /// </param>
        /// <param name="_cancelToken">This won't cancel the build itself, but things around it.</param>
        /// <returns>isSuccess</returns>
        public static async Task<BuildReport> BuildHathoraLinuxServer(
            HathoraServerConfig _serverConfig,
            bool _overwriteExistingDockerfile,
            CancellationToken _cancelToken = default)
        {
            // Set your build options
            HathoraServerPaths configPaths = new(_serverConfig);

            // Create the build directory if it does not exist
            cleanCreateBuildDir(_serverConfig, configPaths.PathToBuildDir);
            _cancelToken.ThrowIfCancellationRequested();

            BuildPlayerOptions buildPlayerOptions = generateBuildPlayerOptions(
                _serverConfig,
                configPaths.PathToBuildExe);

            // Build the server
            BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            _cancelToken.ThrowIfCancellationRequested();
            
            // Generate the Dockerfile to `.hathora/`: Paths will be different for each collaborator
            bool genDockerfile = _overwriteExistingDockerfile || !File.Exists(configPaths.PathToDotHathoraDockerfile); 
            if (genDockerfile)
            {
                Debug.Log("[HathoraServerBuild.BuildHathoraLinuxServer] " +
                    "Generating new Dockerfile (if exists: overwriting)...");
                string dockerFileContent = HathoraDocker.GenerateDockerFileStr(configPaths);
                await HathoraDocker.WriteDockerFileAsync(
                    configPaths.PathToDotHathoraDockerfile,
                    dockerFileContent,
                    _cancelToken);    
            }
            
            // Copy the ./hathora/Dockerfile to build dir
            File.Copy(
                configPaths.PathToDotHathoraDockerfile,
                $"{configPaths.PathToBuildDir}/Dockerfile",
                overwrite: true);
            
            // Open the build directory
            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                // TODO: Play a small, subtle chime sfx?
                Debug.Log("[HathoraServerBuild.BuildHathoraLinuxServer] " +
                    $"Build succeeded @ path: `{configPaths.PathToBuildDir}`");
                EditorUtility.RevealInFinder(configPaths.PathToBuildExe);
            }

            return buildReport;
        }

        private static BuildPlayerOptions generateBuildPlayerOptions(
            HathoraServerConfig _serverConfig, 
            string _serverBuildExeFullPath)
        {
            EditorBuildSettingsScene[] scenesInBuildSettings = EditorBuildSettings.scenes; // From build settings
            string[] scenePaths = scenesInBuildSettings?.Select(scene => scene.path).ToArray();
            
            BuildPlayerOptions buildPlayerOpts = new()
            {
                scenes = scenePaths,
                locationPathName = _serverBuildExeFullPath,
                target = BuildTarget.StandaloneLinux64,
                options = _serverConfig.LinuxHathoraAutoBuildOpts.IsDevBuild 
                    ? BuildOptions.Development 
                    : BuildOptions.None,
            };
            
            // Ensure build is a headless Linux server (formerly set via `options = BuildOptions.EnableHeadlessMode`)
            EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget.Server;

            return buildPlayerOpts;
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

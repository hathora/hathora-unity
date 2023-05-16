// Created by dylan@hathora.dev

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hathora.Scripts.Net.Server;
using Hathora.Scripts.Utils;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Editor
{
    /// <summary>
    /// Contains build + deploy methods for Hathora Server.
    /// Trigger these from NetHathoraConfig ScriptableObject buttons. 
    /// </summary>
    public static class HathoraServerBuild
    {
        /// <summary>
        /// Builds with NetHathoraConfig opts.
        /// </summary>
        /// <param name="config">Find via menu `Hathora/Find UserConfig(s)`</param>
        /// <returns>isSuccess</returns>
        public static BuildReport BuildHathoraLinuxServer(NetHathoraConfig config)
        {
            // Set your build options
            string projRoot = HathoraUtils.GetNormalizedPathToProjRoot();
            string serverBuildDirPath = Path.Combine(projRoot, config.LinuxAutoBuildOpts.ServerBuildDirName);
            string serverBuildExeName = config.LinuxAutoBuildOpts.ServerBuildExeName;
            string serverBuildExeFullPath = Path.Combine(serverBuildDirPath, serverBuildExeName);

            // Create the build directory if it does not exist
            cleanCreateBuildDir(config, serverBuildDirPath);

            BuildPlayerOptions buildPlayerOptions = generateBuildPlayerOptions(
                config,
                serverBuildExeFullPath);

            // Build the server
            BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            
            // Open the build directory
            if (buildReport.summary.result == BuildResult.Succeeded)
                EditorUtility.RevealInFinder(serverBuildExeFullPath);

            return buildReport;
        }

        private static BuildPlayerOptions generateBuildPlayerOptions(
            NetHathoraConfig _config, 
            string _serverBuildExeFullPath)
        {
            EditorBuildSettingsScene[] scenesInBuildSettings = EditorBuildSettings.scenes; // From build settings
            string[] scenePaths = scenesInBuildSettings.Select(scene => scene.path).ToArray();
            
            return new BuildPlayerOptions
            {
                // For demo purposes, we're only assuming one scene
                scenes = scenePaths,
                locationPathName = _serverBuildExeFullPath,
                target = BuildTarget.StandaloneLinux64,
                options = BuildOptions.EnableHeadlessMode | // Adds `-batchmode -nographics` 
                    (_config.LinuxAutoBuildOpts.IsDevBuild ? BuildOptions.Development : 0),
            };
        }

        private static void cleanCreateBuildDir(
            NetHathoraConfig _config,
            string _serverBuildDirPath)
        {
            bool targetBuildDirExists = Directory.Exists(_serverBuildDirPath);

            if (_config.LinuxAutoBuildOpts.CleanBuildDir && targetBuildDirExists)
            {
                Debug.Log("[HathoraServerBuild] Found old build dir && " +
                    "config.LinuxAutoBuildOpts.CleanBuildDir: Deleting...");
                Directory.Delete(_serverBuildDirPath, recursive: true);
            }
                
            if (!targetBuildDirExists)
                Directory.CreateDirectory(_serverBuildDirPath);
        }
    }
}

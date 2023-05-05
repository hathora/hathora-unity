// Created by dylan@hathora.dev

using System.IO;
using Hathora.Scripts.Net.Server;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor
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
        /// <param name="config">Find via menu `Hathora/Find UserConfig(s)`</param>
        public static void BuildHathoraLinuxServer(HathoraServerConfig config)
        {
            // Set your build options
            string projRoot = HathoraUtils.GetNormalizedPathToProjRoot();
            string serverBuildPath = Path.Combine(projRoot, config.LinuxAutoBuildOpts.ServerBuildDirName);
            string serverBuildName = config.LinuxAutoBuildOpts.ServerBuildExeName;
            string serverBuildFullPath = Path.Combine(serverBuildPath, serverBuildName);

            // Create the build directory if it does not exist
            if (!Directory.Exists(serverBuildPath))
                Directory.CreateDirectory(serverBuildPath);

            // Set the build options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { $"Assets/Scenes/{config.LinuxAutoBuildOpts.BuildSceneName}.unity" },
                locationPathName = serverBuildFullPath,
                target = BuildTarget.StandaloneLinux64, // Adding `-batchmode -nographics` for headless mode
                options =  BuildOptions.EnableHeadlessMode | BuildOptions.Development,
            };

            // Build the server
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        public static void DevAuthLogin(HathoraServerConfig hathoraServerConfig)
        {
            throw new System.NotImplementedException();
        }
    }
}

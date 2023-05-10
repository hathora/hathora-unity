// Created by dylan@hathora.dev

using System.IO;
using System.Linq;
using Hathora.Scripts.Net.Server;
using UnityEditor;

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
            
            EditorBuildSettingsScene[] scenesInBuildSettings = EditorBuildSettings.scenes; // From build settings
            string[] scenePaths = scenesInBuildSettings.Select(scene => scene.path).ToArray();


            // Create the build directory if it does not exist
            if (!Directory.Exists(serverBuildPath))
                Directory.CreateDirectory(serverBuildPath);

            // Set the build options
            BuildPlayerOptions buildPlayerOptions = new()
            {
                // For demo purposes, we're only assuming one scene
                scenes = scenePaths,
                locationPathName = serverBuildFullPath,
                target = BuildTarget.StandaloneLinux64,
                options = BuildOptions.EnableHeadlessMode | // Adds `-batchmode -nographics` 
                    (config.LinuxAutoBuildOpts.IsDevBuild ? BuildOptions.Development : 0),
            };

            // Build the server
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}

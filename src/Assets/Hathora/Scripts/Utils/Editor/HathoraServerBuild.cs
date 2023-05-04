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
    public class HathoraServerBuild
    {
        /// <summary>
        /// Deploys with HathoraServerConfig opts.
        /// </summary>
        /// <param name="config">Find via menu `Hathora/Find Config(s)`</param>
        public static void DeployToHathora(HathoraServerConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[HathoraServerBuild] Cannot find HathoraServerConfig ScriptableObject");
                return;
            }

            string devToken = config.DevAuthToken;
            // TODO: Deploy to Hathora via hathora-cloud cli
            
        }

        /// <summary>
        /// Builds with HathoraServerConfig opts.
        /// </summary>
        /// <param name="config">Find via menu `Hathora/Find Config(s)`</param>
        public static void BuildHathoraLinuxServer(HathoraServerConfig config)
        {
            // Set your build options
            string serverBuildPath = Path.Combine(Application.dataPath, "../", config.ServerBuildDirName);
            string serverBuildName = config.ServerBuildName;
            string serverBuildFullPath = Path.Combine(serverBuildPath, serverBuildName);

            // Create the build directory if it does not exist
            if (!Directory.Exists(serverBuildPath))
            {
                Directory.CreateDirectory(serverBuildPath);
            }

            // Set the build options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/YourScene.unity" },
                locationPathName = serverBuildFullPath,
                target = BuildTarget.StandaloneLinux64,
                options = BuildOptions.EnableHeadlessMode | BuildOptions.Development
            };

            // Build the server
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}

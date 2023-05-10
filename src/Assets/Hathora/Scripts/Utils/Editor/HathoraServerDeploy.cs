// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

namespace Hathora.Scripts.Utils.Editor
{
    public static class HathoraServerDeploy
    {
        /// <summary>
        /// Deploys with HathoraServerConfig opts.
        /// TODO: Support cancel token.
        /// </summary>
        /// <param name="config">Find via menu `Hathora/Find UserConfig(s)`</param>
        public static async Task DeployToHathoraAsync(HathoraServerConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[HathoraServerBuild] Cannot find " +
                    "HathoraServerConfig ScriptableObject");
                return;
            }

            // ----------------------------------------------
            // Prepare paths and file names that we didn't get from UserConfig
            HathoraUtils.HathoraDeployPaths deployPaths = new(config);
            bool isNewlyCreatedTempDir = await initHathoraDirIfEmpty(deployPaths);

            // Upload via the server SDK.
            Debug.Log("[HathoraServerDeploy] <color=yellow>Preparing to deploy " +
                "to Hathora via Hathora SDK...</color>");
            Hathora.Cloud.Sdk.Client
        }

        private static async Task<string> writeDockerFileAsync(string tempDir, string dockerfileContent)
        {
            string pathToDockerFile = Path.Combine(tempDir, "Dockerfile");
            await File.WriteAllTextAsync(pathToDockerFile, dockerfileContent);

            return pathToDockerFile;
        }

        /// <summary>
        /// If empty (and not forcing a clean temp dir),
        /// init `.hathora` dir >> generate Dockerfile
        /// </summary>
        /// <param name="deployPaths"></param>
        /// <returns>isNewlyCreatedTempDir</returns>
        private static async Task<bool> initHathoraDirIfEmpty(
            HathoraUtils.HathoraDeployPaths deployPaths)
        {
            // Create a .hathora dir if !exists.
            // Don't clean it, if exists: Possible custom Dockerfile.
            if (Directory.Exists(deployPaths.TempDirPath))
            {
                // Keep temp dir? Perhaps they have a custom Dockerfile?
                if (deployPaths.UserConfig.HathoraDeployOpts.AdvancedDeployOpts.KeepTempDir)
                    return false; // !isNewlyCreatedTempDir
                
                // Delete the old temp dir so we may regenerate it cleanly
                Directory.Delete(deployPaths.TempDirPath, recursive: true);
            }
              
            Directory.CreateDirectory(deployPaths.TempDirPath);
            Assert.IsTrue(Directory.Exists(deployPaths.TempDirPath), 
                $"[HathoraServerDeploy] Cannot find TempDirPath: {deployPaths.TempDirPath}");

            // Generate dockerfile
            string dockerFileContent = generateDockerFileStr(
                deployPaths.TempDirPath, 
                deployPaths.UserConfig.LinuxAutoBuildOpts.ServerBuildExeName);
            
            string dockerfilePath = await writeDockerFileAsync(
                deployPaths.TempDirPath, 
                dockerFileContent);
            
            Assert.IsTrue(File.Exists(dockerfilePath), 
                $"[HathoraServerDeploy] Generated Dockerfile not found @ '{dockerfilePath}'");

            return true; // isNewlyCreatedTempDir
        }

        /// <summary>
        /// Temp dir is probably @ project root `/.uploadToHathora`
        /// </summary>
        /// <param name="tempDir"></param>
        /// <param name="exeName"></param>
        /// <returns>"path/to/DockerFile"</returns>
        private static string generateDockerFileStr(
            string tempDir, 
            string exeName)
        {
            return $@"
FROM ubuntu

COPY ./Build-Server .

CMD ./{tempDir}/{exeName} -mode server -batchmode -nographics
";
        }
        
        /// <summary>
        /// Possibly deprecated
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static async Task<string> executeCrossPlatformShellCmdAsync(string cmd, string args)
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string shell = isWindows ? "cmd.exe" : "/bin/bash";
            string escapedArgs = isWindows ? $"/c {args}" : $"-c \"{args}\"";
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = shell,
                    Arguments = escapedArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            process.Start();

            // Read the output and error asynchronously
            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            // Wait for the process to finish and read the output and error
            await process.WaitForExitAsync();

            // Combine output and error
            var output = await outputTask;
            var error = await errorTask;
            string result = output + error;

            return result;
        }
    }
}

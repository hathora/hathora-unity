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
        /// </summary>
        /// <param name="config">Find via menu `Hathora/Find UserConfig(s)`</param>
        public static async void InitDeployToHathora(HathoraServerConfig config)
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

            // Create a NEW temp dir; just to start clean. If exists, delete it and remake
            if (Directory.Exists(deployPaths.TempDirPath))
                Directory.Delete(deployPaths.TempDirPath, recursive:true);
            
            Directory.CreateDirectory(deployPaths.TempDirPath);
            
            // ----------------------------------------------
            // Sanity checks
            Assert.IsTrue(File.Exists(deployPaths.PathToBuildExe), 
                $"[HathoraServerDeploy] Cannot find PathToBuildExe: {deployPaths.PathToCompressedArchive}");
            
            Assert.IsTrue(Directory.Exists(deployPaths.TempDirPath), 
                $"[HathoraServerDeploy] Cannot find TempDirPath: {deployPaths.TempDirPath}");
            
            // ----------------------------------------------
            // Generate Dockerfile within the temp dir
            string dockerfilePath = generateDockerFileForLinuxBuild(
                deployPaths.TempDirPath, 
                config.LinuxAutoBuildOpts.ServerBuildExeName);

            Assert.IsTrue(File.Exists(dockerfilePath), 
                $"[HathoraServerDeploy] Generated Dockerfile not found @ '{dockerfilePath}'");

            // Implement the upload process
            Debug.Log("[HathoraServerDeploy] Preparing to deploy to Hathora...");
            Debug.Log("[HathoraServerDeploy] <color=yellow>(!) TODO:</color> " +
                "Implement the actual upload process");
        }

        /// <summary>
        /// Temp dir is probably @ project root `/.uploadToHathora`
        /// </summary>
        /// <param name="tempDir"></param>
        /// <param name="exeName"></param>
        /// <returns>"path/to/DockerFile"</returns>
        private static string generateDockerFileForLinuxBuild(
            string tempDir, 
            string exeName)
        {
            string dockerfileContent = $@"
FROM ubuntu

COPY ./Build-Server .

CMD ./{tempDir}/{exeName} -mode server -batchmode -nographics
";

            string pathToDockerFile = Path.Combine(tempDir, "Dockerfile");
            File.WriteAllText(pathToDockerFile, dockerfileContent);

            return pathToDockerFile;
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

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
            
            // ----------------------------------------------
            // TODO: Compress {deployPaths.PathToBuildExe} and {deployPaths.PathToDockerfile}
            // into {config.LinuxAutoBuildOpts.ServerBuildExeName}.tar.gz (gzipped tarball).
            // using PathTo7z* deployPaths
            

            // ----------------------------------------------
            // // Upload via the server SDK.
            // Debug.Log("[HathoraServerDeploy] <color=yellow>Preparing to deploy " +
            //     "to Hathora via Hathora SDK...</color>");
        }

        /// <summary>
        /// We currently premake the Dockerfile, so we don't use this yet.
        /// TODO: Use this to customize the Dockerfile without editing directly.
        /// </summary>
        /// <param name="tempDir"></param>
        /// <param name="dockerfileContent"></param>
        /// <returns></returns>
        private static async Task<string> writeDockerFileAsync(
            string tempDir, 
            string dockerfileContent)
        {
            string pathToDockerFile = Path.Combine(tempDir, "Dockerfile");
            await File.WriteAllTextAsync(pathToDockerFile, dockerfileContent);

            return pathToDockerFile;
        }

        /// <summary>
        /// /// We currently premake the Dockerfile, so we don't use this yet.
        /// TODO: Use this to customize the Dockerfile without editing directly.
        /// </summary>
        /// <param name="tempDir">Project root /.hathora</param>
        /// <param name="exeName">Default: "Hathora-Unity-LinuxServer.x86_64"</param>
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
        /// Useful for 7z compression handling.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static async Task<string> executeCrossPlatformShellCmdAsync(string cmd, string args)
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string shell = isWindows ? "cmd.exe" : "/bin/bash";
            string escapedArgs = isWindows ? $"/c {args}" : $"-c \"{args}\"";
            Process process = new()
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

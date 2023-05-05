// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Server;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Hathora.Scripts.Utils.Editor
{
    public static class HathoraServerDeploy
    {
        /// <summary>
        /// Deploys with HathoraServerConfig opts.
        /// </summary>
        /// <param name="config">Find via menu `Hathora/Find UserConfig(s)`</param>
        public static void InitDeployToHathora(HathoraServerConfig config)
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

            // Create a NEW temp dir; just to start clean. If exists, delete it and remake.
            if (Directory.Exists(deployPaths.TempDirPath))
                Directory.Delete(deployPaths.TempDirPath);
            
            Directory.CreateDirectory(deployPaths.TempDirPath);
            
            // ----------------------------------------------
            // Sanity checks
            Assert.IsTrue(File.Exists(deployPaths.PathToBuildExe), 
                $"[HathoraServerDeploy] Cannot find PathToBuildExe: {deployPaths.PathToCompressedArchive}");
            
            Assert.IsTrue(Directory.Exists(deployPaths.TempDirPath), 
                $"[HathoraServerDeploy] Cannot find TempDirPath: {deployPaths.TempDirPath}");
            
            // ----------------------------------------------
            // Create temp dir and generate Dockerfile within
            string dockerfilePath = generateDockerFileForLinuxBuild(
                deployPaths.TempDirPath, 
                config.LinuxAutoBuildOpts.ServerBuildExeName);

            Assert.IsTrue(File.Exists(dockerfilePath), 
                $"[HathoraServerDeploy] Generated Dockerfile not found @ '{dockerfilePath}'");

            // Compress the build using tar and gzip
            gzipBuild(deployPaths);

            // Implement the upload process
            Debug.Log("[HathoraServerDeploy] Preparing to deploy to Hathora...");
            Debug.Log("[HathoraServerDeploy] <color=yellow>(!) TODO:</color> " +
                "Implement the actual upload process");
        }

        private static void gzipBuild(HathoraUtils.HathoraDeployPaths deployPaths)
        {
            string tarFilePath = Path.Combine(deployPaths.TempDirPath, deployPaths.ArchiveName);
            string gzipFilePath = Path.Combine(deployPaths.TempDirPath, deployPaths.CompressedArchiveName);

            // TODO: Create verbose logs options in UserConfig
            string createDockerFileOutputLogs = ExecuteCrossPlatformShellCmd("tar", $"-cvf {tarFilePath} -C " +
                $"{deployPaths.UnityProjRootPath} {deployPaths.UserConfig.LinuxAutoBuildOpts.ServerBuildDirName} Dockerfile");
            
            string gzippedBuildOutputLogs = ExecuteCrossPlatformShellCmd("gzip", $"-f {tarFilePath}");

            //// TODO: Make this async. The Assert below wil fail.
            // Assert.IsTrue(Directory.Exists(gzipFilePath),
            //     $"[HathoraServerDeploy] !found gzipFilePath (perhaps failed to archive?): {gzipFilePath}");
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
        /// Executes a shell command and returns its output.
        /// This func is cross-platform and works on Windows, macOS, and Linux.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="args">The arguments to pass to the command.</param>
        /// <returns>The output produced by the command.</returns>
        private static string ExecuteCrossPlatformShellCmd(string command, string args)
        {
            string escapedArgs = string.Concat(args.Select(c => 
                c == '"' ? "\\\"" : c.ToString()));

            ProcessStartInfo startInfo = new ProcessStartInfo();

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isMacLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            
            if (isWindows)
            {
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = $"/C {command} {escapedArgs}";
            }
            else if (isMacLinux)
            {
                startInfo.FileName = Path.Combine("/bin", "/bash");
                startInfo.Arguments = $"-c \"{command} {escapedArgs}\"";
            }
            else
            {
                throw new NotSupportedException("[HathoraServerDeploy] Unsupported OS");
            }

            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
    }
}

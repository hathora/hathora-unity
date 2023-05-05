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

            // Create a NEW temp dir; just to start clean. If exists, delete it and remake.
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
            // Create temp dir and generate Dockerfile within
            string dockerfilePath = generateDockerFileForLinuxBuild(
                deployPaths.TempDirPath, 
                config.LinuxAutoBuildOpts.ServerBuildExeName);

            Assert.IsTrue(File.Exists(dockerfilePath), 
                $"[HathoraServerDeploy] Generated Dockerfile not found @ '{dockerfilePath}'");

            // Compress the build using tar and gzip
            await gzipBuild(deployPaths);

            // Implement the upload process
            Debug.Log("[HathoraServerDeploy] Preparing to deploy to Hathora...");
            Debug.Log("[HathoraServerDeploy] <color=yellow>(!) TODO:</color> " +
                "Implement the actual upload process");
        }

        private static async Task gzipBuild(HathoraUtils.HathoraDeployPaths deployPaths)
        {
            bool verboseLogs = deployPaths.UserConfig.HathoraDeployOpts.AdvancedBuildOpts.VerboseLogs;

            // Determine the appropriate 7z command based on the current platform
            string sevenZipCmd;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                sevenZipCmd = Path.Combine(deployPaths.UnityProjRootPath, "7zip", "x64", "7za.exe");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                sevenZipCmd = Path.Combine(deployPaths.UnityProjRootPath, "7zip", "7zz-linux");
            }
            else // Assume macOS
            {
                sevenZipCmd = Path.Combine(deployPaths.UnityProjRootPath, "7zip", "7zz-mac");
            }

            if (verboseLogs)
                Debug.Log($"[HathoraServerDeploy] <color=yellow>(VERBOSE_LOGS) sevenZipCmd</color>: {sevenZipCmd}");
            
            // Create the tar archive with the Dockerfile
            string tarFilePath = Path.Combine(deployPaths.TempDirPath, deployPaths.ArchiveName);
            string gzipFilePath = Path.Combine(deployPaths.TempDirPath, deployPaths.CompressedArchiveName);
            
            string createDockerFileOutputLogs = await ExecuteCrossPlatformShellCmdAsync(sevenZipCmd, $"a \"{tarFilePath}\" " +
                $"-w\"{deployPaths.UnityProjRootPath}\" \"{deployPaths.UserConfig.LinuxAutoBuildOpts.ServerBuildDirName}\" " +
                $"\"{Path.Combine(deployPaths.UnityProjRootPath, "Dockerfile")}\"");

            if (verboseLogs)
            {
                Debug.Log($"[HathoraServerDeploy] (VERBOSE_LOGS) " +
                    $"gzipBuild.ExecuteCrossPlatformShellCmdAsync.createDockerFileOutputLogs: {createDockerFileOutputLogs}");
            }

            // Compress the tar archive with gzip
            string gzippedBuildOutputLogs = await ExecuteCrossPlatformShellCmdAsync(sevenZipCmd, 
                $"a -tgzip \"{gzipFilePath}\" \"{tarFilePath}\"");

            if (verboseLogs)
            {
                Debug.Log($"[HathoraServerDeploy] (VERBOSE_LOGS) " +
                    $"gzipBuild.ExecuteCrossPlatformShellCmdAsync.gzippedBuildOutputLogs: {gzippedBuildOutputLogs}");
            }

            Assert.IsTrue(File.Exists(gzipFilePath),
                $"[HathoraServerDeploy] !found gzipFilePath (perhaps failed to archive?): {gzipFilePath}");
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
        
        public static async Task<string> ExecuteCrossPlatformShellCmdAsync(string cmd, string args)
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

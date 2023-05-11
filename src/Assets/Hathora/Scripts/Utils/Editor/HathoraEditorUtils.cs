// Created by dylan@hathora.dev

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Hathora.Scripts.Utils.Editor
{
    /// <summary>
    /// Editor script to add the Hathora banner to an editor window
    /// </summary>
    public static class HathoraEditorUtils
    {
        /// <summary>
        /// Aligns right. Shrinks to fit.
        /// </summary>
        public static void InsertBanner()
        {
            Texture2D bannerTexture = Resources.Load<Texture2D>("HathoraConfigBanner");
            if (bannerTexture == null)
                return;
        
            float windowWidth = EditorGUIUtility.currentViewWidth;
            float bannerWidth = bannerTexture.width;
            float bannerHeight = bannerTexture.height;

            float maxBannerWidth = windowWidth * 0.9f;
            if (bannerWidth > maxBannerWidth)
            {
                float scale = maxBannerWidth / bannerWidth;
                bannerWidth = maxBannerWidth;
                bannerHeight *= scale;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(bannerTexture, GUILayout.Width(bannerWidth), GUILayout.Height(bannerHeight));
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Useful for 7z compression handling.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static async Task<string> ExecuteCrossPlatformShellCmdAsync(string cmd, string args)
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string shell = isWindows ? "cmd.exe" : "/bin/bash";
            string escapedArgs = isWindows ? $"/c {cmd} {args}" : $"-c \"{cmd} {args}\"";
            
            Debug.Log($"[HathoraEditorUtils.ExecuteCrossPlatformShellCmdAsync] " +
                $"shell: {shell}, cmd args: <color=yellow>`{cmd} {args}`</color>");

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
            string output = await outputTask;
            string error = await errorTask;
            string result = output + error;

            return result;
        }

        
        #region 7z Utils
        /// <summary>
        /// Uses the included utils @ project root `/.hathora/7zip/`
        /// </summary>
        /// <param name="deployPaths">Path info</param>
        /// <param name="filesToCompress"></param>
        public static async Task TarballFilesVia7zAsync(
            HathoraUtils.HathoraDeployPaths deployPaths,
            List<string> filesToCompress)
        {
            // file >> file.tar
            string pathToOutputTar = await compressWithTarVia7z(deployPaths, filesToCompress);

            // file.tar >> file.tar.gz ("Tarball")
            string pathToOutputTarGz = await compressTarAsGzVia7z(
                deployPaths, 
                pathToOutputTar,
                _deleteOldTar: true);

            // Assert the tarball exists
            Assert.IsTrue(File.Exists(pathToOutputTarGz),
                $"[HathoraEditorUtils.TarballFilesVia7zAsync] Expected {pathToOutputTarGz} to exist");
        }

        /// <summary>
        /// Turns file.tar into file.tar.gz ("Gzipped" / "Tarball").
        /// </summary>
        /// <param name="_deployPaths"></param>
        /// <param name="_pathToOutputTar"></param>
        /// <param name="_deleteOldTar"></param>
        /// <returns></returns>
        private static async Task<string> compressTarAsGzVia7z(
            HathoraUtils.HathoraDeployPaths _deployPaths,
            string _pathToOutputTar, 
            bool _deleteOldTar)
        {
            string pathToOutputTarGz = $"{_pathToOutputTar}.gz";
            string gzipArgs = $@"a -tgzip ""{pathToOutputTarGz}"" ""{_pathToOutputTar}""";
            
            string gzipResultLogs = await ExecuteCrossPlatformShellCmdAsync(
                _deployPaths.PathTo7zCliExe, 
                gzipArgs);
            
            // TODO: if (verboseLogs)
            Debug.Log($"[HathoraEditorUtils.compressTarAsGzVia7z] " +
                $"tarResultLogs:\n<color=yellow>{gzipResultLogs}</color>");

            if (_deleteOldTar)
                File.Delete(_pathToOutputTar);

            return pathToOutputTarGz;
        }

        /// <summary>
        /// You generally want to .gz, after, to create a tarball.
        /// </summary>
        /// <param name="deployPaths"></param>
        /// <param name="filesToCompress"></param>
        /// <returns>"path/to/output.tar"</returns>
        private static async Task<string> compressWithTarVia7z(
            HathoraUtils.HathoraDeployPaths deployPaths, 
            List<string> filesToCompress)
        {
            string pathToOutputTar = $"{deployPaths.TempDirPath}/{deployPaths.ExeBuildName}.tar";
            string joinedFilesToCompress = string.Join("' '", filesToCompress);
            string tarArgs = $@"a -ttar ""{pathToOutputTar}"" ""{joinedFilesToCompress}""";
            
            string tarResultLogs = await ExecuteCrossPlatformShellCmdAsync(
                deployPaths.PathTo7zCliExe, 
                tarArgs);
            
            // TODO: if (verboseLogs)
            Debug.Log($"[HathoraEditorUtils.compressWithTarVia7z] " +
                $"tarResultLogs:\n<color=yellow>{tarResultLogs}</color>");

            Assert.IsNotNull(tarResultLogs, "[HathoraEditorUtils.compressWithTarVia7z] " +
                $"Error while creating tar archive: {tarResultLogs}");
            
            return pathToOutputTar;
        }
        #endregion // 7z Utils

    }
}

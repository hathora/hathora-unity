// Created by dylan@hathora.dev

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Editor.Common;
using Hathora.Core.Scripts.Runtime.Server.Models;
using UnityEngine.Assertions;

namespace Hathora.Core.Scripts.Editor.Server
{
    public static class HathoraTar
    {
        /// <summary>
        /// Archives the build + Dockerfile into a .tar.gz file.
        /// - Excludes "*_DoNotShip" dirs.
        /// - Eg: "tar -czvf archive.tar.gz --exclude='*_DoNotShip' -C /path/to/dir ."
        /// </summary>
        /// <param name="_deployPaths"></param>
        /// <param name="_cancelToken"></param>
        private static async Task ArchiveFilesAsTarGz(
            HathoraServerDeployPaths _deployPaths, 
            CancellationToken _cancelToken)
        {
            // TODO: Create verbose logs options in UserConfig
            const string cmd = "tar";
            string serverBuildDir = _deployPaths.UserConfig.LinuxHathoraAutoBuildOpts.ServerBuildDirName;
            string shortDateTimeStr = DateTime.UtcNow.ToString("utc-yyyy-MM-dd_HH-mm-ss");
            string archiveNameWithoutExt = $"{_deployPaths.ExeBuildName}-{shortDateTimeStr}";
            string args = $"-czvf {archiveNameWithoutExt}.tar.gz --exclude='*_DoNotShip' -C {serverBuildDir} .";

            (Process process, string resultLog) output = await HathoraEditorUtils.ExecuteCrossPlatformShellCmdAsync(
                cmd,
                args,
                _cancelToken);
            
            // Assert success
            Assert.AreEqual(0, output.process.ExitCode, 
                "Error in `tar` cmd; check logs for details.");

        }
    }
}

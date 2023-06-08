// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Editor.Common;
using Hathora.Core.Scripts.Runtime.Server.Models;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

namespace Hathora.Core.Scripts.Editor.Server
{
    public static class HathoraTar
    {
        /// /// <summary>Overwrites any existing</summary>
        /// <param name="_paths"></param>
        /// <returns>pathToBuildDirDockerfile</returns>
        private static string copyDockerfileFromDotHathoraToBuildDir(HathoraServerPaths _paths)
        {
            string pathToBuildDirDockerfile = $"{_paths.PathToBuildDir}/Dockerfile"; 
            
            File.Copy(
                _paths.PathToDotHathoraDockerfile,
                pathToBuildDirDockerfile,
                overwrite: true);

            return pathToBuildDirDockerfile;
        }

        /// <summary>
        /// Archives the build + Dockerfile into a .tar.gz file.
        /// - Excludes "*_DoNotShip" dirs.
        /// - Eg: "tar -czvf archive.tar.gz --exclude='*_DoNotShip' -C /path/to/dir ."
        /// </summary>
        /// <param name="_paths"></param>
        /// <param name="_cancelToken"></param>
        public static async Task ArchiveFilesAsTarGzToDotHathoraDir(
            HathoraServerPaths _paths, 
            CancellationToken _cancelToken)
        {
            const string cmd = "pwd && tar";
            string outputArchiveNameTarGz = $"{_paths.ExeBuildName}.tar.gz";
            
            HathoraEditorUtils.ValidateCreateDotHathoraDir();
            HathoraEditorUtils.DeleteFileIfExists(_paths.PathToDotHathoraTarGz);
            
            copyDockerfileFromDotHathoraToBuildDir(_paths);
            
            // #################################################################################### 
            // -czvf:
            // c: Create a new archive.
            // z: Compress the archive with gzip.
            // v: Verbosely list the files processed.
            // f: Use archive file or device archive; eg: "{archiveNameWithoutExt}.tar.gz"
            // ####################################################################################
            string args = $"-czvf {outputArchiveNameTarGz} " +
                $"--exclude='*_DoNotShip' " +
                $"--exclude='{outputArchiveNameTarGz}' " +
                $"-C {_paths.PathToBuildDir} *";
            
            string cmdWithArgs = $"{cmd} {args}";
            (Process process, string resultLog) output = default;

            try
            {
                // Create a barebones .tar.gz
                output = await HathoraEditorUtils.ExecuteCrossPlatformShellCmdAsync(
                    _workingDirPath: _paths.PathToBuildDir,
                    cmdWithArgs,
                    _printLogs: true,
                    _cancelToken);
            }
            catch (TaskCanceledException)
            {
                Debug.Log($"[HathoraTar.ArchiveFilesAsTarGzToDotHathoraDir] Task cancelled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraTar.ArchiveFilesAsTarGzToDotHathoraDir] Error " +
                    $"awaiting {nameof(HathoraEditorUtils.ExecuteCrossPlatformShellCmdAsync)}: {e}");
                throw;
            }
            
            // Assert success
            Assert.AreEqual(0, output.process.ExitCode, 
                "Error in `tar` cmd; check logs for details.");
        }
    }
}

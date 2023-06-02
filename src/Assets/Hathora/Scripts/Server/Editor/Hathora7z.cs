using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 7z archive/zipping/tarball static utils
/// </summary>
public static class Hathora7z
{
    /// <summary>
    /// - Uses the included utils @ project root `/.hathora/7zip/`
    /// - Adds /.hathora/Dockerfile
    /// - Adds /{buildDir}
    /// </summary>
    /// <param name="deployPaths">Path info</param>
    /// <param name="filesToCompress"></param>
    /// <param name="_cancelToken"></param>
    public static async Task TarballDeployFilesVia7zAsync(
        HathoraDeployPaths deployPaths,
        List<string> filesToCompress,
        CancellationToken _cancelToken = default)
    {
        // file [ Dockerfile, {buildDir} ] >> file.tar
        string pathToOutputTar = await compressWithTarVia7zAsync(
            deployPaths, 
            filesToCompress,
            _cancelToken);
        
        // file.tar >> file.tar.gz ("Tarball")
        string pathToOutputTarGz = await compressTarAsGzVia7zAsync(
            deployPaths, 
            pathToOutputTar,
            _deleteOldTar: true,
            _cancelToken);

        // Assert the tarball exists
        Assert.IsTrue(File.Exists(pathToOutputTarGz),
            $"[HathoraEditorUtils.TarballDeployFilesVia7zAsync] Expected {pathToOutputTarGz} to exist");
    }

    /// <summary>
    /// Turns file.tar into file.tar.gz ("Gzipped" / "Tarball").
    /// </summary>
    /// <param name="_deployPaths"></param>
    /// <param name="_pathToOutputTar"></param>
    /// <param name="_deleteOldTar"></param>
    /// <param name="_cancelToken"></param>
    /// <returns></returns>
    private static async Task<string> compressTarAsGzVia7zAsync(
        HathoraDeployPaths _deployPaths,
        string _pathToOutputTar, 
        bool _deleteOldTar,
        CancellationToken _cancelToken = default)
    {
        string pathToOutputTarGz = $"{_pathToOutputTar}.gz";
        string gzipArgs = $@"a -tgzip ""{pathToOutputTarGz}"" ""{_pathToOutputTar}""";
        
        string gzipResultLogs = await ExecuteCrossPlatformShellCmdAsync(
            _deployPaths.PathTo7zCliExe, 
            gzipArgs,
            _cancelToken);
        
        // TODO: if (verboseLogs)
        Debug.Log($"[HathoraEditorUtils.compressTarAsGzVia7zAsync] " +
            $"tarResultLogs:\n<color=yellow>{gzipResultLogs}</color>");

        if (_deleteOldTar)
            File.Delete(_pathToOutputTar);

        return pathToOutputTarGz;
    }

    /// <summary>
    /// - Uses the included utils @ project root `/.hathora/7zip/`
    /// - Adds /.hathora/Dockerfile
    /// - Adds /{buildDir}
    /// - You generally want to .gz, after, to create a tarball.
    /// </summary>
    /// <param name="deployPaths"></param>
    /// <param name="filesToCompress"></param>
    /// <param name="_cancelToken"></param>
    /// <returns>"path/to/output.tar"</returns>
    private static async Task<string> compressWithTarVia7zAsync(
        HathoraDeployPaths deployPaths, 
        List<string> filesToCompress,
        CancellationToken _cancelToken = default)
    {
        string pathToOutputTar = $"{deployPaths.TempDirPath}/{deployPaths.ExeBuildName}.tar";
        string joinedFilesToCompress = string.Join(@""" """, filesToCompress);
        const string excludePattern = @"-x!*\*DoNotShip*";
        string tarArgs = $@"a -ttar ""{pathToOutputTar}"" ""{joinedFilesToCompress}"" {excludePattern}";
        
        string tarResultLogs = await ExecuteCrossPlatformShellCmdAsync(
            deployPaths.PathTo7zCliExe, 
            tarArgs,
            _cancelToken);
        
        // TODO: if (verboseLogs)
        Debug.Log($"[HathoraEditorUtils.compressWithTarVia7zAsync] " +
            $"tarResultLogs:\n<color=yellow>{tarResultLogs}</color>");

        Assert.IsNotNull(tarResultLogs, "[HathoraEditorUtils.compressWithTarVia7zAsync] " +
            $"Error while creating tar archive: {tarResultLogs}");
        
        return pathToOutputTar;
    }
}

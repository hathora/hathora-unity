// Created by dylan@hathora.dev

using System.IO;
using System.Runtime.InteropServices;
using Hathora.Core.Scripts.Runtime.Common.Utils;

namespace Hathora.Core.Scripts.Runtime.Server.Models
{
   /// <summary>
    /// Container for all the many paths to prep/upload/deploy a Hathora server build.
    /// </summary>
    public class HathoraServerPaths
    {
        public const string HathoraConsoleAppBaseUrl = "https://console.hathora.dev/application/";
        public const string DotHathoraDirName = ".hathora";

        public readonly HathoraServerConfig UserConfig;
        public readonly string UnityProjRootPath;
        public readonly string DotHathoraDir;
        public readonly string PathToBuildExe;
        public readonly string PathToBuildDir;
        public readonly string PathToDotHathoraDockerfile;

        public string ExeBuildName => UserConfig.LinuxHathoraAutoBuildOpts.ServerBuildExeName;
        public string ExeBuildDir => UserConfig.LinuxHathoraAutoBuildOpts.ServerBuildDirName;

        public HathoraServerPaths(HathoraServerConfig userConfig)
        {
            this.UserConfig = userConfig;
            this.UnityProjRootPath = HathoraUtils.GetNormalizedPathToProjRoot(); // Path slashes normalized
            this.DotHathoraDir = HathoraUtils.NormalizePath(Path.Combine(UnityProjRootPath, DotHathoraDirName));
            this.PathToBuildExe = UserConfig.GetNormalizedPathToBuildExe();
            this.PathToBuildDir = UserConfig.GetNormalizedPathToBuildDir();
            this.PathToDotHathoraDockerfile = HathoraUtils.NormalizePath(
                Path.Join(DotHathoraDir, "Dockerfile"));
        }
    }
}

// Created by dylan@hathora.dev

using System.IO;
using System.Runtime.InteropServices;

namespace Hathora.Scripts.Server.Models
{
   /// <summary>
        /// Container for all the many paths to prep/upload/deploy a Hathora server build.
        /// </summary>
        public class HathoraDeployPaths
        {
            public const string hathoraConsoleAppBaseUrl = "https://console.hathora.dev/application/";

            public readonly NetHathoraConfig UserConfig;
            public readonly string UnityProjRootPath;
            public readonly string TempDirPath;
            public readonly string PathToBuildExe;
            public readonly string PathTo7zCliExe;

            public string PathToDockerfile => HathoraUtils.NormalizePath($"{TempDirPath}/Dockerfile");
            public string ExeBuildName => UserConfig.LinuxHathoraAutoBuildOpts.ServerBuildExeName;
            public string ExeBuildDir => UserConfig.LinuxHathoraAutoBuildOpts.ServerBuildDirName;
            
            private string pathTo7z64bitDir => HathoraUtils.NormalizePath($"{TempDirPath}/7zip/x64");
            private string pathTo7zForWindows => HathoraUtils.NormalizePath($"{pathTo7z64bitDir}/7za.exe");
            private string pathTo7zForMac => HathoraUtils.NormalizePath($"{pathTo7z64bitDir}/7zz-mac");
            private string pathTo7zForLinux => HathoraUtils.NormalizePath($"{pathTo7z64bitDir}/7zz-linux");

            private HathoraDeployPaths() { }

            public HathoraDeployPaths(NetHathoraConfig userConfig)
            {
                this.UserConfig = userConfig;
                this.UnityProjRootPath = HathoraUtils.GetNormalizedPathToProjRoot(); // Path slashes normalized
                this.TempDirPath = HathoraUtils.NormalizePath(Path.Combine(UnityProjRootPath, ".hathora"));
                this.PathToBuildExe = UserConfig.GetNormalizedPathToBuildExe();
                
                // Determine the correct 7z executable to use based on the platform
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    this.PathTo7zCliExe = pathTo7zForWindows;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    this.PathTo7zCliExe = pathTo7zForMac;
                else
                    this.PathTo7zCliExe = pathTo7zForLinux;
            }
        }
}

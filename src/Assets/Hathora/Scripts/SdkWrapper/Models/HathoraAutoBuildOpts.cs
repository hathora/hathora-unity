// Created by dylan@hathora.dev

using System;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraAutoBuildOpts
     {
         // Private Serialized
         /// <summary>Default: Build-Server</summary>
         private string _serverBuildDirName = "Build-Server";

         /// <summary>Default: Build-Server</summary>
         public string ServerBuildDirName
         {
             get => _serverBuildDirName;
             set => _serverBuildDirName = value;
         }

         /// <summary>Default: Hathora-Unity-LinuxServer.x86_64</summary>
         private string _serverBuildExeName = "Hathora-Unity-LinuxServer.x86_64";
         
         /// <summary>Default: Hathora-Unity-LinuxServer.x86_64</summary>
         public string ServerBuildExeName
         {
             get => _serverBuildExeName;
             set => _serverBuildExeName = value;
         }

         /// <summary>The same as checking 'Developer Build' in build opts</summary>
         private bool _isDevBuild = true;
         
         /// <summary>The same as checking 'Developer Build' in build opts</summary>
         public bool IsDevBuild
         {
             get => _isDevBuild;
             set => _isDevBuild = value;
         }
         
         /// <summary>If an old build exists, first delete this dir?</summary>
         private bool _cleanBuildDir = true;

         /// <summary>If an old build exists, first delete this dir?</summary>
         public bool CleanBuildDir
         {
             get => _cleanBuildDir;
             set => _cleanBuildDir = value;
         }
     }
}

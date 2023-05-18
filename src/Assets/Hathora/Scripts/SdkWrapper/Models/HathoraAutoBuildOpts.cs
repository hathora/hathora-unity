// Created by dylan@hathora.dev

using System;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Models
{
    [Serializable]
    public class HathoraAutoBuildOpts
     {
         // Private Serialized
         [SerializeField, Tooltip("Default: Build-Server")]
         private string _serverBuildDirName = "Build-Server";
         public string ServerBuildDirName
         {
             get => _serverBuildDirName;
             set => _serverBuildDirName = value;
         }

         [SerializeField, Tooltip("Default: Hathora-Unity-LinuxServer.x86_64")]
         private string _serverBuildExeName = "Hathora-Unity-LinuxServer.x86_64";
         public string ServerBuildExeName
         {
             get => _serverBuildExeName;
             set => _serverBuildExeName = value;
         }

         [SerializeField, Tooltip("The same as checking 'Developer Build' in build opts")]
         private bool _isDevBuild = true;
         public bool IsDevBuild
         {
             get => _isDevBuild;
             set => _isDevBuild = value;
         }
         
         [SerializeField, Tooltip("If an old build exists, first delete this dir?")]
         private bool _cleanBuildDir = true;
         public bool CleanBuildDir
         {
             get => _cleanBuildDir;
             set => _cleanBuildDir = value;
         }


         // Public utils
         /// <summary>
         /// Explicit typings for FindNestedProperty() calls
         /// </summary>
         public struct SerializedFieldNames
         {
             public static string ServerBuildDirName => nameof(_serverBuildDirName);
             public static string ServerBuildExeName => nameof(_serverBuildExeName);
             public static string IsDevBuild => nameof(_isDevBuild);
             public static string CleanBuildDir => nameof(_cleanBuildDir);
         }
     }
}

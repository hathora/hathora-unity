// Created by dylan@hathora.dev

using System.IO;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Scripts.Net.Common
{
    /// <summary>
    /// Sensitive info will not be included in Client builds.
    /// For meta objects (like the banner and btns), see HathoraConfigEditor.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(NetHathoraConfig), menuName = "Hathora/Config File")]
    public class NetHathoraConfig : ScriptableObject
    {
        #region Vars
        // ----------------------------------------
        [SerializeField]
        private HathoraUtils.ConfigCoreOpts _hathoraCoreOpts;
        public HathoraUtils.ConfigCoreOpts HathoraCoreOpts
        {
            get => _hathoraCoreOpts;
            set => _hathoraCoreOpts = value;
        }

        [SerializeField]
        private HathoraUtils.AutoBuildOpts _linuxAutoBuildOpts;
        public HathoraUtils.AutoBuildOpts LinuxAutoBuildOpts
        {
            get => _linuxAutoBuildOpts;
            set => _linuxAutoBuildOpts = value;
        }

        [SerializeField] 
        private HathoraUtils.HathoraDeployOpts _hathoraDeployOpts;
        public HathoraUtils.HathoraDeployOpts HathoraDeployOpts
        {
            get => _hathoraDeployOpts;
            set => _hathoraDeployOpts = value;
        }
        
        [SerializeField]
        private HathoraUtils.HathoraLobbyRoomOpts _hathoraLobbyRoomOpts;
        public HathoraUtils.HathoraLobbyRoomOpts HathoraLobbyRoomOpts
        {
            get => _hathoraLobbyRoomOpts;
            set => _hathoraLobbyRoomOpts = value;
        }

        /// <summary>
        /// Explicit typings for FindNestedProperty() calls
        /// </summary>
        public struct SerializedFieldNames
        {
            public static string HathoraCoreOpts => nameof(_hathoraCoreOpts);
            public static string LinuxAutoBuildOpts => nameof(_linuxAutoBuildOpts);
            public static string HathoraDeployOpts => nameof(_hathoraDeployOpts);
            public static string HathoraLobbyRoomOpts => nameof(_hathoraLobbyRoomOpts);
        }
        #endregion // Vars


        /// <summary>(!) Don't use OnEnable for ScriptableObjects</summary>
        private void OnValidate()
        {
        }

        public bool MeetsBuildBtnReqs() =>
            !string.IsNullOrEmpty(_linuxAutoBuildOpts.ServerBuildDirName) &&
            !string.IsNullOrEmpty(_linuxAutoBuildOpts.ServerBuildExeName);
                                                          
        public bool MeetsDeployBtnReqs() =>
            !string.IsNullOrEmpty(_hathoraCoreOpts.AppId) &&
            _hathoraCoreOpts.DevAuthOpts.HasAuthToken &&
            !string.IsNullOrEmpty(_linuxAutoBuildOpts.ServerBuildDirName) &&
            !string.IsNullOrEmpty(_linuxAutoBuildOpts.ServerBuildExeName) &&
            _hathoraDeployOpts.TransportInfo.PortNumber > 1024;

        /// <summary>
        /// Combines path, then normalizes
        /// </summary>
        /// <returns></returns>
        public string GetNormalizedPathToBuildExe() => Path.GetFullPath(Path.Combine(
            HathoraUtils.GetNormalizedPathToProjRoot(), 
            _linuxAutoBuildOpts.ServerBuildDirName, 
            _linuxAutoBuildOpts.ServerBuildExeName));
        

    }
}
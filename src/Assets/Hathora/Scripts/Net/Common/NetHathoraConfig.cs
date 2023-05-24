// Created by dylan@hathora.dev

using System.IO;
using Hathora.Scripts.SdkWrapper.Models;
using Hathora.Scripts.Utils;
using UnityEngine;

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
        private HathoraCoreOpts _hathoraCoreOpts = new();
        public HathoraCoreOpts HathoraCoreOpts
        {
            get => _hathoraCoreOpts;
            set => _hathoraCoreOpts = value;
        }

        [SerializeField]
        private HathoraAutoBuildOpts _linuxHathoraAutoBuildOpts = new();
        public HathoraAutoBuildOpts LinuxHathoraAutoBuildOpts
        {
            get => _linuxHathoraAutoBuildOpts;
            set => _linuxHathoraAutoBuildOpts = value;
        }

        [SerializeField] 
        private HathoraDeployOpts _hathoraDeployOpts = new();
        public HathoraDeployOpts HathoraDeployOpts
        {
            get => _hathoraDeployOpts;
            set => _hathoraDeployOpts = value;
        }
        
        [SerializeField]
        private HathoraLobbyRoomOpts _hathoraLobbyRoomOpts = new();
        public HathoraLobbyRoomOpts HathoraLobbyRoomOpts
        {
            get => _hathoraLobbyRoomOpts;
            set => _hathoraLobbyRoomOpts = value;
        }
        #endregion // Vars


        /// <summary>(!) Don't use OnEnable for ScriptableObjects</summary>
        private void OnValidate()
        {
        }

        public bool MeetsBuildBtnReqs() =>
            !string.IsNullOrEmpty(_linuxHathoraAutoBuildOpts.ServerBuildDirName) &&
            !string.IsNullOrEmpty(_linuxHathoraAutoBuildOpts.ServerBuildExeName);
                                                          
        public bool MeetsDeployBtnReqs() =>
            !string.IsNullOrEmpty(_hathoraCoreOpts.AppId) &&
            _hathoraCoreOpts.DevAuthOpts.HasAuthToken &&
            !string.IsNullOrEmpty(_linuxHathoraAutoBuildOpts.ServerBuildDirName) &&
            !string.IsNullOrEmpty(_linuxHathoraAutoBuildOpts.ServerBuildExeName) &&
            _hathoraDeployOpts.ContainerPortWrapper.PortNumber > 1024;

        /// <summary>
        /// Combines path, then normalizes
        /// </summary>
        /// <returns></returns>
        public string GetNormalizedPathToBuildExe() => Path.GetFullPath(Path.Combine(
            HathoraUtils.GetNormalizedPathToProjRoot(), 
            _linuxHathoraAutoBuildOpts.ServerBuildDirName, 
            _linuxHathoraAutoBuildOpts.ServerBuildExeName));
        

    }
}
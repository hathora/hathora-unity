// Created by dylan@hathora.dev

using System.IO;
using Hathora.Scripts.Server.Models;
using UnityEngine;

namespace Hathora.Scripts.Server.Config
{
    /// <summary>
    /// Sensitive info will not be included in Client builds.
    /// For meta objects (like the banner and btns), see `HathoraConfigUI`.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(HathoraServerConfig), menuName = "Hathora/ServerConfig File")]
    public class HathoraServerConfig : ScriptableObject
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
            _hathoraDeployOpts.ContainerPortWrapper.PortNumber >= 1024;
        
        public bool MeetsBuildAndDeployBtnReqs() =>
            MeetsBuildBtnReqs() &&
            MeetsDeployBtnReqs();

        /// <summary>(!) Hathora SDK Enums start at index 1 (not 0).</summary>
        /// <returns></returns>
        public bool MeetsCreateRoomBtnReqs() =>
            HathoraCoreOpts.HasAppId &&
            HathoraLobbyRoomOpts.RegionSelectedIndex > 0 &&
            HathoraLobbyRoomOpts.HathoraRegion > 0;

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
// Created by dylan@hathora.dev

using System.Collections.Generic;
using System.IO;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Application = UnityEngine.Application;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Hathora.Scripts.Net.Server
{
    /// <summary>
    /// Sensitive info will not be included in Client builds.
    /// For meta objects (like the banner and btns), see HathoraConfigEditor.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(HathoraServerConfig), menuName = "Hathora/Server UserConfig")]
    public class HathoraServerConfig : ScriptableObject
    {
        public enum HathoraPlanSize
        {
            Tiny,
            Small,
            Medium,
            Large,
        }


        #region Serialized Fields (+public accessors)
        // ----------------------------------------
        [SerializeField]
        private HathoraUtils.ConfigCoreOpts hathoraCoreOpts;
        public HathoraUtils.ConfigCoreOpts HathoraCoreOpts => hathoraCoreOpts;

        [FormerlySerializedAs("linuxAutoBuildSettings")]
        [SerializeField]
        private HathoraUtils.AutoBuildOpts linuxAutoBuildOpts;
        public HathoraUtils.AutoBuildOpts LinuxAutoBuildOpts => linuxAutoBuildOpts;

        [FormerlySerializedAs("hathoraDeploySettings")]
        [SerializeField] 
        private HathoraUtils.HathoraDeployOpts hathoraDeployOpts;
        private HathoraUtils.HathoraDeployOpts HathoraDeployOpts => hathoraDeployOpts;
        #endregion // Serialized Fields (+public accessors)


        #region Public Shortcuts
        public string AppId => hathoraCoreOpts.AppId;
        public Region Region => hathoraCoreOpts.region;
        #endregion // Public Shortcuts


        /// <summary>(!) Don't use OnEnable for ScriptableObjects</summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(linuxAutoBuildOpts.BuildSceneName))
            {
                string activeSceneName = SceneManager.GetActiveScene().name;
                linuxAutoBuildOpts.BuildSceneName = activeSceneName;
            }
        }

        public bool MeetsBuildBtnReqs() =>
            !string.IsNullOrEmpty(linuxAutoBuildOpts.BuildSceneName) &&
            !string.IsNullOrEmpty(linuxAutoBuildOpts.ServerBuildDirName) &&
            !string.IsNullOrEmpty(linuxAutoBuildOpts.ServerBuildExeName);
                                                          
        public bool MeetsDeployBtnReqs() =>
            !string.IsNullOrEmpty(AppId) &&
            !string.IsNullOrEmpty(hathoraCoreOpts.DevAuthToken) &&
            !string.IsNullOrEmpty(linuxAutoBuildOpts.ServerBuildDirName) &&
            !string.IsNullOrEmpty(linuxAutoBuildOpts.ServerBuildExeName) &&
            hathoraDeployOpts.PortNumber > 1024;
        
        public List<HathoraEnvVars> EnvVars => hathoraDeployOpts.EnvVars;

        public string GetPathToBuildExe() => Path.Combine(
            HathoraUtils.GetNormalizedPathToProjRoot(), 
            linuxAutoBuildOpts.ServerBuildDirName, 
            linuxAutoBuildOpts.ServerBuildExeName);
        

    }
}
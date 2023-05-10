// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.IO;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Utils;
using UnityEditor;
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
        #region Private Serialized Fields
        // ----------------------------------------
        [Header("Options: Core/Build/Deploy")]
        [SerializeField]
        private HathoraUtils.ConfigCoreOpts hathoraCoreOpts;

        [FormerlySerializedAs("linuxAutoBuildSettings")]
        [SerializeField]
        private HathoraUtils.AutoBuildOpts linuxAutoBuildOpts;

        [FormerlySerializedAs("hathoraDeploySettings")]
        [SerializeField] 
        private HathoraUtils.HathoraDeployOpts hathoraDeployOpts;
        #endregion // Private Serialized Fields
        

        #region Public Accessors
        public HathoraUtils.ConfigCoreOpts HathoraCoreOpts => hathoraCoreOpts;
        public HathoraUtils.AutoBuildOpts LinuxAutoBuildOpts => linuxAutoBuildOpts;
        public HathoraUtils.HathoraDeployOpts HathoraDeployOpts => hathoraDeployOpts;
        #endregion // Public Accessors

        
        #region Public Shortcuts
        public string AppId => hathoraCoreOpts.AppId;
        public Region Region => hathoraCoreOpts.Region;
        #endregion // Public Shortcuts
        
        
        #region Public Setters
        /// <summary>
        /// "Refresh" token is best, due to lasting the longest
        /// </summary>
        /// <param name="_val"></param>
        public void SetDevToken(string _val)
        {
            GUI.FocusControl(null); // Unfocus the field before set so we can see the update
            this.hathoraCoreOpts.DevAuthOpts.SetDevAuthToken(_val);
            refreshUi();
        }
        #endregion // Public Setters


        /// <summary>
        /// ScriptableObjects won't update if set within code unless
        /// you unfocus them, without this. This won't even show in ver ctrl until refresh.
        /// </summary>
        private void refreshUi()
        {
            GUI.FocusControl(null);

            // try
            // {
            //     EditorUtility.SetDirty(this);
            //     AssetDatabase.SaveAssets();
            // }
            // catch (Exception e)
            // {
            //     Debug.LogWarning(e);
            // }
        }

        /// <summary>(!) Don't use OnEnable for ScriptableObjects</summary>
        private void OnValidate()
        {
        }

        public bool MeetsBuildBtnReqs() =>
            !string.IsNullOrEmpty(linuxAutoBuildOpts.ServerBuildDirName) &&
            !string.IsNullOrEmpty(linuxAutoBuildOpts.ServerBuildExeName);
                                                          
        public bool MeetsDeployBtnReqs() =>
            !string.IsNullOrEmpty(AppId) &&
            hathoraCoreOpts.DevAuthOpts.HasAuthToken &&
            !string.IsNullOrEmpty(linuxAutoBuildOpts.ServerBuildDirName) &&
            !string.IsNullOrEmpty(linuxAutoBuildOpts.ServerBuildExeName) &&
            hathoraDeployOpts.TransportInfo.PortNumber > 1024;

        public string GetPathToBuildExe() => Path.Combine(
            HathoraUtils.GetNormalizedPathToProjRoot(), 
            linuxAutoBuildOpts.ServerBuildDirName, 
            linuxAutoBuildOpts.ServerBuildExeName);
        

    }
}
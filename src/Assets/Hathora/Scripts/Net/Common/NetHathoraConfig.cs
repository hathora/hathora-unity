// Created by dylan@hathora.dev

using System.IO;
using Hathora.Cloud.Sdk.Model;
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
        #region Private Serialized Fields
        // ----------------------------------------
        [SerializeField]
        private HathoraUtils.ConfigCoreOpts hathoraCoreOpts;

        [SerializeField]
        private HathoraUtils.AutoBuildOpts linuxAutoBuildOpts;

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

        /// <summary>
        /// Combines path, then normalizes
        /// </summary>
        /// <returns></returns>
        public string GetNormalizedPathToBuildExe() => Path.GetFullPath(Path.Combine(
            HathoraUtils.GetNormalizedPathToProjRoot(), 
            linuxAutoBuildOpts.ServerBuildDirName, 
            linuxAutoBuildOpts.ServerBuildExeName));
        

    }
}
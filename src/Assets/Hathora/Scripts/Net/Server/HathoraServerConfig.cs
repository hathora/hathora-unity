// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using FishNet.Managing.Scened;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Hathora.Scripts.Net.Server
{
    /// <summary>
    /// Sensitive info will not be included in Client builds.
    /// For meta objects (like the banner and btns), see HathoraConfigEditor.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(HathoraServerConfig), menuName = "Hathora/Server Config")]
    public class HathoraServerConfig : ScriptableObject
    {
        public enum HathoraPlanSize
        {
            Tiny,
            Small,
            Medium,
            Large,
        }

        
        #region Serialized Fields
        // ----------------------------------------
        [Header("Hathora Core Settings")]
        [SerializeField, Tooltip("Required")]
        private string appId;
        
#if UNITY_SERVER || DEBUG
        /// <summary>
        /// Doc | https://hathora.dev/docs/guides/generate-admin-token 
        /// </summary>
        [SerializeField, Tooltip("[Eventually] Required (for Server calls). " +
             "Not to be confused with the AuthV1 'Player' token. " +
             "See HathoraServerConfig.cs for doc links.")]
        private string devAuthToken;
#endif

        [SerializeField, Tooltip("Required (for Rooms/Lobbies)")]
        private Region region = Region.Seattle;
        
        // ----------------------------------------
        [Header("Auto-Build Settings"), Tooltip("This will auto-fill with the active scene on !focus, if you leave blank.")]
        [SerializeField]
        private string buildSceneName;
        
        [SerializeField]
        private string serverBuildDirName = "Build-Server";
        
        [SerializeField]
        private string serverBuildName = "Hathora-Unity-LinuxServer.x86_64";

        // ----------------------------------------
        [Header("Hathora Deploy Settings")]
        [SerializeField, Tooltip("Default: 1. How many rooms do you want to support per server?")]
        private int roomsPerProcess = 1;

        [FormerlySerializedAs("planSize")]
        [SerializeField, Tooltip("Default: Tiny. Billing Option: You only get charged for active rooms.")]
        private HathoraPlanSize planSizeSize = HathoraPlanSize.Tiny;

        [SerializeField, Tooltip("Default: UDP. UDP is recommended for realtime games: Faster, but less reliable.")]
        private TransportType transportType = TransportType.Udp;

        [SerializeField, Tooltip("Default: 7777")]
        private int portNumber = 7777;

        // [SerializeField, Tooltip("(!) Like an `.env` file, these are all strings. ")]
        // private List<HathoraEnvVars> EnvVars;
        #endregion // Serialized Fields
        
        
        #region Public props
        public string AppId => appId;
        
#if UNITY_SERVER || DEBUG
        public string DevAuthToken => devAuthToken;
#endif
        
        public Region Region => region;
        public string ServerBuildDirName => serverBuildDirName;
        public string ServerBuildName => serverBuildName;
        public int RoomsPerProcess => roomsPerProcess;
        public HathoraPlanSize PlanSizeSize => planSizeSize;
        public TransportType TransportType => transportType;
        public int PortNumber => portNumber;
        #endregion // Public props


        /// <summary>(!) Don't use OnEnable for ScriptableObjects</summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(buildSceneName))
            {
                string activeSceneName = SceneManager.GetActiveScene().name;
                buildSceneName = activeSceneName;
            }
        }


        #region Buttons from HathoraConfigEditor
        public void OnLoginBtnClick()
        {
            throw new NotImplementedException("TODO");
        }

        public void OnDeployToHathoraBtnClick()
        {
            throw new NotImplementedException("TODO");
        }
        #endregion // Buttons from HathoraConfigEditor
    }
}

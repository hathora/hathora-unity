// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Scripts.Client.Config
{
    /// <summary>Hathora Client Config ScriptableObject.</summary>
    [CreateAssetMenu(fileName = nameof(HathoraClientConfig), menuName = "Hathora/Client Config File")]
    public class HathoraClientConfig : ScriptableObject
    {
        #region Vars
        [SerializeField, Tooltip("Get from your Hathora dashboard, or copy from " +
             "Hathora Server Config (t:HathoraServerConfig)")]
        private string _appId;

        /// <summary>
        /// Get from your Hathora dashboard, or copy from Hathora
        /// Server Config (t:HathoraServerConfig).
        /// </summary>
        public string AppId
        {
            get => _appId;
            set => _appId = value;
        }
        
        public bool HasAppId => !string.IsNullOrEmpty(_appId);


        private Region hathoraRegion = Region.Seattle;
        public Region HathoraRegion
        {
            get => hathoraRegion;
            set => hathoraRegion = value;
        }
        #endregion // Vars


        /// <summary>(!) Don't use OnEnable for ScriptableObjects</summary>
        private void OnValidate()
        {
        }
    }
}
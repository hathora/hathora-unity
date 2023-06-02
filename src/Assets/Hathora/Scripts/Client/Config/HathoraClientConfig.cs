// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Scripts.Client.Config
{
    /// <summary>Hathora Client Config ScriptableObject.</summary>
    [CreateAssetMenu(fileName = nameof(HathoraClientConfig), menuName = "Hathora/Client Config File")]
    public class HathoraClientConfig : ScriptableObject
    {
        #region Vars
        [Header("These should match your `HathoraServerConfig`")]
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

        [System.Obsolete("The user should be prompted instead of hard-coding")]
        [SerializeField, Tooltip("You likely want to get user input instead of use this")]
        private Region fallbackRegion = Region.Seattle;

        [System.Obsolete("The user should be prompted instead of hard-coding")]
        public Region FallbackRegion
        {
            get => fallbackRegion;
            set => fallbackRegion = value;
        }
        #endregion // Vars


        /// <summary>(!) Don't use OnEnable for ScriptableObjects</summary>
        private void OnValidate()
        {
        }
    }
}
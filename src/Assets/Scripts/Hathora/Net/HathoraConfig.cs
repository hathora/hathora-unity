// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Net
{
    [CreateAssetMenu(fileName = "HathoraConfig", menuName = "Hathora/Config")]
    public class HathoraConfig : ScriptableObject
    {
        #region Serialized Fields
        [Header("Hathora Config")]
        [SerializeField, Tooltip("Required")]
        public string appId;
        
        [SerializeField, Tooltip("Required")]
        private string authToken;

        [SerializeField, Tooltip("Required (for Rooms/Lobbies)")]
        private Region region = Region.Seattle;
        #endregion // Serialized Fields

        
        public string AppId => appId;
        public string AuthToken => authToken;
        public Region Region => region;
    }
}

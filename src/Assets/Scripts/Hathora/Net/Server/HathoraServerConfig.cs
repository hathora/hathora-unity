// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;
using UnityEngine;

namespace Hathora.Net.Server
{
    /// <summary>
    /// Sensitive info will not be included in Client builds.
    /// </summary>
    [CreateAssetMenu(fileName = "HathoraServerConfig", menuName = "Hathora/Server Config")]
    public class HathoraServerConfig : ScriptableObject
    {
        #region Serialized Fields
        [Header("Hathora Config - to generate a key, see: {TODO_DOC_LINK}")]
        [SerializeField, Tooltip("Required")]
        private string appId;
        
#if UNITY_SERVER || DEBUG
        [SerializeField, Tooltip("Required (for Server calls). Not to be confused with the AuthV1 'Player' token.")]
        private string devAuthToken;
#endif

        [SerializeField, Tooltip("Required (for Rooms/Lobbies)")]
        private Region region = Region.Seattle;
        #endregion // Serialized Fields

        
        public string AppId => appId;
        
#if UNITY_SERVER || DEBUG
        public string DevAuthToken => devAuthToken;
#endif
        
        public Region Region => region;
    }
}

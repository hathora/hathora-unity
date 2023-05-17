// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPostAuthBodyUI : HathoraConfigUIBase
    {
        private readonly SerializedProperty devAuthTokenProp;

        
        #region Init
        public HathoraConfigPostAuthBodyUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
            : base(_config, _serializedConfig)
        {
            // devAuthTokenProp = FindNestedProperty(SerializedConfig, getDevAuthTokenPath()); 
            // Assert.IsNotNull(devAuthTokenProp, "Could not find SerializedProperty for DevAuthToken");
        }

        /// <returns>HathoraCoreOpts,DevAuthOpts,DevAuthToken</returns>
        private static string[] getDevAuthTokenPath() => new[]
        {
            nameof(NetHathoraConfig.HathoraCoreOpts),
            nameof(HathoraUtils.ConfigCoreOpts.DevAuthOpts),
            nameof(HathoraUtils.DevAuthTokenOpts.DevAuthToken),
        };
        #endregion // Init
        
        
        #region Main
        public void Draw()
        {
            if (!IsAuthed)
                return; // You should be calling HathoraConfigPreAuthBodyUI.Draw()

            insertBodyHeader();
            insertServerBuildSettingsDropdown();
            insertDeploymentSettingsDropdown();
        }

        private void insertBodyHeader()
        {
            // insertDevTokenPasswordField();
        }
        
        private void insertServerBuildSettingsDropdown()
        {
            
        }
        
        private void insertDeploymentSettingsDropdown()
        {
            
        }
        #endregion // Main

        
        private void insertDevTokenPasswordField()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"Developer Token", LeftAlignLabelStyle);
            string newPassword = EditorGUILayout.PasswordField(
                devAuthTokenProp.stringValue,
                options: null);

            if (newPassword != devAuthTokenProp.stringValue)
            {
                devAuthTokenProp.stringValue = newPassword;
                SerializedConfig.ApplyModifiedProperties();
            }

            GUILayout.EndHorizontal();
        }
    }
}

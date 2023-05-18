// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Models;
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
            if (!HathoraConfigUI.ENABLE_BODY_STYLE)
                return;
            
            devAuthTokenProp = FindNestedProperty(SerializedConfig, getDevAuthTokenPath()); 
            Assert.IsNotNull(devAuthTokenProp, "Could not find SerializedProperty for DevAuthToken");
        }

        /// <returns>HathoraCoreOpts,DevAuthOpts,DevAuthToken</returns>
        private static string[] getDevAuthTokenPath() => new[]
        {
            NetHathoraConfig.SerializedFieldNames.HathoraCoreOpts,
            HathoraCoreOpts.SerializedFieldNames.DevAuthOpts,
            HathoraDevAuthTokenOpts.SerializedFieldNames.DevAuthToken,
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
            GUILayout.Space(10f);
            insertDevTokenPasswordField();
            GUILayout.Space(10f);
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

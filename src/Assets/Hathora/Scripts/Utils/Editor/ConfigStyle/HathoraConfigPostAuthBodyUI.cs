// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPostAuthBodyUI : HathoraConfigUIBase
    {
        public HathoraConfigPostAuthBodyUI(NetHathoraConfig _config) 
            : base(_config)
        {
        }
        
        
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
            insertDevTokenPasswordField();
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
            
            // Get the new value from the PasswordField
            string newDevAuthToken = EditorGUILayout.PasswordField(Config.HathoraCoreOpts.DevAuthOpts.DevAuthToken);
            GUILayout.EndHorizontal();
            
            // // Check if the value has changed
            // if (newDevAuthToken != Config.HathoraCoreOpts.DevAuthOpts.DevAuthToken)
            // {
            //     // Update the DevAuthToken in the Config
            //     Config.HathoraCoreOpts.DevAuthOpts.DevAuthToken = newDevAuthToken;
            //
            //     // Mark the Config as dirty to ensure the changes are saved
            //     EditorUtility.SetDirty(Config);
            // }
        }
    }
}

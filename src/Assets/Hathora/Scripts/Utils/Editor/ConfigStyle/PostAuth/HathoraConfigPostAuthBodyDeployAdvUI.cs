// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using UnityEditor;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle.PostAuth
{
    public class HathoraConfigPostAuthBodyDeployAdvUI : HathoraConfigUIBase
    {
        #region Vars
        private bool devReAuthLoginButtonInteractable;
        private bool isRefreshingExistingApps;
        
        // Main foldouts
        private bool isServerBuildFoldout;
        private bool isDeploymentFoldout;
        private bool isCreateRoomLobbyFoldout;
        
        // Sub foldouts
        private bool isServerBuildAdvancedFoldout;
        
        // Focus
        private bool buildDirNameTxtFieldHasFocus;
        #endregion // Vars


        #region Init
        public HathoraConfigPostAuthBodyDeployAdvUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
            : base(_config, _serializedConfig)
        {
            if (!HathoraConfigUI.ENABLE_BODY_STYLE)
                return;
        }
        #endregion // Init
        
        
        #region UI Draw
        public void Draw()
        {
            if (!IsAuthed)
                return; // You should be calling HathoraConfigPreAuthBodyUI.Draw()

            insertServerBuildAdvancedFoldout();
        }
       
        private void insertServerBuildAdvancedFoldout()
        {
            isServerBuildAdvancedFoldout = EditorGUILayout.Foldout(
                isServerBuildAdvancedFoldout, 
                "Advanced");

            if (!isServerBuildAdvancedFoldout)
                return;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // TODO
            
            EditorGUILayout.EndVertical();
        }
        #endregion // UI Draw

        
        #region Event Logic
        // TODO
        #endregion // Event Logic
    }
}

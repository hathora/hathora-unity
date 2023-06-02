// Created by dylan@hathora.dev

using UnityEditor;

namespace Hathora.Scripts.Server.Editor.ConfigStyle.PostAuth
{
    public class HathoraConfigPostAuthBodyRoomLobbyUI : HathoraConfigUIBase
    {
        #region Vars
        // Foldouts
        private bool isLobbySettingsFoldout;
        #endregion // Vars


        #region Init
        public HathoraConfigPostAuthBodyRoomLobbyUI(
            HathoraServerConfig _serverConfig, 
            SerializedObject _serializedConfig)
            : base(_serverConfig, _serializedConfig)
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

            insertLobbySettingsSubFoldout();
            InsertSpace3x();
        }
        
        /// <summary>
        /// TODO: Strange things happen if you nest a FoldoutGroup. This would ideally look better, if possible.
        /// </summary>
        private void insertLobbySettingsSubFoldout()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            isLobbySettingsFoldout = EditorGUILayout.Foldout(
                isLobbySettingsFoldout, 
                "Lobby Settings (optional)");
            
            if (!isLobbySettingsFoldout)
            {
                EditorGUILayout.EndVertical(); // End of foldout box skin
                return;
            }
    
            InsertSpace2x();
            EditorGUILayout.EndVertical(); // End of foldout box skin
        }
        #endregion // UI Draw

        
        #region Event Logic
        // TODO
        #endregion // Event Logic
    }
}

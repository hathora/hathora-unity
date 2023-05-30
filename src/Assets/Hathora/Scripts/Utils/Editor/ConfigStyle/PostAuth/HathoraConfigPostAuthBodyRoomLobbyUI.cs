// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using UnityEditor;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle.PostAuth
{
    public class HathoraConfigPostAuthBodyRoomLobbyUI : HathoraConfigUIBase
    {
        #region Vars
        // Foldouts
        private bool isLobbySettingsFoldout;
        #endregion // Vars


        #region Init
        public HathoraConfigPostAuthBodyRoomLobbyUI(
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

            insertLobbySettingsFoldout();
            InsertSpace3x();
        }
        
        private void insertLobbySettingsFoldout()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            isLobbySettingsFoldout = EditorGUILayout.Foldout(
                isLobbySettingsFoldout, 
                "Lobby Settings (optional)");
            
            EditorGUI.indentLevel++;
            if (!isLobbySettingsFoldout)
            {
                EditorGUILayout.EndVertical(); // End of foldout box skin
                return;
            }
    
            InsertSpace2x();

            insertLobbySettingsFoldoutComponents();
                
            EditorGUILayout.EndVertical(); // End of foldout box skin
        }

        private void insertLobbySettingsFoldoutComponents()
        {
            // TODO
        }
        #endregion // UI Draw

        
        #region Event Logic
        // TODO
        #endregion // Event Logic
    }
}

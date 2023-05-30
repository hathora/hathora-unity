// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using UnityEditor;
using UnityEngine;

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
            
            insertLobbySettingsFoldoutComponents();
                
            EditorGUILayout.EndVertical(); // End of foldout box skin
        }

        private void insertLobbySettingsFoldoutComponents()
        {
            BeginFieldIndent();

            insertLobbyInitConfigTextAreaHorizGroup();

            EndFieldIndent();
        }

        private void insertLobbyInitConfigTextAreaHorizGroup()
        {
            string inputStr = base.insertHorizLabeledTextField(
                _labelStr: "Initial Config",
                _tooltip: null,
                _val: Config.HathoraLobbyRoomOpts.InitConfigJson,
                _alignTextField: GuiAlign.SmallRight,
                isTextArea: true);

            bool isChanged = inputStr != Config.HathoraLobbyRoomOpts.InitConfigJson;
            if (isChanged)
                onLobbyInitConfigChanged(inputStr);

            InsertSpace1x();
        }

        private void onLobbyInitConfigChanged(string _inputStr)
        {
            Config.HathoraLobbyRoomOpts.InitConfigJson = _inputStr;
            SaveConfigChange(
                nameof(Config.HathoraLobbyRoomOpts.InitConfigJson), 
                _inputStr);
        }
        #endregion // UI Draw

        
        #region Event Logic
        // TODO
        #endregion // Event Logic
    }
}

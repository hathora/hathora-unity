// Created by dylan@hathora.dev

using System.Collections.Generic;
using Hathora.Cloud.Sdk.Model;
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
            insertLobbyInitConfigTextAreaHorizGroup();
            insertLobbyVisibilityPopupListHorizGroup();
        }

        private void insertLobbyVisibilityPopupListHorizGroup()
        { 
            BeginFieldIndent();
            
            int selectedIndex = Config.HathoraLobbyRoomOpts.LobbyVisibilitySelectedIndex;

            // Get list of string names from PlanName Enum members - with extra info.
            // The index order is !modified.
            List<string> displayOptsStrArr = GetDisplayOptsStrArrFromEnum<CreateLobbyRequest.VisibilityEnum>(
                _prependDummyIndex0Str: "<Lobby Visibility>");

            int newSelectedIndex = base.insertHorizLabeledPopupList(
                _labelStr: "Visibility",
                _tooltip: null,
                _displayOptsStrArr: displayOptsStrArr.ToArray(),
                _selectedIndex: selectedIndex,
                GuiAlign.SmallRight);

            bool isNewValidIndex = selectedIndex >= 0 &&
                newSelectedIndex != selectedIndex &&
                selectedIndex < displayOptsStrArr.Count;

            if (isNewValidIndex)
                onSelectedLobbyVisibilityPopupIndexChanged(newSelectedIndex);
            
            EndFieldIndent();
            InsertSpace1x();
        }

        private void insertLobbyInitConfigTextAreaHorizGroup()
        {
            BeginFieldIndent();

            string inputStr = base.insertHorizLabeledTextField(
                _labelStr: "Initial Config",
                _tooltip: null,
                _val: Config.HathoraLobbyRoomOpts.InitConfigJson,
                _alignTextField: GuiAlign.SmallRight,
                isTextArea: true);

            bool isChanged = inputStr != Config.HathoraLobbyRoomOpts.InitConfigJson;
            if (isChanged)
                onLobbyInitConfigChanged(inputStr);

            EndFieldIndent();
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
        private void onSelectedLobbyVisibilityPopupIndexChanged(int _newSelectedIndex)
        {
            Config.HathoraLobbyRoomOpts.LobbyVisibilitySelectedIndex = _newSelectedIndex;
            SaveConfigChange(
                nameof(Config.HathoraLobbyRoomOpts.LobbyVisibilitySelectedIndex), 
                _newSelectedIndex.ToString());        }
        #endregion // Event Logic
    }
}

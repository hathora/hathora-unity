// Created by dylan@hathora.dev

using System.Collections.Generic;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using UnityEditor;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPostAuthBodyRoomLobbyUI : HathoraConfigUIBase
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

            insertCreateRoomOrLobbyFoldout();
        }

        private void insertCreateRoomOrLobbyFoldout()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            isCreateRoomLobbyFoldout = EditorGUILayout.Foldout(
                isCreateRoomLobbyFoldout, 
                "Create Room or Lobby");
            
            if (isCreateRoomLobbyFoldout)
            {
                EditorGUILayout.EndVertical(); // End of foldout box skin
                return;
            }
    
            EditorGUI.indentLevel++;
            InsertSpace2x();
            
            insertRegionHorizPopupList();
            
            EditorGUILayout.EndVertical(); // End of foldout box skin
            InsertSpace3x();
            EditorGUI.indentLevel--;
        }

        private void insertRegionHorizPopupList()
        {
            int selectedIndex = Config.HathoraLobbyRoomOpts.RegionSelectedIndex;
            
            // Get list of string names from Region Enum members. Set UPPER.
            List<string> displayOptsStrList = GetStrListOfEnumMemberKeys<Region>(
                EnumListOpts.PascalWithSpaces);

            int newSelectedIndex = base.insertHorizLabeledPopupList(
                _labelStr: "Region",
                _tooltip: "Default: `Seattle`",
                _displayOptsStrArr: displayOptsStrList.ToArray(),
                _selectedIndex: selectedIndex,
                GuiAlign.SmallRight);

            bool isNewValidIndex = selectedIndex >= 0 &&
                newSelectedIndex != selectedIndex &&
                selectedIndex < displayOptsStrList.Count;

            if (isNewValidIndex)
                onSelectedRegionPopupIndexChanged(newSelectedIndex);
            
            InsertSpace2x();
        }
        #endregion // UI Draw

        
        #region Event Logic
        private void onSelectedRegionPopupIndexChanged(int _newSelectedIndex)
        {
            Config.HathoraLobbyRoomOpts.RegionSelectedIndex = _newSelectedIndex;
            SaveConfigChange(
                nameof(Config.HathoraLobbyRoomOpts.RegionSelectedIndex), 
                _newSelectedIndex.ToString());
        }
        #endregion // Event Logic
    }
}

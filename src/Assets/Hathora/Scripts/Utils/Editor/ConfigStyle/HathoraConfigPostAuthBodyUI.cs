// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using UnityEditor;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPostAuthBodyUI : HathoraConfigUIBase
    {
        #region Vars
        private HathoraConfigPostAuthBodyHeaderUI _bodyHeaderUI;
        private HathoraConfigPostAuthBodyBuildUI _bodyBuildUI;
        private HathoraConfigPostAuthBodyDeployUI _bodyDeployUI;
        private HathoraConfigPostAuthBodyRoomLobbyUI _bodyRoomLobbyUI;
        
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
        public HathoraConfigPostAuthBodyUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
            : base(_config, _serializedConfig)
        {
            if (!HathoraConfigUI.ENABLE_BODY_STYLE)
                return;
            
            initDrawUtils();
        }
        
        /// <summary>
        /// There are modulated parts of the post-auth body.
        /// </summary>
        private void initDrawUtils()
        {
            _bodyHeaderUI = new HathoraConfigPostAuthBodyHeaderUI(Config, SerializedConfig);
            _bodyBuildUI = new HathoraConfigPostAuthBodyBuildUI(Config, SerializedConfig);
            _bodyDeployUI = new HathoraConfigPostAuthBodyDeployUI(Config, SerializedConfig);
            _bodyRoomLobbyUI = new HathoraConfigPostAuthBodyRoomLobbyUI(Config, SerializedConfig);
        }
        #endregion // Init
        
        
        #region UI Draw
        public void Draw()
        {
            if (!IsAuthed)
                return; // You should be calling HathoraConfigPreAuthBodyUI.Draw()

            _bodyHeaderUI.Draw();
            InsertSpace4x();
            insertBodyFoldouts();
        }

        private void insertBodyFoldouts()
        {
            _bodyBuildUI.Draw();

            InsertSpace1x();
            _bodyDeployUI.Draw();
            
            InsertSpace1x();
            _bodyRoomLobbyUI.Draw();
        }
        #endregion // UI Draw
    }
}

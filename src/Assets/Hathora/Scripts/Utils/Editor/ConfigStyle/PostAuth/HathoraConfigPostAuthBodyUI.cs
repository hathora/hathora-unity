// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using UnityEditor;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle.PostAuth
{
    public class HathoraConfigPostAuthBodyUI : HathoraConfigUIBase
    {
        #region Vars
        private HathoraConfigPostAuthBodyHeaderUI _bodyHeaderUI;
        private HathoraConfigPostAuthBodyBuildUI _bodyBuildUI;
        private HathoraConfigPostAuthBodyDeployUI _bodyDeployUI;
        private HathoraConfigPostAuthBodyRoomUI bodyRoomUI;
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
            bodyRoomUI = new HathoraConfigPostAuthBodyRoomUI(Config, SerializedConfig);
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
            bodyRoomUI.Draw();
        }
        #endregion // UI Draw
    }
}

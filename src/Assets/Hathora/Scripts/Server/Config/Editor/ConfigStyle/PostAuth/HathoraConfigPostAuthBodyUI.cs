// Created by dylan@hathora.dev

using UnityEditor;

namespace Hathora.Scripts.Server.Config.Editor.ConfigStyle.PostAuth
{
    public class HathoraConfigPostAuthBodyUI : HathoraConfigUIBase
    {
        #region Vars
        private HathoraConfigPostAuthBodyHeaderUI bodyHeaderUI {get;set; }
        public HathoraConfigPostAuthBodyBuildUI BodyBuildUI {get;set; }
        public HathoraConfigPostAuthBodyDeployUI BodyDeployUI {get;set; }
        private HathoraConfigPostAuthBodyRoomUI bodyRoomUI {get;set; }
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
            bodyHeaderUI = new HathoraConfigPostAuthBodyHeaderUI(Config, SerializedConfig);
            BodyBuildUI = new HathoraConfigPostAuthBodyBuildUI(Config, SerializedConfig);
            BodyDeployUI = new HathoraConfigPostAuthBodyDeployUI(Config, SerializedConfig);
            bodyRoomUI = new HathoraConfigPostAuthBodyRoomUI(Config, SerializedConfig);
        }
        #endregion // Init
        
        
        #region UI Draw
        public void Draw()
        {
            if (!IsAuthed)
                return; // You should be calling HathoraConfigPreAuthBodyUI.Draw()

            bodyHeaderUI.Draw();
            InsertSpace4x();
            insertBodyFoldoutComponents();
        }

        private void insertBodyFoldoutComponents()
        {
            BodyBuildUI.Draw();

            InsertSpace1x();
            BodyDeployUI.Draw();
            
            InsertSpace1x();
            bodyRoomUI.Draw();
        }
        #endregion // UI Draw
    }
}

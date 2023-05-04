// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Server;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor
{
    [CustomEditor(typeof(HathoraServerConfig))]
    public class HathoraConfigObjectEditor : UnityEditor.Editor
    {
        public HathoraServerConfig GetSelectedInstance() =>
            (HathoraServerConfig)target;
        
        // Create a new GUIStyle for the buttons with custom padding
        private GUIStyle buttonStyle;
        
        /// <summary>Hathora banner</summary>
        public override void OnInspectorGUI()
        {
            insertBanner(); // Place banner @ top
            insertEditingTemplateWarningMemo();
            base.OnInspectorGUI();
            insertButtons(); // Place btns @ bottom
        }

        /// <summary>
        /// Only insert memo if using the template file
        /// </summary>
        private void insertEditingTemplateWarningMemo()
        {
            bool isDefaultName = GetSelectedInstance().name == nameof(HathoraServerConfig);
            if (!isDefaultName)
                return;
            
            EditorGUILayout.HelpBox("You are editing a template! Best practice is to duplicate" +
                "this file and rename it >> then .gitignore it; treat the dupe like an `.env` file.", 
                MessageType.Warning);
            
            GUILayout.Space(10);
        }

        void initButtonStyle()
        {
            buttonStyle = new(GUI.skin.button);
            buttonStyle.padding = new RectOffset(10, 10, 10, 10);
            buttonStyle.richText = true;
            buttonStyle.fontSize = 13;
        }
        
        
        #region Core Buttons
        private void insertButtons()
        {
            initButtonStyle();
            GUILayout.Space(5);

            EditorGUILayout.HelpBox("TODO: Launch OAuth2 window and auto-set the dev token", MessageType.Info);
            if (GUILayout.Button("Login (Set Dev Token)", buttonStyle))
            {
                GetSelectedInstance().OnLoginBtnClick();
            }
            
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("1. Build a dedicated Linux server to `/Build-Server/Build-Server.x86_64`\n" +
                "2. Deploy below.", MessageType.Info);
            if (GUILayout.Button("Deploy to Hathora", buttonStyle))
            {
                GetSelectedInstance().OnDeployToHathoraBtnClick();
                GUILayout.Space(20);
            }
            
            GUILayout.Space(10);
        }

        private void insertBanner()
        {
            Texture2D bannerTexture = Resources.Load<Texture2D>("HathoraConfigBanner");
            if (bannerTexture != null)
            {
                GUILayout.Label(bannerTexture);
            }
        }
        #endregion // Core Buttons

        
        #region Utils
        
        #endregion // Utils

    }
}

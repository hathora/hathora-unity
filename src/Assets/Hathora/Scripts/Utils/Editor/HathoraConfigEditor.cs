// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Server;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor
{
    /// <summary>
    /// The main editor for HathoraServerConfig, including all the button clicks and extra UI.
    /// </summary>
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
            HathoraServerConfig selectedConfig = GetSelectedInstance();
            
            initButtonStyle();
            GUILayout.Space(5);

            insertBuildBtn(selectedConfig);
            GUILayout.Space(10);

            insertDevAuthLoginBtn(selectedConfig);
            GUILayout.Space(10);

            insertHathoraDeployBtn(selectedConfig);
            GUILayout.Space(10);
        }

        private void insertHathoraDeployBtn(HathoraServerConfig selectedConfig)
        {
            GUI.enabled = selectedConfig.MeetsDeployBtnReqs();;
            if (GUI.enabled)
            {
                EditorGUILayout.HelpBox("1. Build a dedicated Linux server to " +
                    "`/Build-Server/Build-Server.x86_64`\n" +
                    "2. Deploy below.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Ensure the core+deploy settings a" +
                    "bove are filled.", MessageType.Error);
            }
            
            if (GUILayout.Button("Deploy to Hathora", buttonStyle))
            {
                HathoraServerDeploy.InitDeployToHathora(selectedConfig);
                GUILayout.Space(20);
            }
        }

        private void insertDevAuthLoginBtn(HathoraServerConfig selectedConfig)
        {
            EditorGUILayout.HelpBox("TODO: Launch OAuth2 window and auto-set the dev token", MessageType.Warning);
            if (GUILayout.Button("Developer Login", buttonStyle))
            {
                GUI.FocusControl(null); // Unfocus so we can see the update instantly later (Unity bug)
                HathoraServerBuild.DevAuthLogin(selectedConfig);
            }
        }

        private void insertBuildBtn(HathoraServerConfig selectedConfig)
        {
            GUI.enabled = selectedConfig.MeetsBuildBtnReqs();;
            EditorGUILayout.HelpBox("Build your dedicated Linux server in 1 click, using the Aut-Build settings above.", MessageType.Info);
            if (GUILayout.Button("Build Linux Server", buttonStyle))
            {
                HathoraServerBuild.BuildHathoraLinuxServer(selectedConfig);
            }
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
    }
}

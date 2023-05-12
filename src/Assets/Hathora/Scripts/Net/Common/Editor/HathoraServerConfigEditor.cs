// Created by dylan@hathora.dev

using System.Threading.Tasks;
using Hathora.Scripts.Net.Server;
using Hathora.Scripts.SdkWrapper.Editor;
using Hathora.Scripts.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Net.Common.Editor
{
    /// <summary>
    /// The main editor for NetHathoraConfig, including all the button clicks and extra UI.
    /// </summary>
    [CustomEditor(typeof(NetHathoraConfig))]
    public class HathoraServerConfigEditor : UnityEditor.Editor
    {
        private static bool devAuthLoginButtonInteractable = true;
        private const string HATHORA_GREEN_HEX = "#76FDBA";
        private static bool buildFoldout = false;
        private static bool authFoldout = false;
        private static bool deployFoldout = false;
        private static bool splitBtns = true; // TODO: Throw this in a settings file
        private static GUIStyle btnsFoldoutStyle;

        
        public NetHathoraConfig GetSelectedInstance() =>
            (NetHathoraConfig)target;
        
        // Create a new GUIStyle for the buttons with custom padding
        private GUIStyle buttonStyle;
        
        /// <summary>Hathora banner</summary>
        public override void OnInspectorGUI()
        {
            HathoraEditorUtils.InsertBanner(); // Place banner @ top
            insertEditingTemplateWarningMemo();
            base.OnInspectorGUI();
            insertButtons(); // Place btns @ bottom
        }
        
        private void drawHorizontalLine(float thickness, Color color)
        {
            Rect lineRect = EditorGUILayout.GetControlRect(hasLabel: false, thickness);
            lineRect.height = thickness;
            EditorGUI.DrawRect(lineRect, color);
        }


        /// <summary>
        /// Only insert memo if using the template file
        /// </summary>
        private void insertEditingTemplateWarningMemo()
        {
            string configTemplateName = $"{nameof(NetHathoraConfig)}.template";
            bool isDefaultName = GetSelectedInstance().name == configTemplateName;
            if (!isDefaultName)
                return;
            
            EditorGUILayout.HelpBox("You are editing a template!\n" +
                "1. Duplicate this (CTRL+D)\n" +
                "2. Add dupe to .gitignore >> treat as an .env file", 
                MessageType.Warning);
            
            GUILayout.Space(10);
        }

        /// <summary>
        /// Adds padding, rich text, and sets font size to 13.
        /// </summary>
        void initButtonStyle()
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(10, 10, 10, 10),
                richText = true,
                fontSize = 13,
            };
        }
 
        private void initBtnsFoldout()
        {
            btnsFoldoutStyle = new GUIStyle(EditorStyles.foldoutHeader)
            {
                richText = true,
            };
        }
        
        private void insertButtons()
        {
            GUILayout.Space(5);
            drawHorizontalLine(1, Color.gray);

            if (splitBtns)
                insertSplitButtons();
            else
                insertSingleBuildAuthDeployButton();

            GUILayout.Space(10);
        }
        
        #region Core Buttons
        private void insertSingleBuildAuthDeployButton()
        {
            NetHathoraConfig selectedConfig = GetSelectedInstance();
            initButtonStyle();
            GUILayout.Space(5);

            GUILayout.Label("Build > Auth > Deploy", EditorStyles.boldLabel);
            insertHathoraDeployBtn(selectedConfig);
        }

        private void insertSplitButtons()
        {
            initBtnsFoldout();
            buildFoldout = EditorGUILayout.Foldout(
                buildFoldout,
                "Actions: Build/Auth/Deploy",
                toggleOnLabelClick: true,
                btnsFoldoutStyle);
            
            if (!buildFoldout)
                return;
            
            // ----------------------
            NetHathoraConfig selectedConfig = GetSelectedInstance();
            initButtonStyle();
            
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Build", EditorStyles.boldLabel);
            insertBuildBtn(selectedConfig);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Auth", EditorStyles.boldLabel);
            insertDevAuthLoginBtn(selectedConfig).Wait(); // Note: Using Wait() here is not ideal, consider refactoring to avoid blocking the main thread
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Deploy", EditorStyles.boldLabel);
            insertHathoraDeployBtn(selectedConfig);
            EditorGUILayout.EndVertical();
        }

        private async Task insertHathoraDeployBtn(NetHathoraConfig selectedConfig)
        {
            GUI.enabled = selectedConfig.MeetsDeployBtnReqs();;
            if (GUI.enabled)
            {
                string step1 = "1. Build a headless Linux server to " +
                    $"`/{selectedConfig.LinuxAutoBuildOpts.ServerBuildDirName}" +
                    $"/{selectedConfig.LinuxAutoBuildOpts.ServerBuildExeName}`\n";
                const string step2 = "2. Authenticates to set a secret dev token.\n";
                const string step3 = "3. Deploy Unity build to Hathora cloud.";
                string helpboxContent = step1 + step2 + step3;
                
                EditorGUILayout.HelpBox(helpboxContent, MessageType.Info);
            }
            else
            {
                if (splitBtns)
                {
                    EditorGUILayout.HelpBox("" +
                        "* Ensure the core+deploy settings above are set.\n" +
                        "* Ensure you have a linux server built.\n" +
                        "* Auth above, if dev token is not set.", 
                        MessageType.Error);
                }

            }
            
            if (GUILayout.Button("Deploy to Hathora", buttonStyle))
            {
                await HathoraServerDeploy.DeployToHathoraAsync(selectedConfig);
                GUILayout.Space(20);
            }
            
            GUI.enabled = true;
        }

        private async Task insertDevAuthLoginBtn(NetHathoraConfig selectedConfig)
        {
            bool hasAuthToken = !string.IsNullOrEmpty(selectedConfig.HathoraCoreOpts.DevAuthOpts.DevAuthToken);
            bool pendingGuiEnable = devAuthLoginButtonInteractable && !hasAuthToken;

            string btnStr = "Developer Login";
            if (!pendingGuiEnable)
            {
                if (hasAuthToken)
                {
                    EditorGUILayout.HelpBox("Unset `dev auth token` to relogin", MessageType.Info);
                    btnStr = $"<color={HATHORA_GREEN_HEX}>Dev Auth Token Set!</color>";
                }
                else
                {
                    EditorGUILayout.HelpBox("Your default browser should have popped up.", MessageType.Info);
                    btnStr = "<color=yellow>Awaiting Auth...</color>";
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Launch a browser window to login and set your " +
                    "`dev auth token` above.", MessageType.Info);
            }

            GUI.enabled = pendingGuiEnable;
            EditorGUI.BeginDisabledGroup(!devAuthLoginButtonInteractable);
            if (GUILayout.Button(btnStr, buttonStyle))
            {
                GUI.FocusControl(null); // Unfocus to refresh UI // TODO: Is Repaint() better?
                devAuthLoginButtonInteractable = false;
                
                await HathoraServerAuth.DevAuthLogin(selectedConfig);
                
                devAuthLoginButtonInteractable = true;
                Repaint();
                EditorGUI.EndDisabledGroup();
            }

            GUI.enabled = true;
        }

        private void insertBuildBtn(NetHathoraConfig selectedConfig)
        {
            GUI.enabled = selectedConfig.MeetsBuildBtnReqs();
            EditorGUILayout.HelpBox("Build your dedicated Linux server in 1 click, " +
                "using the Auto-Build settings above.", MessageType.Info);
            
            if (GUILayout.Button("Build Linux Server", buttonStyle))
            {
                HathoraServerBuild.BuildHathoraLinuxServer(selectedConfig);
            }
            
            GUI.enabled = true;
        }
        #endregion // Core Buttons
    }
}

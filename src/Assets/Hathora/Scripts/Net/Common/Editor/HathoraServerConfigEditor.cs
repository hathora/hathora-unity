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
        private const string HATHORA_VIOLET_COLOR_HEX = "#EEDDFF";
        private static GUIStyle btnsFoldoutStyle;
        private static GUIStyle leftAlignLabelStyle;
        private static GUIStyle centerAlignLabelStyle;
        private static GUIStyle rightAlignLabelStyle;
        private string previousDevAuthToken;
        
        public NetHathoraConfig GetSelectedInstance() =>
            (NetHathoraConfig)target;
        
        // Create a new GUIStyle for the buttons with custom padding
        private GUIStyle buttonStyle;
        
        /// <summary>Hathora banner</summary>
        public override void OnInspectorGUI()
        {
            initStyles();
            initButtonStyle();
            HathoraEditorUtils.InsertBanner(_includeVerticalGroup: true); // Place banner @ top
            
            NetHathoraConfig selectedConfig = GetSelectedInstance();
            insertEditingTemplateWarningMemo(selectedConfig);
            
            bool isAuthed = selectedConfig.HathoraCoreOpts.DevAuthOpts.HasAuthToken; 
            if (isAuthed)
                base.OnInspectorGUI(); // Hide entire config

            insertButtons(selectedConfig, isAuthed); // Show only auth button, if !authed
            
            // The real config is hidden, so we add fields to pass to config later
            if (!isAuthed)
                insertUnauthedUi(selectedConfig);
        }

        private void initStyles()
        {
            initButtonStyle();
            initBtnFoldoutsStyle();
            initLabelsStyle();
        }

        private void initLabelsStyle()
        {
            leftAlignLabelStyle = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleLeft,
            };
            
            centerAlignLabelStyle = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
            };
            
            rightAlignLabelStyle = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleRight,
            };
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
        private void insertEditingTemplateWarningMemo(NetHathoraConfig _selectedConfig)
        {
            string configTemplateName = $"{nameof(NetHathoraConfig)}.template";
            bool isDefaultName = _selectedConfig.name == configTemplateName;
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
 
        private void initBtnFoldoutsStyle()
        {
            btnsFoldoutStyle = new GUIStyle(EditorStyles.foldoutHeader)
            {
                richText = true,
            };
        }
        
        private void insertButtons(NetHathoraConfig _config, bool _isAuthed)
        {
            GUILayout.Space(5);

            insertSplitButtons(_config, _isAuthed);

            GUILayout.Space(10);
        }
        
        #region Core Buttons
        private void insertSplitButtons(NetHathoraConfig _config, bool _isAuthed)
        {
            GUILayout.Space(5);

            if (!_isAuthed)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_HEX}>" +
                    "<b>Welcome!</b> Get started below</color>", centerAlignLabelStyle);
                insertDevAuthLoginBtn(_config);
                EditorGUILayout.EndVertical();
                return;
            }

            // drawHorizontalLine(1, Color.gray);
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_HEX}>" +
                "Customize via `Linux Auto Build Opts`</color>", centerAlignLabelStyle);
            insertBuildBtn(_config);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_HEX}>" +
                "Customize via `Hathora Deploy Opts`</color>", centerAlignLabelStyle);
            insertHathoraDeployBtn(_config);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// We already added an auth button, leading up to this point.
        /// We always want the AppId and DevAuthToken fields to be visible.
        /// Since this is just a styler, the "real" fields are in the actual Config (invisible).
        /// </summary>
        private void insertUnauthedUi(NetHathoraConfig _config)
        {
            insertDevTokenPasswordField(_config);
            // TODO: What else?
        }

        private void insertDevTokenPasswordField(NetHathoraConfig _config)
        {
            GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_HEX}>" +
                "Or paste an existing token:</color>", rightAlignLabelStyle);
            
            const string previousPassword = "";
            string currentDevAuthToken = EditorGUILayout.PasswordField(
                "Dev Auth Token", previousPassword);

            if (currentDevAuthToken != previousDevAuthToken)
            {
                _config.SetDevToken(currentDevAuthToken);
                previousDevAuthToken = currentDevAuthToken;
            }
        }

        private async Task insertHathoraDeployBtn(NetHathoraConfig selectedConfig)
        {
            GUI.enabled = selectedConfig.MeetsDeployBtnReqs();;

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

            string btnStr = "Hathora Login";
            if (!pendingGuiEnable)
            {
                if (hasAuthToken)
                    btnStr = $"<color={HATHORA_GREEN_HEX}>Dev Auth Token Set!</color>";
                else
                {
                    EditorGUILayout.HelpBox("Your default browser should have popped up.", MessageType.Info);
                    btnStr = "<color=yellow>Awaiting Auth...</color>";
                }
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
            
            if (GUILayout.Button("Build Linux Server", buttonStyle))
            {
                HathoraServerBuild.BuildHathoraLinuxServer(selectedConfig);
            }
            
            GUI.enabled = true;
        }
        #endregion // Core Buttons
    }
}

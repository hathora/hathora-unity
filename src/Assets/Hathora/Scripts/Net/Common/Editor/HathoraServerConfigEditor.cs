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
        
        private static GUIStyle centerAlignLabelStyle;
        private static GUIStyle centerAlignLargerTxtLabelNoWrapStyle;
        private static GUIStyle leftAlignLabelStyle;
        private static GUIStyle centerLinkLabelStyle;
        private static GUIStyle rightAlignLabelStyle;
        private static GUIStyle preLinkLabelStyle;
        private static GUIStyle generalButtonStyle;
        private static GUIStyle btnsFoldoutStyle;
                
        private string previousDevAuthToken;
        
        public NetHathoraConfig GetSelectedInstance() =>
            (NetHathoraConfig)target;
        
        
        /// <summary>Hathora banner</summary>
        public override void OnInspectorGUI()
        {
            // Create style
            serializedObject.Update();
            initStyles();
            initButtonStyles();
            NetHathoraConfig selectedConfig = GetSelectedInstance();

            // Insert style
            insertTopHeader(selectedConfig);
            insertMiddleBody(selectedConfig);
            insertBottomFooter(selectedConfig);
        }

        private void insertBottomFooter(NetHathoraConfig _config)
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUILayout.Label("To learn more about Hathora Cloud, check out our ", preLinkLabelStyle);
            insertLinkLabel("documentation", HathoraEditorUtils.HATHORA_DOCS_URL);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void insertLinkLabel(string _label, string _url)
        {
            if (EditorGUILayout.LinkButton(
                    _label,
                    GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(_url);
            }
        }

        private void insertMiddleBody(NetHathoraConfig _config)
        {
            insertEditingTemplateWarningMemo(_config);
            
            bool isAuthed = _config.HathoraCoreOpts.DevAuthOpts.HasAuthToken; 
            if (isAuthed)
                base.OnInspectorGUI(); // Hide entire config

            insertButtons(_config, isAuthed); // Show only auth button, if !authed
        }

        private void insertTopHeader(NetHathoraConfig _config)
        {
            HathoraEditorUtils.InsertBanner(
                _includeVerticalGroup: false,
                _wrapperExtension: 30f); // Place banner @ top
            GUILayout.Label("Multiplayer Server Hosting", centerAlignLargerTxtLabelNoWrapStyle);
            GUILayout.EndVertical();
        }

        private void initStyles()
        {
            initButtonStyles();
            initBtnFoldoutStyles();
            initLabelStyles();
        }

        private static void initLabelStyles()
        {
            leftAlignLabelStyle = HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleLeft);
            centerAlignLabelStyle = HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleCenter);
            rightAlignLabelStyle = HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleRight);
            centerLinkLabelStyle = HathoraEditorUtils.GetRichLinkStyle(TextAnchor.MiddleCenter);
            preLinkLabelStyle = HathoraEditorUtils.GetPreLinkLabelStyle();
            centerAlignLargerTxtLabelNoWrapStyle = HathoraEditorUtils.GetRichLabelStyle(
                TextAnchor.MiddleCenter,
                _wordWrap: false,
                _fontSize: 15);
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
        static void initButtonStyles()
        {
            generalButtonStyle = HathoraEditorUtils.GetRichButtonStyle();
        }
 
        private static void initBtnFoldoutStyles()
        {
            btnsFoldoutStyle = HathoraEditorUtils.GetRichFoldoutHeaderStyle();
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
                    "Create an account or log in to Hathora Cloud's Console to get started</color>", 
                    centerAlignLabelStyle);
                GUILayout.Space(10f);

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

        private static async Task insertHathoraDeployBtn(NetHathoraConfig selectedConfig)
        {
            GUI.enabled = selectedConfig.MeetsDeployBtnReqs();;

            if (GUILayout.Button("Deploy to Hathora", generalButtonStyle))
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

            string btnStr = "Register or log in to Hathora Console";
            if (!pendingGuiEnable)
            {
                if (hasAuthToken)
                    btnStr = $"<color={HathoraEditorUtils.HATHORA_GREEN_HEX}>Dev Auth Token Set!</color>";
                else
                {
                    btnStr = "<color=yellow>Awaiting browser login...</color>";
                }
            }

            GUI.enabled = pendingGuiEnable;
            EditorGUI.BeginDisabledGroup(!devAuthLoginButtonInteractable);
            if (GUILayout.Button(btnStr, generalButtonStyle))
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

        private static void insertBuildBtn(NetHathoraConfig selectedConfig)
        {
            GUI.enabled = selectedConfig.MeetsBuildBtnReqs();
            
            if (GUILayout.Button("Build Linux Server", generalButtonStyle))
            {
                HathoraServerBuild.BuildHathoraLinuxServer(selectedConfig);
            }
            
            GUI.enabled = true;
        }
        #endregion // Core Buttons
    }
}

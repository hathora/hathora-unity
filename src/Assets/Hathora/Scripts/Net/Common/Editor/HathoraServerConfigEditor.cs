// Created by dylan@hathora.dev

using System.Threading;
using System.Threading.Tasks;
using Hathora.Scripts.Net.Server;
using Hathora.Scripts.SdkWrapper.Editor;
using Hathora.Scripts.Utils.Editor;
using UnityEditor;
using UnityEditor.Build.Reporting;
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
        private static GUIStyle bigButtonStyle;
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
            bool isAuthed = selectedConfig.HathoraCoreOpts.DevAuthOpts.HasAuthToken; 

            // Insert style
            insertTopHeader(selectedConfig, isAuthed);
            insertMiddleBody(selectedConfig, isAuthed);
            insertBottomFooter(selectedConfig, isAuthed);
        }

        private void insertBottomFooter(NetHathoraConfig _config, bool _isAuthed)
        {
            if (_isAuthed)
            {
                insertBuildUploadDeployComboBtn(_config);
                return;
            }
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Learn more about Hathora Cloud", preLinkLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            insertLinkLabel("Documentation", HathoraEditorUtils.HATHORA_DOCS_URL);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
 
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            insertLinkLabel("Demo Projects", HathoraEditorUtils.HATHORA_DOCS_DEMO_PROJECTS_URL);
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

        private void insertMiddleBody(NetHathoraConfig _config, bool _isAuthed)
        {
            insertEditingTemplateWarningMemo(_config);
            
            if (_isAuthed)
                base.OnInspectorGUI(); // Hide entire config

            insertButtons(_config, _isAuthed); // Show only auth button, if !authed
        }

        private void insertTopHeader(NetHathoraConfig _config, bool _isAuthed)
        {
            HathoraEditorUtils.InsertBanner(
                _includeVerticalGroup: false,
                _wrapperExtension: 55f); // Place banner @ top
            HathoraEditorUtils.InsertHathoraSloganLbl();

            EditorGUILayout.Space(5);
            insertHeaderBtns(_config);
            GUILayout.EndVertical();
        }

        private void insertHeaderBtns(NetHathoraConfig _config)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUILayoutOption[] buttonOptions =
            {
                GUILayout.MinWidth(100), 
                GUILayout.MinHeight(20), 
                GUILayout.ExpandWidth(true),
            };
            
            if (GUILayout.Button("Get Started", generalButtonStyle, buttonOptions))
            {
                Application.OpenURL(HathoraEditorUtils.HATHORA_DOCS_GETTING_STARTED_URL);
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Unity Tutorial", generalButtonStyle, buttonOptions))
            {
                Application.OpenURL(HathoraEditorUtils.HATHORA_DOCS_UNITY_TUTORIAL_URL);
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Discord", generalButtonStyle, buttonOptions))
            {
                Application.OpenURL(HathoraEditorUtils.HATHORA_DISCORD_URL);
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Website", generalButtonStyle, buttonOptions))
            {
                Application.OpenURL(HathoraEditorUtils.HATHORA_HOME_URL);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void initStyles()
        {
            initButtonStyles();
            initBtnFoldoutStyles();
            initLabelStyles();
        }

        private static void initLabelStyles()
        {
            leftAlignLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleLeft);
            centerAlignLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleCenter);
            rightAlignLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleRight);
            centerLinkLabelStyle ??= HathoraEditorUtils.GetRichLinkStyle(TextAnchor.MiddleCenter);
            preLinkLabelStyle ??= HathoraEditorUtils.GetPreLinkLabelStyle();
            centerAlignLargerTxtLabelNoWrapStyle ??= HathoraEditorUtils.GetRichLabelStyle(
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
            
            EditorGUILayout.Space(10);
        }

        /// <summary>
        /// Adds padding, rich text, and sets font size to 13.
        /// </summary>
        private static void initButtonStyles()
        {
            generalButtonStyle ??= HathoraEditorUtils.GetRichButtonStyle();
            bigButtonStyle ??= HathoraEditorUtils.GetBigButtonStyle();
        }
 
        private static void initBtnFoldoutStyles()
        {
            btnsFoldoutStyle ??= HathoraEditorUtils.GetRichFoldoutHeaderStyle();
        }
        
        private void insertButtons(NetHathoraConfig _config, bool _isAuthed)
        {
            EditorGUILayout.Space(5);
            insertSplitButtons(_config, _isAuthed);
            EditorGUILayout.Space(10);
        }
        
        #region Core Buttons
        private void insertSplitButtons(NetHathoraConfig _config, bool _isAuthed)
        {
            EditorGUILayout.Space(5);

            if (!_isAuthed)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_HEX}>" +
                    "Create an account or log in to Hathora Cloud's Console to get started</color>", 
                    centerAlignLabelStyle);
                EditorGUILayout.Space(10f);

                insertDevAuthLoginBtn(_config);
                EditorGUILayout.EndVertical();
                return;
            }

            // drawHorizontalLine(1, Color.gray);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_HEX}>" +
                "Customize via `Linux Auto Build Opts`</color>", centerAlignLabelStyle);
            insertBuildBtn(_config);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

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
                EditorGUILayout.Space(20);
            }
            
            GUI.enabled = true;
        }
        
        private static async Task insertBuildUploadDeployComboBtn(NetHathoraConfig selectedConfig)
        {
            GUI.enabled = selectedConfig.MeetsDeployBtnReqs();;

            EditorGUILayout.HelpBox("This action will create a new server build, upload to Hathora, " +
                "and create a new development version of your application.", MessageType.Info);
            if (GUILayout.Button("Build, Upload & Deploy New Version", generalButtonStyle))
            {
                BuildReport buildReport = HathoraServerBuild.BuildHathoraLinuxServer(selectedConfig);
                if (buildReport.summary.result != BuildResult.Succeeded)
                    return;
                
                await HathoraServerDeploy.DeployToHathoraAsync(selectedConfig);
                EditorGUILayout.Space(20);
            }
            
            GUI.enabled = true;
        }

        private async Task insertDevAuthLoginBtn(NetHathoraConfig selectedConfig)
        {
            bool hasAuthToken = !string.IsNullOrEmpty(selectedConfig.HathoraCoreOpts.DevAuthOpts.DevAuthToken);
            bool pendingGuiEnable = devAuthLoginButtonInteractable && !hasAuthToken;

            string btnStr = pendingGuiEnable
                ? "Register or log in to Hathora Console"
                : "<color=yellow>Awaiting browser login...</color>";

            EditorGUI.BeginDisabledGroup(!devAuthLoginButtonInteractable);
            
            if (GUILayout.Button(btnStr, bigButtonStyle))
            {
                devAuthLoginButtonInteractable = false;
                await HathoraServerAuth.DevAuthLogin(selectedConfig);
                devAuthLoginButtonInteractable = true; 
            }
            
            EditorGUI.EndDisabledGroup(); 

            if (HathoraServerAuth.HasCancellableToken && !devAuthLoginButtonInteractable)
            {
                insertAuthCancelBtn(HathoraServerAuth.ActiveCts);
            }
        }

        private void insertAuthCancelBtn(CancellationTokenSource _cts) 
        {
            if (GUILayout.Button("Cancel", generalButtonStyle))
            {
                _cts?.Cancel();
                devAuthLoginButtonInteractable = true;
            }
            Repaint();
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

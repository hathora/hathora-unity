// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using Hathora.Scripts.SdkWrapper.Editor.ApiWrapper;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Application = Hathora.Cloud.Sdk.Model.Application;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPostAuthBodyUI : HathoraConfigUIBase
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
        public HathoraConfigPostAuthBodyUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
            : base(_config, _serializedConfig)
        {
            // if (!HathoraConfigUI.ENABLE_BODY_STYLE)
            //     return;
        }
        #endregion // Init
        
        
        #region UI Draw
        public void Draw()
        {
            if (!IsAuthed)
                return; // You should be calling HathoraConfigPreAuthBodyUI.Draw()

            // EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            insertBodyHeader();
            // EditorGUILayout.EndVertical();

            EditorGUILayout.Space(30f);
            insertFoldouts();
        }

        private void insertBodyHeader()
        {
            insertLoginTokenGroup();
            
            // InsertHorizontalLine(1f, Color.gray, _space: 15);
            GUILayout.Space(20f); // Space looks better than line w/group boxes
            
            insertAppIdGroup();
        }

        private void insertLoginTokenGroup()
        {
            insertDevTokenPasswordField();
            insertLoginToHathoraConsoleBtn(); // !await
            
            EditorGUILayout.Space(10);
        }

        private void insertFoldouts()
        {
            insertServerBuildSettingsFoldout();

            insertFieldVertSpace();
            insertDeploymentSettingsFoldout();
            
            insertFieldVertSpace();
            insertCreateRoomOrLobbyFoldout();
        }
        
        private void insertAppIdGroup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            insertAppIdHorizHeader();
            insertAppsListPopupListHorizGroup();
            insertAppIdDisplayCopyGroup();
            
            EditorGUILayout.EndVertical();
        }
        
        private void insertAppIdHorizHeader()
        {
            GUILayout.BeginHorizontal();
            
            insertTargetAppLabelWithTooltip();
            GUILayout.FlexibleSpace();
            insertSelectAppToUseOpacityLabel();
            
            GUILayout.EndHorizontal();
        }
        
        /// <summary>(!) Despite its name, a Popup() is actually a dropdown list</summary>
        private void insertAppsListPopupListHorizGroup()
        {
            EditorGUI.BeginDisabledGroup(disabled: isRefreshingExistingApps);
            GUILayout.BeginHorizontal();
            
            insertExistingAppsPopupList(); // This actually drops down, despite the name
            insertExistingAppsRefreshBtn(); // !await
            
            GUILayout.EndHorizontal(); 
            EditorGUI.EndDisabledGroup();
        }
        
        private void insertAppIdDisplayCopyGroup()
        {
            if (!CheckHasSelectedApp())
                return;
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>AppId:</b>", LeftAlignLabelStyle, GUILayout.ExpandWidth(false));

            string selectedAppId = Config.HathoraCoreOpts.AppId;
            base.insertLeftSelectableLabel(selectedAppId);
            GUILayout.FlexibleSpace();
            
            // USER INPUT >>
            bool clickedCopyAppIdBtn = insertLeftGeneralBtn("Copy AppId");
            if (clickedCopyAppIdBtn)
                onCopyAppIdBtnClick(selectedAppId);

            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void onCopyAppIdBtnClick(string _selectedAppId)
        {
            GUIUtility.systemCopyBuffer = _selectedAppId; // Copy to clipboard
            Debug.Log($"Copied AppId to clipboard: `{_selectedAppId}`");
        }

        private void insertSelectAppToUseOpacityLabel()
        {
            GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GRAY_TRANSPARENT_COLOR_HEX}>" +
                "Select an application to use</color>", CenterAlignLabelStyle);
        }

        private async Task insertExistingAppsRefreshBtn()
        {
            // USER INPUT >>
            bool clickedAppRefreshBtn = insertLeftGeneralBtn("↻ Refresh List"); 
            if (clickedAppRefreshBtn)
                onRefreshAppsListBtnClick(); // !await
        }

        /// <summary>(!) Despite its name, a Popup() is actually a dropdown list</summary>
        private void insertExistingAppsPopupList()
        {
            List<string> displayedOptionsList = Config.HathoraCoreOpts.GetExistingAppNames();
            string[] displayedOptionsArr = displayedOptionsList?.ToArray();
    
            int selectedIndex = Config.HathoraCoreOpts.ExistingAppsSelectedIndex;
    
            int newSelectedIndex = EditorGUILayout.Popup(
                selectedIndex, 
                displayedOptionsArr,
                GUILayout.ExpandWidth(true));

            bool isNewValidIndex = displayedOptionsList != null &&
                selectedIndex >= 0 &&
                newSelectedIndex != selectedIndex &&
                selectedIndex < displayedOptionsList.Count;

            if (isNewValidIndex)
                onSelectedPopupAppChanged(newSelectedIndex);
            
            GUILayout.Space(10f);
        }

        private async Task insertLoginToHathoraConsoleBtn()
        {
            EditorGUI.BeginDisabledGroup(disabled: false); 
            
            if (GUILayout.Button("Log in with another account", GeneralButtonStyle))
            {
                devReAuthLoginButtonInteractable = false;
                await HathoraServerAuth.DevAuthLogin(Config);
                devReAuthLoginButtonInteractable = true; 
                InvokeRequestRepaint();
            }
            
            EditorGUI.EndDisabledGroup();

            if (HathoraServerAuth.HasCancellableToken && !devReAuthLoginButtonInteractable)
                insertAuthCancelBtn(HathoraServerAuth.ActiveCts);
        }
        
        private void insertAuthCancelBtn(CancellationTokenSource _cts) 
        {
            if (GUILayout.Button("Cancel", GeneralButtonStyle))
            {
                _cts?.Cancel();
                devReAuthLoginButtonInteractable = true;
            }
            
            InvokeRequestRepaint();
        }
        
        private void insertServerBuildSettingsFoldout()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            isServerBuildFoldout = EditorGUILayout.Foldout(
                isServerBuildFoldout,
                "Server Build Settings");

            if (!isServerBuildFoldout)
            {
                EditorGUILayout.EndVertical(); // End of foldout box skin
                return;
            }
            
            EditorGUI.indentLevel++;
            EditorGUILayout.Space(10);
            
            insertBuildDirNameHorizGroup();
            insertBuildFileExeNameHorizGroup();
            insertServerBuildAdvancedFoldout();

            EditorGUILayout.Space(10);
            insertGenerateServerBuildBtn(); // !await
            
            EditorGUILayout.EndVertical(); // End of foldout box skin
            EditorGUI.indentLevel--;
        }

        private async Task insertGenerateServerBuildBtn()
        {
            bool clickedBuildBtn = insertLeftGeneralBtn("Generate Server Build");
            if (!clickedBuildBtn)
                return;
            
            BuildReport buildReport = HathoraServerBuild.BuildHathoraLinuxServer(Config);
            Assert.That(
                buildReport.summary.result,
                Is.EqualTo(BuildResult.Succeeded),
                "Server build failed. Check console for details.");
        }

        private void insertServerBuildAdvancedFoldout()
        {
            isServerBuildAdvancedFoldout = EditorGUILayout.Foldout(
                isServerBuildAdvancedFoldout, 
                "Advanced");

            if (!isServerBuildAdvancedFoldout)
                return;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // TODO
            
            EditorGUILayout.EndVertical();
        }
        
        private void insertBuildDirNameHorizGroup()
        {
            string inputStr = base.insertHorizLabeledTextField(
                _labelStr: "Build directory",
                _tooltip: "Default: `Build-Linux-Server`",
                _val: Config.LinuxHathoraAutoBuildOpts.ServerBuildDirName);

            bool isChanged = inputStr != Config.LinuxHathoraAutoBuildOpts.ServerBuildDirName;
            if (isChanged)
                onServerBuildDirChanged(inputStr);

            insertFieldVertSpace();
        }
        
        private void insertBuildFileExeNameHorizGroup()
        {
            string inputStr = base.insertHorizLabeledTextField(
                _labelStr: "Build file name", 
                _tooltip: "Default: `Unity-LinuxServer.x86_64",
                _val: Config.LinuxHathoraAutoBuildOpts.ServerBuildExeName);
            
            bool isChanged = inputStr != Config.LinuxHathoraAutoBuildOpts.ServerBuildExeName;
            if (isChanged)
                onServerBuildExeNameChanged(inputStr);
            
            insertFieldVertSpace();
        }

        private void insertDeploymentSettingsFoldout()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            isDeploymentFoldout = EditorGUILayout.Foldout(
                isDeploymentFoldout, 
                "Hathora Deployment Configuration");
            
            if (isDeploymentFoldout)
            {
                EditorGUILayout.EndVertical(); // End of foldout box skin
                return;
            }
    
            EditorGUILayout.Space(10);
            
            insertDeployAppHelpbox(); // indentLevel is buggy, here: Keep it above
            
            EditorGUI.indentLevel++;
            insertDeployAppBtn(); // !await
            
            EditorGUILayout.EndVertical(); // End of foldout box skin
            EditorGUILayout.Space(20);
            EditorGUI.indentLevel--;
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
            EditorGUILayout.Space(10);
            
            // TODO
            
            EditorGUILayout.EndVertical(); // End of foldout box skin
            EditorGUILayout.Space(20);
            EditorGUI.indentLevel--;
        }

        private void insertDeployAppHelpbox()
        {
            // TODO: Validate that the correct fields are filled before allowing a button click
            const MessageType helpMsgType = MessageType.Info;
            const string helpMsg = "This action will create a new deployment version of your application. " +
                "New rooms will be created with this version of your server.";

            // Post the help box *before* we disable the button so it's easier to see (if toggleable)
            EditorGUILayout.HelpBox(helpMsg, helpMsgType);
        }

        private async Task insertDeployAppBtn()
        {
            bool clickedDeployBtn = insertLeftGeneralBtn("Deploy Application");
            if (!clickedDeployBtn)
                return;
            
            Deployment deployment = await HathoraServerDeploy.DeployToHathoraAsync(Config);
            Assert.That(deployment?.BuildId, Is.Not.Null,
                "Deployment failed: Check console for details.");
        }

        private void insertDevTokenPasswordField()
        {
            GUILayout.BeginHorizontal();

            InsertLeftLabel(_labelStr: "Developer Token",
                _tooltip: "Developer Token is used to authenticate with Hathora Cloud SDK");
            
            // USER INPUT >>
            string newDevAuthToken = EditorGUILayout.PasswordField(
                Config.HathoraCoreOpts.DevAuthOpts.DevAuthToken,
                options: null);

            if (newDevAuthToken != Config.HathoraCoreOpts.DevAuthOpts.DevAuthToken)
            {
                Config.HathoraCoreOpts.DevAuthOpts.DevAuthToken = newDevAuthToken;
                SerializedConfig.ApplyModifiedProperties();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);
        }

        private void insertTargetAppLabelWithTooltip()
        {
            InsertLeftLabel(_labelStr: "Target Application",
                _tooltip: "Defines which app to use for this project. " +
                "Create a new one in the Hathora console.");
        }
        #endregion // UI Draw

        
        #region Event Logic
        private async Task onRefreshAppsListBtnClick()
        {
            isRefreshingExistingApps = true;
            HathoraServerAppApi appApi = new(Config); 
            
            List<ApplicationWithDeployment> apps = await appApi.GetAppsAsync();

            try
            {
                // The wrappers go through a great deal of parsing
                Config.HathoraCoreOpts.ExistingAppsWithDeployment = apps; // Cache the response to Config
            }
            catch (Exception e)
            {
                Debug.LogError("Error setting " +
                    $"{nameof(Config.HathoraCoreOpts.ExistingAppsWithDeployment)}: {e}");
                throw;
            }

            List<ApplicationWithDeployment> DELETEME = null; 
            try
            {
                DELETEME = Config.HathoraCoreOpts.ExistingAppsWithDeployment; // TEST
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }
              
            // If selected app is -1 and apps count is > 0, select the first app
            bool hasSelectedApp = Config.HathoraCoreOpts.ExistingAppsSelectedIndex != -1;
            if (!hasSelectedApp && apps.Count > 0)
                setSelectedApp(_newSelectedIndex: 0); 
            
            isRefreshingExistingApps = false;
        }
        
        /// <summary>(!) Despite its name, a Popup() is actually a dropdown list</summary>
        private void onSelectedPopupAppChanged(int _newSelectedIndex)
        {
            setSelectedApp(_newSelectedIndex);
        }
        
        private void onServerBuildDirChanged(string _inputStr)
        {
            Config.LinuxHathoraAutoBuildOpts.ServerBuildDirName = _inputStr;
            SaveConfigChange(
                nameof(Config.LinuxHathoraAutoBuildOpts.ServerBuildDirName), 
                _inputStr);
        }        
        
        private void onServerBuildExeNameChanged(string _inputStr)
        {
            Config.LinuxHathoraAutoBuildOpts.ServerBuildExeName = _inputStr;
            
            SaveConfigChange(
                nameof(Config.LinuxHathoraAutoBuildOpts.ServerBuildExeName), 
                _inputStr);
        }
        #endregion // Event Logic

        #region Utils
        /// <summary>Sets AppId + ExistingAppsSelectedIndex</summary>
        private void setSelectedApp(int _newSelectedIndex)
        {
            Config.HathoraCoreOpts.AppId = Config.HathoraCoreOpts.ExistingAppsWithDeployment?[_newSelectedIndex]?.AppId;
            Config.HathoraCoreOpts.ExistingAppsSelectedIndex = _newSelectedIndex;

            SaveConfigChange(
                nameof(Config.HathoraCoreOpts.ExistingAppsSelectedIndex), 
                _newSelectedIndex.ToString());
        }
        
        /// <summary>The default vert spacing between fields is cramped</summary>
        private void insertFieldVertSpace() =>
            EditorGUILayout.Space(5f);
        #endregion // Utils
    }
}

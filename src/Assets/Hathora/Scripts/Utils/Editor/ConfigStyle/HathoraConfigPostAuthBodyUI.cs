// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
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

            InsertSpace4x();
            insertFoldouts();
        }

        private void insertBodyHeader()
        {
            insertLoginTokenGroup();
            
            // InsertHorizontalLine(1f, Color.gray, _space: 15);
            InsertSpace3x(); // Space looks better than line w/group boxes
            
            insertAppIdGroup();
        }

        private void insertLoginTokenGroup()
        {
            insertDevTokenPasswordField();
            insertLoginToHathoraConsoleBtn(); // !await
            
            InsertSpace2x();
        }

        private void insertFoldouts()
        {
            insertServerBuildSettingsFoldout();

            InsertSpace1x();
            insertDeploymentSettingsFoldout();
            
            InsertSpace1x();
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
            GUILayout.Label("<b>AppId:</b>", LeftAlignNoWrapLabelStyle, GUILayout.ExpandWidth(false));

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
                onSelectedAppPopupIndexChanged(newSelectedIndex);
            
            InsertSpace2x();
        }

        private async Task insertLoginToHathoraConsoleBtn()
        {
            EditorGUI.BeginDisabledGroup(disabled: false); 
            
            if (GUILayout.Button($"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>[Logged In]</color> " +
                    $"Log in with another account", GeneralButtonStyle))
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
            InsertSpace2x();
            
            insertBuildDirNameHorizGroup();
            insertBuildFileExeNameHorizGroup();

            InsertSpace2x();
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

            InsertSpace1x();
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
            
            InsertSpace1x();
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
    
            InsertSpace2x();
            
            insertPlanSizeHorizPopupList();
            insertRoomsPerProcessHorizSliderGroup();
            insertContainerPortNumberHorizSliderGroup();
            insertTransportTypeHorizRadioBtnGroup();
            insertDeployAppHelpbox(); // indentLevel is buggy, here: Keep it above
            insertDeployAppBtn(); // !await
            
            EditorGUILayout.EndVertical(); // End of foldout box skin
            InsertSpace3x();
        }

        private void insertRoomsPerProcessHorizSliderGroup()
        {
            int inputInt = base.insertHorizLabeledConstrainedIntField(
                _labelStr: "Rooms per process",
                _tooltip: null, // "Default: 1",
                _val: Config.HathoraDeployOpts.RoomsPerProcess,
                _minVal: 1,
                _maxVal: 10000,
                _alignPopup: GuiAlign.SmallRight);

            bool isChanged = inputInt != Config.HathoraDeployOpts.RoomsPerProcess;
            if (isChanged)
                onRoomsPerProcessSliderNumChanged(inputInt);
            
            InsertSpace1x();
        }
        
        private void insertContainerPortNumberHorizSliderGroup()
        {
            int inputInt = base.insertHorizLabeledConstrainedIntField(
                _labelStr: "Container port number",
                _tooltip: "Default: 7777 (<1024 is generally reserved by system)",
                _val: Config.HathoraDeployOpts.ContainerPortWrapper.PortNumber,
                _minVal: 1024,
                _maxVal: 49151,
                _alignPopup: GuiAlign.SmallRight);

            bool isChanged = inputInt != Config.HathoraDeployOpts.ContainerPortWrapper.PortNumber;
            if (isChanged)
                onContainerPortNumberSliderNumChanged(inputInt);
            
            InsertSpace1x();
        }
        
        private void insertTransportTypeHorizRadioBtnGroup()
        {
            int selectedIndex = Config.HathoraDeployOpts.TransportTypeSelectedIndex;
            
            // Get list of string names from PlanName Enum members. Set UPPER.
            List<string> displayOptsStrList = GetStrListOfEnumMemberKeys<TransportType>(
                EnumListOpts.AllCaps);

            int newSelectedIndex = base.insertHorizLabeledPopupList(
                _labelStr: "Transport Type",
                _tooltip: "Default: `UDP` (Fastest; although less reliable)",
                _displayOptsStrArr: displayOptsStrList.ToArray(),
                _selectedIndex: selectedIndex,
                GuiAlign.SmallRight);

            bool isNewValidIndex = selectedIndex >= 0 &&
                newSelectedIndex != selectedIndex &&
                selectedIndex < displayOptsStrList.Count;

            if (isNewValidIndex)
                onSelectedTransportTypeRadioBtnIndexChanged(newSelectedIndex);
            
            InsertSpace2x();
        }

        private static string getPlanNameListWithExtraInfo(PlanName _planName)
        {
            switch (_planName)
            {
                default:
                case PlanName.Tiny:
                    return $"{nameof(PlanName.Tiny)} (Shared core, 1GB)";
                
                case PlanName.Small:
                    return $"{nameof(PlanName.Small)} (1 core, 2GB)";
                
                case PlanName.Medium:
                    return $"{nameof(PlanName.Medium)} (2 cores, 4GB)";
                
                case PlanName.Large:
                    return $"{nameof(PlanName.Large)} (4 cores, 8GB)"; 
            }
        }

        private void insertPlanSizeHorizPopupList()
        {
            int selectedIndex = Config.HathoraDeployOpts.PlanSizeSelectedIndex;
            
            // Get list of string names from PlanName Enum members - with extra info
            List<string> displayOptsStrArr = Enum
                .GetValues(typeof(PlanName))
                .Cast<PlanName>()
                .Select(getPlanNameListWithExtraInfo)
                .ToList();

            int newSelectedIndex = base.insertHorizLabeledPopupList(
                _labelStr: "Plan Size",
                _tooltip: "Default: `Tiny` (Most affordable for pre-production)",
                _displayOptsStrArr: displayOptsStrArr.ToArray(),
                _selectedIndex: selectedIndex,
                GuiAlign.SmallRight);

            bool isNewValidIndex = selectedIndex >= 0 &&
                newSelectedIndex != selectedIndex &&
                selectedIndex < displayOptsStrArr.Count;

            if (isNewValidIndex)
                onSelectedPlanSizePopupIndexChanged(newSelectedIndex);
            
            InsertSpace2x();
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
            InsertSpace2x();
            
            // TODO
            
            EditorGUILayout.EndVertical(); // End of foldout box skin
            InsertSpace3x();
            EditorGUI.indentLevel--;
        }

        private void insertDeployAppHelpbox()
        {
            InsertSpace2x();
            
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
            InsertSpace2x();
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
              
            // If selected app is -1 and apps count is > 0, select the first app
            bool hasSelectedApp = Config.HathoraCoreOpts.ExistingAppsSelectedIndex != -1;
            if (!hasSelectedApp && apps.Count > 0)
                setSelectedApp(_newSelectedIndex: 0); 
            
            isRefreshingExistingApps = false;
        }
        
        /// <summary>(!) Despite its name, a Popup() is actually a dropdown list</summary>
        private void onSelectedAppPopupIndexChanged(int _newSelectedIndex)
        {
            // There may be more than 1 way to set this, so we curry to a common func
            setSelectedApp(_newSelectedIndex);
        }
        
        private void onSelectedPlanSizePopupIndexChanged(int _newSelectedIndex)
        {
            Config.HathoraDeployOpts.PlanSizeSelectedIndex = _newSelectedIndex;
            SaveConfigChange(
                nameof(Config.HathoraDeployOpts.PlanSizeSelectedIndex), 
                _newSelectedIndex.ToString());
        }
        
        
        private void onRoomsPerProcessSliderNumChanged(int _inputInt)
        {
            Config.HathoraDeployOpts.RoomsPerProcess = _inputInt;
            SaveConfigChange(
                nameof(Config.HathoraDeployOpts.RoomsPerProcess), 
                _inputInt.ToString());
        }
        
        private void onContainerPortNumberSliderNumChanged(int _inputInt)
        {
            Config.HathoraDeployOpts.ContainerPortWrapper.PortNumber = _inputInt;
            SaveConfigChange(
                nameof(Config.HathoraDeployOpts.ContainerPortWrapper.PortNumber), 
                _inputInt.ToString());
        }
        
        private void onSelectedTransportTypeRadioBtnIndexChanged(int _newSelectedIndex)
        {
            Config.HathoraDeployOpts.TransportTypeSelectedIndex = _newSelectedIndex;
            SaveConfigChange(
                nameof(Config.HathoraDeployOpts.TransportTypeSelectedIndex), 
                _newSelectedIndex.ToString());
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
        #endregion // Utils
    }
}

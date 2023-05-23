// Created by dylan@hathora.dev

using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using Hathora.Scripts.SdkWrapper.Editor.ApiWrapper;
using UnityEditor;
using UnityEngine;
using Application = Hathora.Cloud.Sdk.Model.Application;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPostAuthBodyUI : HathoraConfigUIBase
    {
        #region Vars
        private bool devReAuthLoginButtonInteractable;
        private bool isRefreshingExistingApps;
        
        private bool isServerBuildFoldout;
        private bool isDeploymentFoldout;
        private bool isCreateRoomLobbyFoldout;
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

            insertBodyHeader();
            insertFoldouts();
        }

        private void insertBodyHeader()
        {
            insertDevTokenPasswordField();
            insertLoginToHathoraConsoleBtn(); // !await

            InsertHorizontalLine(1.5f, Color.gray, _space: 15);
            
            insertAppIdGroup();
            
            EditorGUILayout.Space(20f);
        }
        
        private void insertFoldouts()
        {
            insertServerBuildSettingsFoldout();
            insertDeploymentSettingsFoldout();
            insertCreateRoomOrLobbyFoldout();
        }
        
        private void insertAppIdGroup()
        {
            insertAppIdHorizHeader();
            insertAppsListHorizGroup();
            insertAppIdDisplayCopyGroup();
        }
        
        private void insertAppIdHorizHeader()
        {
            GUILayout.BeginHorizontal();
            
            insertTargetAppLabelWithTooltip();
            insertSelectAppToUseOpacityLabel();
            
            GUILayout.EndHorizontal();
        }
        
        private void insertAppsListHorizGroup()
        {
            EditorGUI.BeginDisabledGroup(disabled: isRefreshingExistingApps);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            
            insertExistingAppsPopup(); // This actually drops down, despite the name
            insertExistingAppsRefreshBtn(); // !await
            
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        }
        
        private void insertAppIdDisplayCopyGroup()
        {
            if (!CheckHasSelectedApp())
                return;
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>AppId:</b>", LeftAlignLabelStyle, GUILayout.ExpandWidth(false));

            string selectedAppId = Config.HathoraCoreOpts.AppId;
            base.insertLeftSelectableLabel(selectedAppId);
            
            // USER INPUT >>
            bool clickedCopyAppIdBtn = insertLeftGeneralBtn("Copy AppId");
            if (clickedCopyAppIdBtn)
                onCopyAppIdBtnClick(selectedAppId);

            GUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);
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

        private void insertExistingAppsPopup()
        {
            List<string> displayedOptionsList = Config.HathoraCoreOpts.GetExistingAppNames();
            string[] displayedOptionsArr = displayedOptionsList?.ToArray();
            
            int selectedIndex = Config.HathoraCoreOpts.ExistingAppsSelectedIndex;
            
            // USER INPUT >>
            int newSelectedIndex = EditorGUILayout.Popup(
                selectedIndex, 
                displayedOptionsArr);

            bool isNewValidIndex = displayedOptionsList != null &&
                selectedIndex >= 0 &&
                newSelectedIndex != selectedIndex &&
                selectedIndex < displayedOptionsList.Count;

            if (isNewValidIndex)
                onSelectedPopupAppChanged(newSelectedIndex);

            EditorGUILayout.Space(10);
        }

        private async Task insertLoginToHathoraConsoleBtn()
        {
            EditorGUI.BeginDisabledGroup(disabled: false); 
            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            if (GUILayout.Button("Log in with another account", GeneralButtonStyle))
            {
                devReAuthLoginButtonInteractable = false;
                await HathoraServerAuth.DevAuthLogin(Config);
                devReAuthLoginButtonInteractable = true; 
                InvokeRequestRepaint();
            }
            
            EditorGUI.EndDisabledGroup();

            if (HathoraServerAuth.HasCancellableToken && !devReAuthLoginButtonInteractable)
            {
                insertAuthCancelBtn(HathoraServerAuth.ActiveCts);
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
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
            isServerBuildFoldout = EditorGUILayout.Foldout(
                isServerBuildFoldout, 
                "Server Build Settings");
            
            if (isServerBuildFoldout)
            {
                // TODO 
            }
        }
        
        private void insertDeploymentSettingsFoldout()
        {
            isDeploymentFoldout = EditorGUILayout.Foldout(
                isDeploymentFoldout, 
                "Hathora Deployment Configuration");
            
            if (isDeploymentFoldout)
            {
                // TODO 
            }
        }
        
        private void insertCreateRoomOrLobbyFoldout()
        {
            isCreateRoomLobbyFoldout = EditorGUILayout.Foldout(
                isCreateRoomLobbyFoldout, 
                "Create Room or Lobby");
            
            if (isCreateRoomLobbyFoldout)
            {
                // TODO 
            }
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
            
            Config.HathoraCoreOpts.ExistingApps = apps; // Cache the response to Config
            
            // If selected app is -1 and apps count is > 0, select the first app
            bool hasSelectedApp = Config.HathoraCoreOpts.ExistingAppsSelectedIndex != -1;
            if (!hasSelectedApp && apps.Count > 0)
                setSelectedApp(_newSelectedIndex: 0);
            
            isRefreshingExistingApps = false;
        }
        
        private void onSelectedPopupAppChanged(int _newSelectedIndex)
        {
            setSelectedApp(_newSelectedIndex);
        }
        #endregion // Event Logic
        
        
        #region Utils
        /// <summary>Sets AppId + ExistingAppsSelectedIndex</summary>
        private void setSelectedApp(int _newSelectedIndex)
        {
            Config.HathoraCoreOpts.AppId = Config.HathoraCoreOpts.ExistingApps?[_newSelectedIndex]?.AppId;
            Config.HathoraCoreOpts.ExistingAppsSelectedIndex = _newSelectedIndex;
            
            Debug.Log($"[{nameof(HathoraConfigPostAuthBodyUI)}] Set new " +
                $"{nameof(Config.HathoraCoreOpts.ExistingAppsSelectedIndex)}=={_newSelectedIndex}");
            
            SerializedConfig.ApplyModifiedProperties();
        }
        #endregion // Utils
    }
}

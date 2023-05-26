// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using Hathora.Scripts.SdkWrapper.Editor.ApiWrapper;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle.PostAuth
{
    public class HathoraConfigPostAuthBodyHeaderUI : HathoraConfigUIBase
    {
        #region Vars
        private bool devReAuthLoginButtonInteractable;
        private bool isRefreshingExistingApps;
        #endregion // Vars


        #region Init
        public HathoraConfigPostAuthBodyHeaderUI(
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

        private void insertExistingAppsRefreshBtn()
        {
            // USER INPUT >>
            bool clickedAppRefreshBtn = insertLeftGeneralBtn("↻ Refresh List"); 
            if (clickedAppRefreshBtn)
                onRefreshAppsListBtnClick();
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

// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using Hathora.Scripts.SdkWrapper.Models;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPostAuthBodyUI : HathoraConfigUIBase
    {
        private readonly SerializedProperty devAuthTokenProp;
        private readonly SerializedProperty appIdProp;
        
        private bool devReAuthLoginButtonInteractable;


        #region Init
        public HathoraConfigPostAuthBodyUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
            : base(_config, _serializedConfig)
        {
            if (!HathoraConfigUI.ENABLE_BODY_STYLE)
                return;
            
            devAuthTokenProp = FindNestedProperty(SerializedConfig, getDevAuthTokenPath()); 
            Assert.IsNotNull(devAuthTokenProp, "!SerializedProperty for " +
                $"{nameof(devAuthTokenProp)}: '{getDevAuthTokenPath()}'");
            
            appIdProp = FindNestedProperty(SerializedConfig, getAppIdTokenPath());
            Assert.IsNotNull(appIdProp, "!SerializedProperty for " +
                $"{nameof(appIdProp)}: '{getAppIdTokenPath()}'");
        }

        private static string[] getDevAuthTokenPath() => new[]
        {
            NetHathoraConfig.SerializedFieldNames.HathoraCoreOpts,
            HathoraCoreOpts.SerializedFieldNames.DevAuthOpts,
            HathoraDevAuthTokenOpts.SerializedFieldNames.DevAuthToken,
        };
        
        private static string[] getAppIdTokenPath() => new[]
        {
            NetHathoraConfig.SerializedFieldNames.HathoraCoreOpts,
            HathoraCoreOpts.SerializedFieldNames.AppId,
        };
        #endregion // Init
        
        
        #region Main
        public void Draw()
        {
            if (!IsAuthed)
                return; // You should be calling HathoraConfigPreAuthBodyUI.Draw()

            insertBodyHeader();
            insertServerBuildSettingsDropdown();
            insertDeploymentSettingsDropdown();
        }

        private void insertBodyHeader()
        {
            insertDevTokenPasswordField();
            insertLoginToHathoraConsoleBtn(); // !await
            insertAppIdField();
            insertExistingAppsDropdown();
        }
        
        private void insertExistingAppsDropdown()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>" +
                "(enter appId above - or select app below)</color>", CenterAlignLabelStyle);

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
            {
                selectedIndex = newSelectedIndex;
                Config.HathoraCoreOpts.ExistingAppsSelectedIndex = selectedIndex;
                SerializedConfig.ApplyModifiedProperties();
                
                Debug.Log($"[{nameof(HathoraConfigPostAuthBodyUI)}] Set new " +
                    $"{nameof(Config.HathoraCoreOpts.ExistingAppsSelectedIndex)}=={selectedIndex}");
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        private async Task insertLoginToHathoraConsoleBtn()
        {
            EditorGUI.BeginDisabledGroup(disabled: false); 

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>" +
                "(enter token above - or log in below)</color>", CenterAlignLabelStyle);
            
            if (GUILayout.Button("Log in to Hathora Console", GeneralButtonStyle))
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

        private void insertServerBuildSettingsDropdown()
        {
            
        }
        
        private void insertDeploymentSettingsDropdown()
        {
            
        }
        #endregion // Main

        
        private void insertDevTokenPasswordField()
        {
            GUILayout.BeginHorizontal();

            InsertLeftLabel(labelStr: "Developer Token",
                tooltip: "Developer Token is used to authenticate with Hathora Cloud SDK");

            // USER INPUT >>
            string newPassword = EditorGUILayout.PasswordField(
                devAuthTokenProp.stringValue,
                options: null);

            if (newPassword != devAuthTokenProp.stringValue)
            {
                devAuthTokenProp.stringValue = newPassword;
                SerializedConfig.ApplyModifiedProperties();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);
        }
        
        private void insertAppIdField()
        {
            GUILayout.BeginHorizontal();

            InsertLeftLabel(labelStr: "Application ID",
                tooltip: "Defines which app to use for this project. " +
                    "Create a new one in the Hathora console.");

            // USER INPUT >>
            string newAppId = EditorGUILayout.TextField(
                appIdProp.stringValue,
                options: null);

            if (newAppId != appIdProp.stringValue)
            {
                appIdProp.stringValue = newAppId;
                SerializedConfig.ApplyModifiedProperties();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);
        }
    }
}

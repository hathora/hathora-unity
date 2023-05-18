// Created by dylan@hathora.dev

using System.Threading;
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
            Assert.IsNotNull(devAuthTokenProp, "Could not find SerializedProperty for DevAuthToken");
            
            appIdProp = FindNestedProperty(SerializedConfig, getAppIdTokenPath());
            Assert.IsNotNull(appIdProp, "Could not find SerializedProperty for AppId");
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
        }

        private async Task insertLoginToHathoraConsoleBtn()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>" +
                "(enter or log in)</color>", CenterAlignLabelStyle);
            
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

            string newAppId = EditorGUILayout.TextField(
                appIdProp.stringValue,
                options: null);

            if (newAppId != appIdProp.stringValue)
            {
                appIdProp.stringValue = newAppId;
                SerializedConfig.ApplyModifiedProperties();
            }

            GUILayout.EndHorizontal();
        }
    }
}

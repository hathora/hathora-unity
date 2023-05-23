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

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPostAuthBodyUI : HathoraConfigUIBase
    {
        private bool devReAuthLoginButtonInteractable;
        private bool isRefreshingExistingApps;
        
        private bool isServerBuildFoldout;
        private bool isDeploymentFoldout;
        private bool isCreateRoomLobbyFoldout;


        #region Init
        public HathoraConfigPostAuthBodyUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
            : base(_config, _serializedConfig)
        {
            if (!HathoraConfigUI.ENABLE_BODY_STYLE)
                return;
            
            Debug.Log("[HathoraConfigPostAuthBodyUI] @ Constructor");
        }
        #endregion // Init
        
        
        #region Main
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
            insertAppIdCombo();
        }
        
        private void insertFoldouts()
        {
            insertServerBuildSettingsFoldout();
            insertDeploymentSettingsFoldout();
            insertCreateRoomOrLobbyFoldout();
        }
        #endregion // Main
        
        
        private void insertAppIdCombo()
        {
            EditorGUI.BeginDisabledGroup(disabled: isRefreshingExistingApps);
            insertAppIdField();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            
            insertExistingAppsPopup(); // This actually drops down, despite the name
            insertExistingAppsRefreshBtn(); // !await
            
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        }

        private async Task insertExistingAppsRefreshBtn()
        {
            if (GUILayout.Button("↻ Refresh List", GeneralButtonStyle))
            {
                isRefreshingExistingApps = true;

                HathoraServerAppApi appApi = new(Config); 
                List<ApplicationWithDeployment> apps = await appApi.GetAppsAsync();
                Config.HathoraCoreOpts.ExistingApps = apps; // Cache the response to Config
                
                isRefreshingExistingApps = false;
            }
        }

        private void insertExistingAppsPopup()
        {
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

            InsertLeftLabel(labelStr: "Developer Token",
                tooltip: "Developer Token is used to authenticate with Hathora Cloud SDK");
            
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
        
        private void insertAppIdField()
        {
            GUILayout.BeginHorizontal();

            InsertLeftLabel(labelStr: "Application ID",
                tooltip: "Defines which app to use for this project. " +
                    "Create a new one in the Hathora console.");

            // USER INPUT >>
            string newAppId = EditorGUILayout.TextField(
                Config.HathoraCoreOpts.AppId,
                options: null);

            if (newAppId != Config.HathoraCoreOpts.AppId)
            {
                Config.HathoraCoreOpts.AppId = newAppId;
                SerializedConfig.ApplyModifiedProperties();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);
        }
    }
}

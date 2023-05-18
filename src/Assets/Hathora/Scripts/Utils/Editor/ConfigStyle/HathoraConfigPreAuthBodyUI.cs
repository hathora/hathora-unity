// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPreAuthBodyUI : HathoraConfigUIBase
    {
        // Declare an event to trigger a repaint
        private static bool devAuthLoginButtonInteractable = true;
        
        
        public HathoraConfigPreAuthBodyUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig) 
            : base(_config, _serializedConfig)
        {
            if (!HathoraConfigUI.ENABLE_BODY_STYLE)
                return;
        }
        
        public void Draw()
        {
            if (IsAuthed)
                return; // You should be calling HathoraConfigPostAuthBodyUI.Draw()
            
            insertAuthBtns(); // Show only auth button, if !authed
            InvokeRequestRepaint();
        }
        
        private void insertAuthBtns()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>" +
                "Create an account or log in to Hathora Cloud's Console to get started</color>", 
                CenterAlignLabelStyle);
            EditorGUILayout.Space(10f);

            insertDevAuthLoginBtn(Config); // !await
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        private async Task insertDevAuthLoginBtn(NetHathoraConfig selectedConfig)
        {
            bool hasAuthToken = !string.IsNullOrEmpty(selectedConfig.HathoraCoreOpts.DevAuthOpts.DevAuthToken);
            bool pendingGuiEnable = devAuthLoginButtonInteractable && !hasAuthToken;

            string btnStr = pendingGuiEnable
                ? "Register or log in to Hathora Console"
                : "<color=yellow>Awaiting browser login...</color>";

            EditorGUI.BeginDisabledGroup(!devAuthLoginButtonInteractable);
            
            if (GUILayout.Button(btnStr, BigButtonStyle))
            {
                devAuthLoginButtonInteractable = false;
                await HathoraServerAuth.DevAuthLogin(selectedConfig);
                devAuthLoginButtonInteractable = true; 
                InvokeRequestRepaint();
            }
            
            EditorGUI.EndDisabledGroup(); 

            if (HathoraServerAuth.HasCancellableToken && !devAuthLoginButtonInteractable)
            {
                insertAuthCancelBtn(HathoraServerAuth.ActiveCts);
            }
        }
        
        private void insertAuthCancelBtn(CancellationTokenSource _cts) 
        {
            if (GUILayout.Button("Cancel", GeneralButtonStyle))
            {
                _cts?.Cancel();
                devAuthLoginButtonInteractable = true;
            }
            
            InvokeRequestRepaint();
        }
    }
}

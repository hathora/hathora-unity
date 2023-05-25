// Created by dylan@hathora.dev

using System.Threading;
using System.Threading.Tasks;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using Hathora.Scripts.SdkWrapper.Editor.Auth0;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigPreAuthBodyUI : HathoraConfigUIBase
    {
        // Declare an event to trigger a repaint
        private static bool devAuthLoginButtonInteractable = true;

        private bool checkedTokenCache;
        private string cachedToken;
        private bool CheckHasCachedToken() => 
            !string.IsNullOrEmpty(cachedToken);
        
        
        public HathoraConfigPreAuthBodyUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig) 
            : base(_config, _serializedConfig)
        {
            // if (!HathoraConfigUI.ENABLE_BODY_STYLE)
            //     return;
        }
        
        
        #region UI Draw
        public void Draw()
        {
            if (IsAuthed)
                return; // You should be calling HathoraConfigPostAuthBodyUI.Draw()

            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            insertAuthHeaderLbl();
            insertRegAuthBtns();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
            
            InvokeRequestRepaint();
        }

        private void insertAuthHeaderLbl()
        {
            const string labelStr = "Create an account or log in to Hathora Cloud's Console to get started";
            GUILayout.Label(labelStr, 
                CenterAlignLabelStyle);
            
            EditorGUILayout.Space(10f);        
        }
        
        /// <summary>
        /// These essentially do the same thing
        /// </summary>
        private void insertRegAuthBtns()
        {
            EditorGUI.BeginDisabledGroup(!devAuthLoginButtonInteractable);
            
            // !await these
            insertDevAuthLoginBtn();
            InsertMoreActionsLbl();
            insertRegisterOrTokenCacheLogin();

            EditorGUI.EndDisabledGroup(); 

            if (HathoraServerAuth.HasCancellableToken && !devAuthLoginButtonInteractable)
                insertAuthCancelBtn(HathoraServerAuth.ActiveCts);
        }

        private void insertRegisterOrTokenCacheLogin()
        {
            tokenCheckSetCache();
            
            if (CheckHasCachedToken())
                insertTokenCacheBtn();
            else
                insertRegisterLinkLbl();
        }

        private void InsertMoreActionsLbl()
        {
            InsertCenterLabel("<b>- or -</b>");
            EditorGUILayout.Space(5f);
        }

        /// <summary>
        /// If we didn't check it before and exists, insert the button
        /// </summary>
        private void tokenCheckSetCache()
        {
            if (!checkedTokenCache)
            {
                cachedToken = Auth0Login.CheckForExistingCachedTokenAsync();
                checkedTokenCache = true;
            }
        }

        private void insertTokenCacheBtn()
        {
            StartCenterHorizAlign();

            string btnLabel = "Existing token cache found:\n" +
                $"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}><b>Log in with token</b></color>";
            
            // USER INPUT >>
            // if (GUILayout.Button(btnLabel, CenterAlignLabelStyle))
            bool clickedLabelLink = InsertLinkLabelEvent(btnLabel, _centerAlign: true);
            
            if (clickedLabelLink)
                onInsertTokenCacheBtnClick(cachedToken);
                
            EndCenterHorizAlign();
            EditorGUILayout.Space(10f);
        }

        private async Task insertRegisterLinkLbl()
        {
            StartCenterHorizAlign();

            string btnLabel = $"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>" +
                $"<b>Register Now</b></color>";
        
            // USER INPUT >>
            // if (GUILayout.Button(btnLabel, CenterAlignLabelStyle))
            bool clickedLabelLink = InsertLinkLabelEvent(btnLabel, _centerAlign: true);

            if (clickedLabelLink)
                onLoginBtnClick();
            
            EndCenterHorizAlign();
        }

        private async Task insertDevAuthLoginBtn()
        {
            // USER INPUT >>
            if (GUILayout.Button("Log in to Hathora Cloud", BigButtonStyle))
                await onLoginBtnClick();
            
            EditorGUILayout.Space(10f);
        }
        
        private void insertAuthCancelBtn(
            CancellationTokenSource _cts, 
            string _cancelLabelStr = "Cancel") 
        {
            // USER INPUT >>
            if (GUILayout.Button(_cancelLabelStr, GeneralButtonStyle))
                onCancelBtnClick(_cts);
            
            EditorGUILayout.Space(10f);
            InvokeRequestRepaint();
        }
        
        
        private void onInsertTokenCacheBtnClick(string _cachedAuthToken)
        {
            HathoraServerAuth.SetAuthToken(Config, _cachedAuthToken);
        }
        #endregion // UI Draw

        
        #region Logic Events
        private async Task onLoginBtnClick()
        {
            devAuthLoginButtonInteractable = false;
                
            await HathoraServerAuth.DevAuthLogin(Config);
                
            devAuthLoginButtonInteractable = true; 
            InvokeRequestRepaint();
        }
        
        private void onCancelBtnClick(CancellationTokenSource _cts)
        {
            _cts?.Cancel();
            devAuthLoginButtonInteractable = true;
        }
        #endregion // Logic Events


        #region Utils
        /// <summary>The auth button should be disabled while authenticating.</summary>
        private bool checkHasActiveAuthPolling() =>
            devAuthLoginButtonInteractable && !CheckHasAuthToken();
        #endregion // Utils
    }
}

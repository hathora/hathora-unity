// Created by dylan@hathora.dev

using System;
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

        /// <summary>Checking a file for a cached token repeatedly would cause lag</summary>
        public static bool checkedTokenCache { get; set; }
        
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
            InsertSpace2x();
            
            InvokeRequestRepaint();
        }

        private void insertAuthHeaderLbl()
        {
            const string labelStr = "Create an account or log in to Hathora Cloud's Console to get started";
            GUILayout.Label(labelStr, 
                CenterAlignLabelStyle);
            
            InsertSpace2x();        
        }
        
        /// <summary>
        /// These essentially do the same thing
        /// </summary>
        private void insertRegAuthBtns()
        {
            bool showCancelBtn = !HathoraServerAuth.IsAuthComplete && !devAuthLoginButtonInteractable;
            
            // !await these
            if (showCancelBtn)
            {
                insertAuthCancelBtn(HathoraServerAuth.AuthCancelTokenSrc);
                return;
            }
            
            insertDevAuthLoginBtn(); // !await
            InsertMoreActionsLbl();
            insertRegisterOrTokenCacheLogin();
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
            InsertSpace1x();
        }

        /// <summary>
        /// If we didn't check it before and exists, insert the button
        /// </summary>
        private void tokenCheckSetCache()
        {
            if (checkedTokenCache)
                return;
            
            cachedToken = Auth0Login.CheckForExistingCachedTokenAsync();
            checkedTokenCache = true;
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
            InsertSpace2x();
        }

        private async Task insertRegisterLinkLbl()
        {
            InsertCenterLabel("Don't have an account yet?");
        
            // // USER INPUT >>
            // bool clickedLink = InsertLinkLabelEvent(
            //     "Register Here",
            //     _centerAlign: true);
            
            // USER INPUT >> Opens bad url, catch Exception, trigger our own event.
            InsertLinkLabel(
                "Register Here",
                _url: null,
                _centerAlign: true,
                onClick: () =>
                {
                    onRegisterBtnClick(); // !await
                });  
        }

        private async Task insertDevAuthLoginBtn()
        {
            // USER INPUT >>
            bool clickedLoginBtn = GUILayout.Button("Log in to Hathora Cloud", BigButtonSideMarginsStyle);
            if (clickedLoginBtn)
                await onLoginBtnClickAsync();
            
            InsertSpace2x();
        }
        
        private void insertAuthCancelBtn(CancellationTokenSource _cancelTokenSrc) 
        {
            string btnLabelStr = $"<color={HathoraEditorUtils.HATHORA_PINK_CANCEL_COLOR_HEX}>" +
                "<b>Cancel</b> (Logging in via browser...)</color>";
            
            // USER INPUT >>
            bool clickedAuthCancelBtn = GUILayout.Button(btnLabelStr, GeneralButtonStyle);
            if (clickedAuthCancelBtn)
                onAuthCancelBtnClick(_cancelTokenSrc);
            
            InsertSpace2x();
            InvokeRequestRepaint();
        }
        
        
        private void onInsertTokenCacheBtnClick(string _cachedAuthToken)
        {
            HathoraServerAuth.SetAuthToken(Config, _cachedAuthToken);
            onLoginSuccess();
        }
        #endregion // UI Draw

        
        #region Logic Events
        private void onRegisterBtnClick() => 
            onLoginBtnClickAsync(); // !await
        
        private async Task<bool> onLoginBtnClickAsync()
        {
            Debug.Log("[HathoraConfigPreAuthBodyUI] onLoginBtnClickAsync");
            devAuthLoginButtonInteractable = false;

            HathoraServerAuth.AuthCompleteSrc = new TaskCompletionSource<bool>();
            bool isSuccess = await HathoraServerAuth.DevAuthLogin(Config);
            if (!isSuccess)
                onLoginFail();
            else
                onLoginSuccess();
            
            devAuthLoginButtonInteractable = true;
            return isSuccess;
        }

        private void onLoginSuccess()
        {
            Debug.Log("[HathoraConfigPreAuthBodyUI] onLoginSuccess");
             
            // Set a flag to refresh apps automatically the next time we Draw post-auth body header
            if (!HathoraServerAuth.IsAuthComplete)
                HathoraServerAuth.AuthCompleteSrc?.SetResult(true); // isSuccess
            
            Config.HathoraCoreOpts.DevAuthOpts.RecentlyAuthed = true;
        }

        private void onLoginFail()
        {
            Debug.Log("[HathoraConfigPreAuthBodyUI] onLoginFail");
            
            if (!HathoraServerAuth.IsAuthComplete)
                HathoraServerAuth.AuthCompleteSrc?.SetResult(false); // !isSuccess
        }

        private void onAuthCancelBtnClick(CancellationTokenSource _cancelTokenSrc)
        {
            _cancelTokenSrc?.Cancel();
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

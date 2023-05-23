// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    /// <summary>
    /// The main editor for NetHathoraConfig, including all the button clicks and extra UI.
    /// </summary>
    [CustomEditor(typeof(NetHathoraConfig))]
    public class HathoraConfigUI : UnityEditor.Editor
    {
        /// <summary>Set false to view the "raw" ScriptableObject</summary>
        public const bool ENABLE_BODY_STYLE = true;
        
        #region Vars
        private HathoraConfigHeaderUI headerUI;
        private HathoraConfigPreAuthBodyUI preAuthBodyUI;
        private HathoraConfigPostAuthBodyUI postAuthAuthPostAuthBodyUI;
        private HathoraConfigFooterUI footerUI;
                
        private string previousDevAuthToken;
        private NetHathoraConfig selectedConfig;
        private SerializedObject serializedConfig;
        
        private bool IsAuthed => 
            selectedConfig.HathoraCoreOpts.DevAuthOpts.HasAuthToken;

        private NetHathoraConfig getSelectedInstance() =>
            (NetHathoraConfig)target;
        #endregion // Vars

        
        #region Main
        /// <summary>
        /// Essentially the editor version of Update().
        /// We'll mask over the entire Config with a styled UI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            checkForDirtyRefs();
            drawHeaderBodyFooter();
            applyChanges();
        }

        private void applyChanges()
        {
            // Apply the modified properties to the target object
            serializedObject.ApplyModifiedProperties();
        }

        private void drawHeaderBodyFooter()
        {
            headerUI.Draw();
            
            if (!ENABLE_BODY_STYLE)
                base.OnInspectorGUI(); // Show the raw config, auto-gen'd by ScriptableObj
            else if (IsAuthed)
                postAuthAuthPostAuthBodyUI.Draw();
            else
                preAuthBodyUI.Draw();
            
            footerUI.Draw();
        }

        private void checkForDirtyRefs()
        {
            bool lostRefs = headerUI == null 
                || preAuthBodyUI == null 
                || postAuthAuthPostAuthBodyUI == null
                || footerUI == null 
                || !ReferenceEquals(selectedConfig, getSelectedInstance());
            
            if (lostRefs)
                initDrawUtils();
            
            serializedConfig.Update();
        }

        private void initDrawUtils()
        {
            selectedConfig = getSelectedInstance();
            serializedConfig = new SerializedObject(selectedConfig);

            // New instances of util draw classes
            headerUI = new HathoraConfigHeaderUI(selectedConfig, serializedConfig);
            preAuthBodyUI = new HathoraConfigPreAuthBodyUI(selectedConfig, serializedConfig);
            
            postAuthAuthPostAuthBodyUI = new HathoraConfigPostAuthBodyUI(
                selectedConfig, 
                serializedConfig);
            
            footerUI = new HathoraConfigFooterUI(selectedConfig, serializedConfig);
            
            // Subscribe to repainting events
            headerUI.RequestRepaint += Repaint;
            preAuthBodyUI.RequestRepaint += Repaint;
            postAuthAuthPostAuthBodyUI.RequestRepaint += Repaint;
            footerUI.RequestRepaint += Repaint;
        }
        #endregion // Main


        #region Core Buttons
        // private void insertSplitButtons(NetHathoraConfig _config, bool _isAuthed)
        // {
        //     EditorGUILayout.Space(5);
        //
        //     if (!_isAuthed)
        //     {
        //
        //         return;
        //     }
        //
        //     // InsertHorizontalLine(1, Color.gray);
        //     EditorGUILayout.Space(10);
        //
        //     EditorGUILayout.BeginVertical(GUI.skin.box);
        //     GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>" +
        //         "Customize via `Linux Auto Build Opts`</color>", CenterAlignLabelStyle);
        //     insertBuildBtn(_config);
        //     EditorGUILayout.EndVertical();
        //
        //     EditorGUILayout.Space(10);
        //
        //     EditorGUILayout.BeginVertical(GUI.skin.box);
        //     GUILayout.Label($"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>" +
        //         "Customize via `Hathora Deploy Opts`</color>", CenterAlignLabelStyle);
        //     insertHathoraDeployBtn(_config);
        //     EditorGUILayout.EndVertical();
        // }

        // private static async Task insertHathoraDeployBtn(NetHathoraConfig selectedConfig)
        // {
        //     GUI.enabled = selectedConfig.MeetsDeployBtnReqs();;
        //
        //     if (GUILayout.Button("Deploy to Hathora", GeneralButtonStyle))
        //     {
        //         await HathoraServerDeploy.DeployToHathoraAsync(selectedConfig);
        //         EditorGUILayout.Space(20);
        //     }
        //     
        //     GUI.enabled = true;
        // }
        
        // private static void insertBuildBtn(NetHathoraConfig selectedConfig)
        // {
        //     GUI.enabled = selectedConfig.MeetsBuildBtnReqs();
        //     
        //     if (GUILayout.Button("Build Linux Server", GeneralButtonStyle))
        //     {
        //         HathoraServerBuild.BuildHathoraLinuxServer(selectedConfig);
        //     }
        //     
        //     GUI.enabled = true;
        // }
        #endregion // Core Buttons
    }
}

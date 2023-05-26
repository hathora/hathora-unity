// Created by dylan@hathora.dev

using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle.PostAuth
{
    public class HathoraConfigPostAuthBodyBuildUI : HathoraConfigUIBase
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
        public HathoraConfigPostAuthBodyBuildUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
            : base(_config, _serializedConfig)
        {
            if (!HathoraConfigUI.ENABLE_BODY_STYLE)
                return;
        }
        #endregion // Init
        
        
        #region UI Draw
        public void Draw()
        {
            if (!IsAuthed)
                return; // You should be calling HathoraConfigPreAuthBodyUI.Draw()

            insertServerBuildSettingsFoldout();
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
        #endregion // UI Draw

        
        #region Event Logic
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
    }
}

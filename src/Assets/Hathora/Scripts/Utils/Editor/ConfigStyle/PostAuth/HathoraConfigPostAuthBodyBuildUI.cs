// Created by dylan@hathora.dev

using System.Text;
using System.Threading.Tasks;
using Codice.CM.Common.Tree.Partial;
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
        // Foldouts
        private bool isServerBuildFoldout;
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
            isServerBuildFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(
                isServerBuildFoldout,
                "Server Build Settings");

            if (!isServerBuildFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            
            EditorGUI.indentLevel++;
            InsertSpace2x();
            
            insertBuildSettingsFoldoutComponents();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void insertBuildSettingsFoldoutComponents()
        {
            insertBuildDirNameHorizGroup();
            insertBuildFileExeNameHorizGroup();

            InsertSpace2x();
            bool enableBuildBtn = insertGenerateServerBuildBtnHelpboxOnMissingReqs();
            insertGenerateServerBuildBtn(); // !await
        }

        /// <summary>
        /// </summary>
        /// <returns>enableBuildBtn</returns>
        private bool insertGenerateServerBuildBtnHelpboxOnMissingReqs()
        {
            bool enableBuildBtn =
                Config.HathoraCoreOpts.HasAppId &&
                Config.LinuxHathoraAutoBuildOpts.HasServerBuildDirName &&
                Config.LinuxHathoraAutoBuildOpts.HasServerBuildExeName;

            if (enableBuildBtn)
                return true; // enableBuildBtn

            StringBuilder helpboxLabelStrb = new("Missing required fields: ");
            if (!Config.HathoraCoreOpts.HasAppId)
                helpboxLabelStrb.Append($"{nameof(Config.HathoraCoreOpts.AppId)}, ");
            
            if (!Config.LinuxHathoraAutoBuildOpts.HasServerBuildDirName)
                helpboxLabelStrb.Append($"{nameof(Config.LinuxHathoraAutoBuildOpts.ServerBuildDirName)}, ");
                
            if (!Config.LinuxHathoraAutoBuildOpts.HasServerBuildExeName)
                helpboxLabelStrb.Append($"{nameof(Config.LinuxHathoraAutoBuildOpts.ServerBuildExeName)}, ");

            EditorGUILayout.HelpBox(helpboxLabelStrb.ToString(), MessageType.Error);
            return false; // !enableBuildBtn
        }

        private async Task insertGenerateServerBuildBtn()
        {
            bool clickedBuildBtn = InsertLeftGeneralBtn("Generate Server Build");
            InsertSpace1x();
            
            if (!clickedBuildBtn)
                return;
            
            BuildReport buildReport = HathoraServerBuild.BuildHathoraLinuxServer(Config);
            Assert.That(
                buildReport.summary.result,
                Is.EqualTo(BuildResult.Succeeded),
                "Server build failed. Check console for details.");
        }

        private void insertBuildDirNameHorizGroup()
        {
            string inputStr = base.InsertHorizLabeledTextField(
                _labelStr: "Build directory",
                _tooltip: "Default: `Build-Linux-Server`",
                _val: Config.LinuxHathoraAutoBuildOpts.ServerBuildDirName,
                _alignTextField: GuiAlign.SmallRight);

            bool isChanged = inputStr != Config.LinuxHathoraAutoBuildOpts.ServerBuildDirName;
            if (isChanged)
                onServerBuildDirChanged(inputStr);

            InsertSpace1x();
        }
        
        private void insertBuildFileExeNameHorizGroup()
        {
            string inputStr = base.InsertHorizLabeledTextField(
                _labelStr: "Build file name", 
                _tooltip: "Default: `Unity-LinuxServer.x86_64",
                _val: Config.LinuxHathoraAutoBuildOpts.ServerBuildExeName,
                _alignTextField: GuiAlign.SmallRight);
            
            bool isChanged = inputStr != Config.LinuxHathoraAutoBuildOpts.ServerBuildExeName;
            if (isChanged)
                onServerBuildExeNameChanged(inputStr);
            
            InsertSpace1x();
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

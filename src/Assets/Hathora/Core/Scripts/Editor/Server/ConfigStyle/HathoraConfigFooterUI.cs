// Created by dylan@hathora.dev

using System;
using System.Text;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Editor.Common;
using Hathora.Core.Scripts.Editor.Server.ConfigStyle.PostAuth;
using Hathora.Core.Scripts.Runtime.Server;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Hathora.Core.Scripts.Editor.Server.ConfigStyle
{
    public class HathoraConfigFooterUI : HathoraConfigUIBase
    {
        /// <summary>
        /// For the combo build+deploy btn, we want to use the same deploy func
        /// </summary>
        private readonly HathoraConfigPostAuthBodyDeployUI postAuthBodyDeployUI;
        
        /// <summary>
        /// For the combo build+deploy btn, we want to use the same build func
        /// </summary>
        private readonly HathoraConfigPostAuthBodyBuildUI postAuthBodyBuildUI;
        
        // Scrollable logs
        private Vector2 logsScrollPos = Vector2.zero;


        public HathoraConfigFooterUI(
            HathoraServerConfig _serverConfig, 
            SerializedObject _serializedConfig,
            HathoraConfigPostAuthBodyBuildUI _postAuthBodyBuildUI,
            HathoraConfigPostAuthBodyDeployUI _postAuthBodyDeployUI) 
            : base(_serverConfig, _serializedConfig)
        {
            this.postAuthBodyBuildUI = _postAuthBodyBuildUI;
            this.postAuthBodyDeployUI = _postAuthBodyDeployUI;
            
            // HathoraServerDeploy.OnZipComplete += onDeployAppStatus_1ZipComplete;
            // HathoraServerDeploy.OnBuildReqComplete += onDeployAppStatus_2BuildReqComplete;
            // HathoraServerDeploy.OnUploadComplete += onDeployAppStatus_3UploadComplete;
        }

        public void Draw()
        {
            InsertSpace4x();
            
            if (IsAuthed)
                insertPostAuthFooter();
            else
                insertPreAuthFooter();
        }

        private void insertPostAuthFooter()
        {
            bool enableBuildUploadDeployBtn = 
                !HathoraServerDeploy.IsDeploying &&
                ServerConfig.MeetsBuildBtnReqs() && 
                ServerConfig.MeetsDeployBtnReqs(); 
            
            insertBuildUploadDeployHelpbox(_enabled: enableBuildUploadDeployBtn);
            insertBuildUploadDeployBtn(_enabled: enableBuildUploadDeployBtn); // !await
            insertScrollableLogs();
        }

        private void insertScrollableLogs()
        {
            bool hasLastbuildLogsStrb = ServerConfig.LinuxHathoraAutoBuildOpts.HasLastBuildLogsStrb;
            bool hasLastDeployLogsStrb = ServerConfig.HathoraDeployOpts.HasLastDeployLogsStrb;
            if (!hasLastbuildLogsStrb && !hasLastDeployLogsStrb)
                return;

            logsScrollPos = GUILayout.BeginScrollView(
                logsScrollPos,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(300f));

            if (hasLastbuildLogsStrb)
                insertBuildLogsBox();
            
            if (hasLastDeployLogsStrb)
                InsertLabel(ServerConfig.HathoraDeployOpts.LastDeployLogsStrb.ToString());
            
            GUILayout.EndScrollView();
            InsertSpace1x();
        }

        private void insertBuildLogsBox()
        {
            InsertLabel("Build Logs");
            InsertLabel(ServerConfig.LinuxHathoraAutoBuildOpts.LastBuildLogsStrb.ToString());
        }

        private void insertBuildUploadDeployHelpbox(bool _enabled)
        {
            MessageType helpMsgType = _enabled || HathoraServerDeploy.IsDeploying
                ? MessageType.Info 
                : MessageType.Error;
            
            string helpMsg;
            if (_enabled || HathoraServerDeploy.IsDeploying) 
            {
                helpMsg = "This action will create a new server build, upload to Hathora, " +
                    "and create a new deployment version of your application.";
            }
            else
            {
                StringBuilder helpMsgStrb = new StringBuilder("Missing required fields: ")
                    .Append(HathoraConfigPostAuthBodyBuildUI.GetCreateBuildMissingReqsStrb(
                        ServerConfig,
                        _includeMissingReqsFieldsPrefixStr: false));
                // TODO: add validation for deploy fields (i tried, but currently private)

                helpMsg = helpMsgStrb.ToString();
            }
            
            // Post the help box *before* we disable the button so it's easier to see
            EditorGUILayout.HelpBox(helpMsg, helpMsgType);
        }

        private void insertPreAuthFooter()
        {
            InsertHorizontalLine(1.5f, Color.gray, _space: 20);
            InsertCenterLabel("Learn more about Hathora Cloud");
            InsertLinkLabel("Documentation", HathoraEditorUtils.HATHORA_DOCS_URL, _centerAlign: true);
            InsertLinkLabel("Demo Projects", HathoraEditorUtils.HATHORA_DOCS_DEMO_PROJECTS_URL, _centerAlign: true);
        }

        /// <summary>AKA "The Do-All Button"</summary>
        /// <param name="_enabled"></param>
        private async Task insertBuildUploadDeployBtn(bool _enabled)
        {
            EditorGUI.BeginDisabledGroup(disabled: !_enabled);

            string btnLabelStr = HathoraServerDeploy.IsDeploying 
                ? HathoraServerDeploy.GetDeployFriendlyStatus()
                : "Build, Upload & Deploy New Version";

            // USER INPUT >>
            bool clickedBuildUploadDeployBtn = GUILayout.Button(
                btnLabelStr,
                GeneralButtonStyle);
            
            EditorGUI.EndDisabledGroup();

            if (clickedBuildUploadDeployBtn)
                onBuildUploadDeployBtnClick();

            InsertSpace2x();
        }
        

        #region Status Callbacks
        
        private async Task onBuildUploadDeployBtnClick()
        {
            // TODO: Cancel token
            BuildReport buildReport = null;
            try
            {
                buildReport = await postAuthBodyBuildUI.GenerateServerBuildAsync(); // Cached @ ServerConfig
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraConfigFooterUI.onBuildUploadDeployBtnClick] " +
                    $"GenerateServerBuildAsync => Error: {e}");
                return;
            }
            
            if (buildReport.summary.result != BuildResult.Succeeded)
                return;
            
            // ------------
            // TODO: Cancel token   
            Deployment deployment = null;
            try
            {
                deployment = await postAuthBodyDeployUI.DeployApp(); // Cached @ ServerConfig
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraConfigFooterUI.onBuildUploadDeployBtnClick] " +
                    $"DeployApp => Error: {e}");
                return;
            }
        }
        #endregion Status Callbacks
    }
}

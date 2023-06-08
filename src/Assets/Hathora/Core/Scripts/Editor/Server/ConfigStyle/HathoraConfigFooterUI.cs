// Created by dylan@hathora.dev

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

        
        public HathoraConfigFooterUI(
            HathoraServerConfig _serverConfig, 
            SerializedObject _serializedConfig,
            HathoraConfigPostAuthBodyBuildUI _postAuthBodyBuildUI,
            HathoraConfigPostAuthBodyDeployUI _postAuthBodyDeployUI) 
            : base(_serverConfig, _serializedConfig)
        {
            this.postAuthBodyBuildUI = _postAuthBodyBuildUI;
            this.postAuthBodyDeployUI = _postAuthBodyDeployUI;
            
            HathoraServerDeploy.OnBuildReqComplete += onDeployAppStatus_2BuildReqComplete;
            HathoraServerDeploy.OnUploadComplete += onDeployAppStatus_3UploadComplete;
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
                ServerConfig.MeetsBuildBtnReqs() && 
                ServerConfig.MeetsDeployBtnReqs(); 
            
            insertBuildUploadDeployHelpbox(_enabled: enableBuildUploadDeployBtn);
            insertBuildUploadDeployBtn(_enabled: enableBuildUploadDeployBtn); // !await
            InsertSpace1x();
        }

        private void insertBuildUploadDeployHelpbox(bool _enabled)
        {
            bool isEnabledNotDeploying = _enabled && !HathoraServerDeploy.IsDeploying; 
            MessageType helpMsgType =  isEnabledNotDeploying
                ? MessageType.Info 
                : MessageType.Error;
            
            string helpMsg;
            if (isEnabledNotDeploying)
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

            if (!clickedBuildUploadDeployBtn)
                return;
            
            BuildReport buildReport = await postAuthBodyBuildUI.GenerateServerBuildAsync();
            if (buildReport.summary.result != BuildResult.Succeeded)
                return;
            
            // TODO: Check for cancel token @ postAuthBodyDeployUI.DeployingCancelTokenSrc   
            Deployment deployment = await postAuthBodyDeployUI.DeployApp();
        }
        
        #region Status Callbacks
        private void onDeployAppStatus_1ZipComplete()
        {
            
        }

        private void onDeployAppStatus_2BuildReqComplete(Build __buildinfo)
        {
            
        }

        private void onDeployAppStatus_3UploadComplete()
        {
            
        }
        #endregion Status Callbacks
    }
}

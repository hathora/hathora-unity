// Created by dylan@hathora.dev

using System.Text;
using Hathora.Scripts.Server.Config.Editor.ConfigStyle.PostAuth;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Hathora.Scripts.Server.Config.Editor.ConfigStyle
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
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig,
            HathoraConfigPostAuthBodyBuildUI _postAuthBodyBuildUI,
            HathoraConfigPostAuthBodyDeployUI _postAuthBodyDeployUI) 
            : base(_config, _serializedConfig)
        {
            this.postAuthBodyBuildUI = _postAuthBodyBuildUI;
            this.postAuthBodyDeployUI = _postAuthBodyDeployUI;
            
            HathoraServerDeploy.OnZipComplete += onDeployAppStatus_1ZipComplete;
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
                Config.MeetsBuildBtnReqs() && 
                Config.MeetsDeployBtnReqs(); 
            
            insertBuildUploadDeployHelpbox(_enabled: enableBuildUploadDeployBtn);
            insertBuildUploadDeployBtn(_enabled: enableBuildUploadDeployBtn); // !await
            InsertSpace1x();
        }

        private void insertBuildUploadDeployHelpbox(bool _enabled)
        {
            MessageType helpMsgType = _enabled 
                ? MessageType.Info 
                : MessageType.Error;
            
            string helpMsg;
            if (_enabled)
            {
                helpMsg = "This action will create a new server build, upload to Hathora, " +
                    "and create a new development version of your application.";
            }
            else
            {
                StringBuilder helpMsgStrb = new StringBuilder("Missing required fields: ")
                    .Append(HathoraConfigPostAuthBodyBuildUI.GetCreateBuildMissingReqsStrb(
                        Config,
                        _includeMissingReqsFieldsPrefixStr: false))
                    .Append(HathoraConfigPostAuthBodyRoomUI.GetCreateRoomMissingReqsStrb(
                        Config,
                        _includeMissingReqsFieldsPrefixStr: false));

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
            GUI.enabled = _enabled;
                
            // USER INPUT >>
            bool clickedBuildUploadDeployBtn = GUILayout.Button(
                "Build, Upload & Deploy New Version",
                GeneralButtonStyle);
            
            if (clickedBuildUploadDeployBtn)
            {
                BuildReport buildReport = postAuthBodyBuildUI.GenerateServerBuild();
                if (buildReport.summary.result != BuildResult.Succeeded)
                    return;
                
                // TODO: Check for cancel token @ postAuthBodyDeployUI.DeployingCancelTokenSrc   
                Deployment deployment = await postAuthBodyDeployUI.DeployApp();
            }
            
            GUI.enabled = true;
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

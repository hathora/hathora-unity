// Created by dylan@hathora.dev

using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using Hathora.Scripts.SdkWrapper.Models;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigFooterUI : HathoraConfigUIBase
    {
        public HathoraConfigFooterUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig) 
            : base(_config, _serializedConfig)
        {
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
            bool meetsDeployBtnReqs = Config.MeetsDeployBtnReqs();
            
            insertBuildUploadDeployHelpbox(_enabled: meetsDeployBtnReqs);
            insertBuildUploadDeployBtn(_enabled: meetsDeployBtnReqs); // !await
        }

        private void insertBuildUploadDeployHelpbox(bool _enabled)
        {
            MessageType helpMsgType = _enabled ? MessageType.Info : MessageType.Error;
            string helpMsg = _enabled
                ? "This action will create a new server build, upload to Hathora, " +
                "and create a new development version of your application."
                : $"Requires set: {nameof(HathoraCoreOpts.AppId)}, " +
                $"{nameof(HathoraAutoBuildOpts.ServerBuildExeName)}, " +
                $"{nameof(HathoraAutoBuildOpts.ServerBuildDirName)}, ";

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
                
            if (GUILayout.Button("Build, Upload & Deploy New Version", GeneralButtonStyle))
            {
                BuildReport buildReport = HathoraServerBuild.BuildHathoraLinuxServer(Config);
                if (buildReport.summary.result != BuildResult.Succeeded)
                    return;
                
                Deployment deployment = await HathoraServerDeploy.DeployToHathoraAsync(Config);
                InsertSpace3x();
            }
            
            GUI.enabled = true;
        }
    }
}

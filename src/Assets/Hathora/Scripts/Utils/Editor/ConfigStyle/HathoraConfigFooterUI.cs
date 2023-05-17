// Created by dylan@hathora.dev

using System.Threading.Tasks;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigFooterUI : HathoraConfigUIBase
    {
        public HathoraConfigFooterUI(NetHathoraConfig _config) 
            : base(_config)
        {
        }

        public void Draw()
        {
            if (IsAuthed)
            {
                insertBuildUploadDeployComboBtn();
                return;
            }
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Learn more about Hathora Cloud", PreLinkLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            InsertLinkLabel("Documentation", HathoraEditorUtils.HATHORA_DOCS_URL);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
 
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            InsertLinkLabel("Demo Projects", HathoraEditorUtils.HATHORA_DOCS_DEMO_PROJECTS_URL);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        private async Task insertBuildUploadDeployComboBtn()
        {
            GUI.enabled = Config.MeetsDeployBtnReqs();;

            EditorGUILayout.HelpBox("This action will create a new server build, upload to Hathora, " +
                "and create a new development version of your application.", MessageType.Info);
            if (GUILayout.Button("Build, Upload & Deploy New Version", GeneralButtonStyle))
            {
                BuildReport buildReport = HathoraServerBuild.BuildHathoraLinuxServer(Config);
                if (buildReport.summary.result != BuildResult.Succeeded)
                    return;
                
                await HathoraServerDeploy.DeployToHathoraAsync(Config);
                EditorGUILayout.Space(20);
            }
            
            GUI.enabled = true;
        }
    }
}

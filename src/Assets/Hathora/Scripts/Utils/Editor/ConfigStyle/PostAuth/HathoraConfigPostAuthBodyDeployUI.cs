// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Editor;
using NUnit.Framework;
using UnityEditor;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle.PostAuth
{
    public class HathoraConfigPostAuthBodyDeployUI : HathoraConfigUIBase
    {
        #region Vars
        private HathoraConfigPostAuthBodyDeployAdvUI _advancedDeployUI;

        // Foldouts
        private bool isDeploymentFoldout;
        #endregion // Vars


        #region Init
        public HathoraConfigPostAuthBodyDeployUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
            : base(_config, _serializedConfig)
        {
            if (!HathoraConfigUI.ENABLE_BODY_STYLE)
                return;

            initDrawUtils();
        }
        
        /// <summary>
        /// There are modulated parts of the post-auth body.
        /// </summary>
        private void initDrawUtils()
        {
            _advancedDeployUI = new HathoraConfigPostAuthBodyDeployAdvUI(Config, SerializedConfig);
        }
        #endregion // Init
        
        
        #region UI Draw
        public void Draw()
        {
            if (!IsAuthed)
                return; // You should be calling HathoraConfigPreAuthBodyUI.Draw()

            insertDeploymentSettingsFoldout();
        }

        private void insertDeploymentSettingsFoldout()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            isDeploymentFoldout = EditorGUILayout.Foldout(
                isDeploymentFoldout, 
                "Hathora Deployment Configuration");
            
            if (isDeploymentFoldout)
            {
                EditorGUILayout.EndVertical(); // End of foldout box skin
                return;
            }
    
            InsertSpace2x();

            insertDeploymentSettingsFoldoutComponents();

            EditorGUILayout.EndVertical(); // End of foldout box skin
            InsertSpace3x();
        }

        private void insertDeploymentSettingsFoldoutComponents()
        {
            insertPlanNameHorizPopupList();
            insertRoomsPerProcessHorizSliderGroup();
            insertContainerPortNumberHorizSliderGroup();
            insertTransportTypeHorizRadioBtnGroup();
            _advancedDeployUI.Draw();

            insertDeployAppHelpbox(); // indentLevel is buggy, here: Keep it above
            insertDeployAppBtn(); // !await
        }

        private void insertRoomsPerProcessHorizSliderGroup()
        {
            int inputInt = base.insertHorizLabeledConstrainedIntField(
                _labelStr: "Rooms per process",
                _tooltip: null, // "Default: 1",
                _val: Config.HathoraDeployOpts.RoomsPerProcess,
                _minVal: 1,
                _maxVal: 10000,
                _alignPopup: GuiAlign.SmallRight);

            bool isChanged = inputInt != Config.HathoraDeployOpts.RoomsPerProcess;
            if (isChanged)
                onRoomsPerProcessNumChanged(inputInt);
            
            InsertSpace1x();
        }
        
        private void insertContainerPortNumberHorizSliderGroup()
        {
            int inputInt = base.insertHorizLabeledConstrainedIntField(
                _labelStr: "Container port number",
                _tooltip: "Default: 7777 (<1024 is generally reserved by system)",
                _val: Config.HathoraDeployOpts.ContainerPortWrapper.PortNumber,
                _minVal: 1024,
                _maxVal: 49151,
                _alignPopup: GuiAlign.SmallRight);

            bool isChanged = inputInt != Config.HathoraDeployOpts.ContainerPortWrapper.PortNumber;
            if (isChanged)
                onContainerPortNumberNumChanged(inputInt);
            
            InsertSpace1x();
        }
        
        private void insertTransportTypeHorizRadioBtnGroup()
        {
            int selectedIndex = Config.HathoraDeployOpts.TransportTypeSelectedIndex;
            
            // Get list of string names from PlanName Enum members. Set UPPER.
            List<string> displayOptsStrList = GetStrListOfEnumMemberKeys<TransportType>(
                EnumListOpts.AllCaps);

            int newSelectedIndex = base.insertHorizLabeledPopupList(
                _labelStr: "Transport Type",
                _tooltip: "Default: `UDP` (Fastest; although less reliable)",
                _displayOptsStrArr: displayOptsStrList.ToArray(),
                _selectedIndex: selectedIndex,
                GuiAlign.SmallRight);

            bool isNewValidIndex = selectedIndex >= 0 &&
                newSelectedIndex != selectedIndex &&
                selectedIndex < displayOptsStrList.Count;

            if (isNewValidIndex)
                onSelectedTransportTypePopupIndexChanged(newSelectedIndex);
            
            InsertSpace2x();
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

        private void insertPlanNameHorizPopupList()
        {
            int selectedIndex = Config.HathoraDeployOpts.PlanNameSelectedIndex;
            
            // Get list of string names from PlanName Enum members - with extra info.
            // The index order is !modified.
            List<string> displayOptsStrArr = Enum
                .GetValues(typeof(PlanName))
                .Cast<PlanName>()
                .Select(getPlanNameListWithExtraInfo)
                .ToList();

            int newSelectedIndex = base.insertHorizLabeledPopupList(
                _labelStr: "Plan Size",
                _tooltip: "Default: `Tiny` (Most affordable for pre-production)",
                _displayOptsStrArr: displayOptsStrArr.ToArray(),
                _selectedIndex: selectedIndex,
                GuiAlign.SmallRight);

            bool isNewValidIndex = selectedIndex >= 0 &&
                newSelectedIndex != selectedIndex &&
                selectedIndex < displayOptsStrArr.Count;

            if (isNewValidIndex)
                onSelectedPlanNamePopupIndexChanged(newSelectedIndex);
            
            InsertSpace2x();
        }

        private void insertDeployAppHelpbox()
        {
            InsertSpace2x();
            
            // TODO: Validate that the correct fields are filled before allowing a button click
            const MessageType helpMsgType = MessageType.Info;
            const string helpMsg = "This action will create a new deployment version of your application. " +
                "New rooms will be created with this version of your server.";

            // Post the help box *before* we disable the button so it's easier to see (if toggleable)
            EditorGUILayout.HelpBox(helpMsg, helpMsgType);
        }

        private async Task insertDeployAppBtn()
        {
            bool clickedDeployBtn = insertLeftGeneralBtn("Deploy Application");
            InsertSpace1x();
            
            if (!clickedDeployBtn)
                return;
            
            Deployment deployment = await HathoraServerDeploy.DeployToHathoraAsync(Config);
            Assert.That(deployment?.BuildId, Is.Not.Null,
                "Deployment failed: Check console for details.");
        }
        #endregion // UI Draw

        
        #region Event Logic
        private void onSelectedPlanNamePopupIndexChanged(int _newSelectedIndex)
        {
            Config.HathoraDeployOpts.PlanNameSelectedIndex = _newSelectedIndex;
            SaveConfigChange(
                nameof(Config.HathoraDeployOpts.PlanNameSelectedIndex), 
                _newSelectedIndex.ToString());
        }
        
        private void onSelectedTransportTypePopupIndexChanged(int _newSelectedIndex)
        {
            Config.HathoraDeployOpts.TransportTypeSelectedIndex = _newSelectedIndex;
            SaveConfigChange(
                nameof(Config.HathoraDeployOpts.TransportTypeSelectedIndex), 
                _newSelectedIndex.ToString());
        }

        private void onRoomsPerProcessNumChanged(int _inputInt)
        {
            Config.HathoraDeployOpts.RoomsPerProcess = _inputInt;
            SaveConfigChange(
                nameof(Config.HathoraDeployOpts.RoomsPerProcess), 
                _inputInt.ToString());
        }
        
        private void onContainerPortNumberNumChanged(int _inputInt)
        {
            Config.HathoraDeployOpts.ContainerPortWrapper.PortNumber = _inputInt;
            SaveConfigChange(
                nameof(Config.HathoraDeployOpts.ContainerPortWrapper.PortNumber), 
                _inputInt.ToString());
        }
        #endregion // Event Logic
    }
}

// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public class HathoraConfigHeaderUI : HathoraConfigUIBase
    {
        public HathoraConfigHeaderUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig) 
            : base(_config, _serializedConfig)
        {
        }

        public void Draw()
        {
            HathoraEditorUtils.InsertBanner(
                _includeVerticalGroup: false,
                _wrapperExtension: 55f); // Place banner @ top
            HathoraEditorUtils.InsertHathoraSloganLbl();

            EditorGUILayout.Space(5);
            insertHeaderBtns();
            GUILayout.EndVertical();
            
            insertEditingTemplateWarningMemo();
        }
        
        private void insertHeaderBtns()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUILayoutOption[] buttonOptions =
            {
                GUILayout.MinWidth(100), 
                GUILayout.MinHeight(20), 
                GUILayout.ExpandWidth(true),
            };
            
            if (GUILayout.Button("Get Started", GeneralButtonStyle, buttonOptions))
            {
                Application.OpenURL(HathoraEditorUtils.HATHORA_DOCS_GETTING_STARTED_URL);
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Unity Tutorial", GeneralButtonStyle, buttonOptions))
            {
                Application.OpenURL(HathoraEditorUtils.HATHORA_DOCS_UNITY_TUTORIAL_URL);
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Discord", GeneralButtonStyle, buttonOptions))
            {
                Application.OpenURL(HathoraEditorUtils.HATHORA_DISCORD_URL);
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Website", GeneralButtonStyle, buttonOptions))
            {
                Application.OpenURL(HathoraEditorUtils.HATHORA_HOME_URL);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Only insert memo if using the template file
        /// </summary>
        private void insertEditingTemplateWarningMemo()
        {
            string configTemplateName = $"{nameof(NetHathoraConfig)}.template";
            bool isDefaultName = Config.name == configTemplateName;
            if (!isDefaultName)
                return;
            
            EditorGUILayout.HelpBox("You are editing a template!\n" +
                "1. Duplicate this (CTRL+D)\n" +
                "2. Add dupe to .gitignore >> treat as an .env file", 
                MessageType.Warning);
            
            EditorGUILayout.Space(10);
        }
    }
}

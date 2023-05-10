// Created by dylan@hathora.dev

using System.Collections.Generic;
using System.Linq;
using Hathora.Scripts.Net.Server;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor
{
    public class HathoraServerConfigFinder : EditorWindow
    {
        private List<HathoraServerConfig> serverConfigs;
        private Vector2 scrollPos;

        [MenuItem("Hathora/Find Configs", priority = -1000)]
        public static void ShowWindow()
        {
            var window = GetWindow<HathoraServerConfigFinder>("Hathora Server UserConfig Finder");
            window.minSize = new Vector2(300, 200);
            window.maxSize = window.minSize;
        }

        private void OnEnable()
        {
            findAllHathoraServerConfigs();
            if (serverConfigs.Count == 1) 
            {
                selectHathoraServerConfig(serverConfigs[0]);
            }
        }

        private void OnGUI()
        {
            HathoraEditorUtils.InsertBanner();

            EditorGUILayout.Space(10);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (var config in serverConfigs)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(config.name, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    selectHathoraServerConfig(config);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (serverConfigs.Count == 1)
            {
                Close();
            }
        }

        private void findAllHathoraServerConfigs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Hathora.Scripts.Net.Server.HathoraServerConfig");
            serverConfigs = guids.Select(guid => AssetDatabase.LoadAssetAtPath<HathoraServerConfig>(
                AssetDatabase.GUIDToAssetPath(guid))).ToList();
        }

        private void selectHathoraServerConfig(HathoraServerConfig config)
        {
            EditorGUIUtility.PingObject(config);
            Selection.activeObject = config;
        }
    }
}

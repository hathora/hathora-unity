// Created by dylan@hathora.dev

using System.Collections.Generic;
using System.Linq;
using Hathora.Scripts.Net.Server;
using Hathora.Scripts.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Editor
{
    [InitializeOnLoad]
    public class HathoraServerConfigFinder : EditorWindow
    {
        private const string ShowOnStartupKey = "HathoraServerConfigFinder.ShowOnStartup";
        private static List<NetHathoraConfig> serverConfigs;
        private Vector2 scrollPos;

        static HathoraServerConfigFinder()
        {
            EditorApplication.delayCall += ShowWindowOnStartup;
        }

        private static void ShowWindowOnStartup()
        {
            if (EditorPrefs.GetBool(ShowOnStartupKey, true))
                ShowWindow();
        }

        [MenuItem("Hathora/Config Finder On Startup/Enable", false, -1001)]
        public static void EnableConfigFinderOnStartup()
        {
            EditorPrefs.SetBool(ShowOnStartupKey, true);
        }

        [MenuItem("Hathora/Config Finder On Startup/Disable", false, -1000)]
        public static void DisableConfigFinderOnStartup()
        {
            EditorPrefs.SetBool(ShowOnStartupKey, false);
        }

        [MenuItem("Hathora/Find Configs", priority = -1000)]
        public static void ShowWindow()
        {
            HathoraServerConfigFinder window = GetWindow<HathoraServerConfigFinder>(
                "Hathora Server UserConfig Finder");
            
            window.minSize = new Vector2(300, 200);
            window.maxSize = window.minSize;
            
            // Select the 1st one found
            selectHathoraServerConfig(serverConfigs[0]);
        }

        [MenuItem("Hathora/Config Finder On Startup/Enable", true)]
        public static bool ValidateEnableConfigFinderOnStartup()
        {
            return !EditorPrefs.GetBool(ShowOnStartupKey, true);
        }

        [MenuItem("Hathora/Config Finder On Startup/Disable", true)]
        public static bool ValidateDisableConfigFinderOnStartup()
        {
            return EditorPrefs.GetBool(ShowOnStartupKey, true);
        }

        private void OnEnable()
        {
            findAllHathoraServerConfigs();
        }

        private void OnGUI()
        {
            HathoraEditorUtils.InsertBanner();

            EditorGUILayout.Space(10);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            // Check if any serverConfigs are null with LINQ: If they are, findAllHathoraServerConfigs() to refresh cache.
            if (serverConfigs != null && serverConfigs.Any(config => config == null))
                findAllHathoraServerConfigs();

            foreach (NetHathoraConfig config in serverConfigs)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(config.name, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                    selectHathoraServerConfig(config);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (serverConfigs.Count == 1)
                Close();
        }

        private void findAllHathoraServerConfigs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Hathora.Scripts.Net.Server.NetHathoraConfig");
            serverConfigs = guids.Select(guid => AssetDatabase.LoadAssetAtPath<NetHathoraConfig>(
                AssetDatabase.GUIDToAssetPath(guid))).ToList();
        }

        private static void selectHathoraServerConfig(NetHathoraConfig config)
        {
            EditorGUIUtility.PingObject(config);
            Selection.activeObject = config;
        }
    }
}

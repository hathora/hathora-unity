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
        private static Vector2 scrollPos;
        private static GUIStyle richTxtLabelStyle;

        
        #region Init
        /// <summary>Just run once on startup</summary>
        static HathoraServerConfigFinder()
        {
            EditorApplication.delayCall += ShowWindowOnStartup;
        }
        
        /// <summary>
        /// As soon as we see the window, we find all the configs and select the latest (if any).
        /// </summary>
        private void OnEnable()
        {
            findAllHathoraServerConfigs();
        }
        
        private static void InitStyles()
        {
            richTxtLabelStyle ??= new GUIStyle(EditorStyles.label) { richText = true };
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
        #endregion // Init
        

        [MenuItem("Hathora/Find Configs _%#h", priority = -1000)] // Ctrl + Shift + H
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

        private void OnGUI()
        {
            InitStyles();
            HathoraEditorUtils.InsertBanner();
            EditorGUILayout.Space(10);

            insertDescrLbl();
            EditorGUILayout.Space(10);
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
    
            // Check if any serverConfigs are null with LINQ: If they are, findAllHathoraServerConfigs() to refresh cache.
            if (serverConfigs != null && serverConfigs.Any(config => config == null))
                findAllHathoraServerConfigs();

            insertBtnForEachConfigFound();
            insertNewConfigBtn();
            
            EditorGUILayout.Space(5);
        }

        private void insertDescrLbl()
        {
            string headerColor = HathoraEditorUtils.HATHORA_GREEN_HEX;
            string lblContent = serverConfigs?.Count > 0
                ? $"<b><color={headerColor}>Choose a Hathora Config:</color></b>"
                : $"<color={headerColor}>Create a new Hathora config to get started!</color>";
            
            GUILayout.Label(lblContent, richTxtLabelStyle);
        }

        private void insertNewConfigBtn()
        {
            // Add the "New Config" button
            if (GUILayout.Button("New Config", GUILayout.Height(30)))
                createAndSelectNewConfig();
        }

        private void insertBtnForEachConfigFound()
        {
            foreach (NetHathoraConfig config in serverConfigs)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(config.name, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                    selectHathoraServerConfig(config);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void createAndSelectNewConfig()
        {
            string basePath = "Assets/Hathora";
            string fullPath = $"{basePath}/NetHathoraConfig.asset";

            // Create Hathora directory if it doesn't exist
            if (!AssetDatabase.IsValidFolder(basePath))
                AssetDatabase.CreateFolder("Assets", "Hathora");

            // Create a new instance of NetHathoraConfig
            NetHathoraConfig newConfig = ScriptableObject.CreateInstance<NetHathoraConfig>();

            // Save the new instance as an asset
            AssetDatabase.CreateAsset(newConfig, AssetDatabase.GenerateUniqueAssetPath(fullPath));

            // Refresh the AssetDatabase to ensure the new asset is properly saved and recognized
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select the newly created NetHathoraConfig in the Project tab
            selectHathoraServerConfig(newConfig);

            // Refresh the list of configs
            findAllHathoraServerConfigs();        
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

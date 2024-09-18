// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HathoraIntegration.Scripts
{
    /// <summary>
    /// Commandline helper - run via `YourBuild.exe -mode {server|client|host}`.
    /// (!) `-scene` is loaded / awaited before any other cmd.
    /// Unity: Command Line Helper | https://docs-multiplayer.unity3d.com/netcode/current/tutorials/command-line-helper/index.html  
    /// </summary>
    public class CommandLineArgHandler : MonoBehaviour
    {
        #region Vars
        private static bool _sceneArgConsumed = false;
        
        /// <summary>We only trigger this once</summary>
        public static bool SceneArgConsumed
        {
            get => _sceneArgConsumed;
            set 
            {
                if (value)
                    Debug.Log($"[CommandLineArgHandler] SceneArgConsumed @ " +
                        SceneManager.GetActiveScene().name);
                
                _sceneArgConsumed = value;
            }
        }
        
        private static bool _modeArgConsumed;
        
        /// <summary>We only trigger this once</summary>
        public static bool ModeArgConsumed
        {
            get => _modeArgConsumed;
            set 
            {
                if (value)
                    Debug.Log($"[CommandLineArgHandler] ModeArgConsumed @ {SceneManager.GetActiveScene().name}");
                
                _modeArgConsumed = value;
            }
        }
        #endregion // Vars


        #region Init
        private void Awake() {}
        private async void Start() => await InitArgsAsync();
        
        /// <summary>
        /// (!) Some args like `-scene` and `-mode` are statically consumed only once
        /// (eg: reloading the scene won't apply them).</summary>
        private async Task InitArgsAsync()
        {
            string logPrefix = $"[CommandLineArgHandler.{nameof(InitArgsAsync)}]";
            
            Dictionary<string, string> args = GetCommandlineArgs();
            string argsStr = string.Join(" ", args.Select(kvp => $"{kvp.Key} {kvp.Value}"));
            Debug.Log($"{logPrefix} Handling args: `{argsStr}`");

            try
            {
                // -scene {string} // Load scene by name
                bool hasSceneArg = args.TryGetValue("-scene", out string sceneName) && !string.IsNullOrEmpty(sceneName); 
                    
                if (!Application.isEditor)
                    Debug.Log($"{logPrefix} Has `-scene` arg? {hasSceneArg}");
                    
                if (hasSceneArg)
                    await InitArgScene(sceneName);
            
                // -----------------
                // -_mode {server|client|host} // Logs and start netcode
                bool hasModeArg = args.TryGetValue("-mode", out string mode) && !string.IsNullOrEmpty(mode);
                
                if (!Application.isEditor)
                    Debug.Log($"{logPrefix} Has `-mode` arg? {hasModeArg}");
                
                if (hasModeArg)
                    InitArgMode(mode);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} Error: {e.Message}");
                throw;
            }
        }
        
        private async Task InitArgScene(string _sceneName)
        {
            string logPrefix = $"[CommandLineArgHandler.{nameof(InitArgScene)}]";

            Debug.Log($"{logPrefix} sceneName: {_sceneName}");

            // Get current scene name
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == _sceneName)
            {
                _sceneArgConsumed = true;
                return; // We're already in this scene
            }

            if (SceneArgConsumed)
            {
                Debug.LogWarning($"{logPrefix} {nameof(SceneArgConsumed)}, already");
                return;
            }

            try
            {
                await loadSceneOnceFromArgAsync(_sceneName);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} Error: {e}");
                throw;
            }
        }
        #endregion // Init

        
        /// <summary>
        /// After using this once, you won't be able to do it again
        /// (to prevent multiple arg handling stack overflows).
        /// 
        /// Mostly used for args or initial scene selection.
        /// </summary>
        /// <param name="_sceneName">CLI Arg passed from `-scene {sceneName}`.</param>
        private static async Task loadSceneOnceFromArgAsync(string _sceneName)
        {
            string logPrefix = $"[CommandLineArgHandler.{nameof(loadSceneOnceFromArgAsync)}]";
            Debug.Log($"{logPrefix} sceneName: {_sceneName}");

            if (SceneArgConsumed)
            {
                Debug.LogWarning($"{logPrefix} {nameof(SceneArgConsumed)} already consumed! Aborting.");
                return;
            }

            SceneArgConsumed = true;
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
                _sceneName, 
                LoadSceneMode.Single);

            while (!asyncLoad.isDone)
                await Task.Yield();
        }
        
        /// <summary>
        /// - "server" -> StartServer()
        /// - "client" -> StartClient()
        /// - "host" -> StartHost() // server+client together
        /// </summary>
        private void InitArgMode(string _mode)
        {
            string logPrefix = $"[CommandLineArgHandler.{nameof(InitArgMode)}]";

            Debug.Log($"{logPrefix} mode: {_mode}");

            if (ModeArgConsumed)
            {
                Debug.LogWarning($"{logPrefix} {nameof(ModeArgConsumed)}, already");
                return;
            }
            
            ModeArgConsumed = true;

            switch (_mode)
            {
                case "server":
                    ArgModeStartServer();
                    break;
                
                case "client":
                    ArgModeStartClient();
                    break;
                
                case "host":
                    ArgModeStartHost();
                    break;
            }
        }
        
        private void ArgModeStartServer()
        {
            bool alreadyStartedServer = false; // TODO: Check your NetworkManager to ensure it's not already started
            if (alreadyStartedServer)
                return;
            
            Debug.Log($"[{nameof(CommandLineArgHandler)}] {nameof(ArgModeStartServer)}");
            // NetworkManager.Instance.StartServer(); // TODO
        }

        private void ArgModeStartClient()
        {
            bool alreadyStartedClient = false; // TODO: Check your NetworkManager to ensure it's not already started
            if (alreadyStartedClient)
                return;
            
            Debug.Log($"[{nameof(CommandLineArgHandler)}] {nameof(ArgModeStartClient)}");
            // NetworkManager.Instance.StartClient(); // TODO
        }

        private void ArgModeStartHost()
        {
            bool alreadyStartedClientOrServer = false; // TODO: Check your NetworkManager to ensure it's not already started
            if (alreadyStartedClientOrServer)
                return;
            
            Debug.Log($"[{nameof(CommandLineArgHandler)}] {nameof(ArgModeStartHost)} (Server+Client)");
            // NetworkManager.Instance.StartHost(); // TODO
        }

        
        #region Utils
        private static Dictionary<string, string> GetCommandlineArgs()
        {
            Dictionary<string, string> argDictionary = new();

            string[] args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i].ToLower();

                if (!arg.StartsWith("-"))
                    continue;
                
                string value = i < args.Length - 1 
                    ? args[i + 1].ToLower() 
                    : null;
                
                value = value?.StartsWith("-") ?? false 
                    ? null 
                    : value;

                argDictionary.Add(arg, value);
            }
            return argDictionary;
        }
        
        void OnDisable()
        {
            // Static vars may persist between Editor play sessions. 
            _sceneArgConsumed = false;
            _modeArgConsumed = false;
        }
        #endregion // Utils
    }
}

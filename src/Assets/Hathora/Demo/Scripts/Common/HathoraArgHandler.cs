// Created by dylan@hathora.dev
// Unity: Command Line Helper | https://docs-multiplayer.unity3d.com/netcode/current/tutorials/command-line-helper/index.html  

using System.Collections.Generic;
using FishNet;
using Hathora.Demo.Scripts.Client.ClientMgr;
using UnityEngine;

namespace Hathora.Demo.Scripts.Common
{
    /// <summary>
    /// Commandline helper - run via `-mode {server|client|host} -memo {someStr}`
    /// </summary>
    public class HathoraArgHandler : MonoBehaviour
    {
        private void Start()
        {
            if (Application.isEditor) 
                return;

            Dictionary<string, string> args = GetCommandlineArgs();

            // -mode {server|client|host} // Logs and start netcode
            if (args.TryGetValue("-mode", out string mode))
                initMode(mode);

            // -memo {string} // Show arbitrary text at bottom of screen
            if (args.TryGetValue("-memo", out string memoStr) && !string.IsNullOrEmpty(memoStr))
                NetUI.Singleton.SetShowDebugMemoTxt(memoStr);
        }

        /// <summary>
        /// -mode {server|client|host}
        /// </summary>
        /// <param name="mode"></param>
        private void initMode(string mode)
        {
            switch (mode)
            {
                case "server":
                    Debug.Log("[HathoraArgHandler] @ initMode - Starting server ...");

                    if (!InstanceFinder.ServerManager.Started)
                    {
                        // It's very possible this already started, if FishNet's NetworkManager.ServerMgr
                        // start on headless checkbox is true
                        InstanceFinder.ServerManager.StartConnection();
                    }
                    break;
                
                case "client":
                    Debug.Log("[HathoraArgHandler] @ initMode - Starting client ...");
                    if (!InstanceFinder.ClientManager.Started)
                        InstanceFinder.ClientManager.StartConnection();
                    break;
                
                case "host":
                    Debug.Log("[HathoraArgHandler] @ initMode - starting host (server+client) ...");
                    
                    if (!InstanceFinder.ServerManager.Started)
                        InstanceFinder.ServerManager.StartConnection();
    
                    if (!InstanceFinder.ClientManager.Started)
                        InstanceFinder.ClientManager.StartConnection();
                    break;
            }
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
        #endregion // Utils
    }
}

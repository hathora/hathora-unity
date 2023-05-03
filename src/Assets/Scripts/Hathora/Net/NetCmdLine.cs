// Created by dylan@hathora.dev
// Unity: Command Line Helper | https://docs-multiplayer.unity3d.com/netcode/current/tutorials/command-line-helper/index.html  

using System.Collections.Generic;
using FishNet;
using Hathora.Common;
using UnityEngine;

namespace Hathora.Net
{
    /// <summary>
    /// Commandline helper - run via `-mode {server|client|host} -memo {someStr}`
    /// </summary>
    public class NetCmdLine : MonoBehaviour
    {
        private void Start()
        {
            if (Application.isEditor) 
                return;

            var args = GetCommandlineArgs();

            if (args.TryGetValue("-mode", out string mode))
                initMode(mode);

            if (args.TryGetValue("-memo", out string memoStr) && !string.IsNullOrEmpty(memoStr))
                NetUI.Singleton.SetShowDebugMemoTxt(memoStr);
        }

        private void initMode(string mode)
        {
            switch (mode)
            {
                case "server":
                    Debug.Log("[NetCmdLine] @ initMode - Starting server ...");
                    InstanceFinder.ServerManager.StartConnection();
                    break;
                
                case "client":
                    Debug.Log("[NetCmdLine] @ initMode - Starting client ...");
                    InstanceFinder.ClientManager.StartConnection();
                    break;
                
                case "host":
                    Debug.Log("[NetCmdLine] @ initMode - starting host (server+client) ...");
                    InstanceFinder.ServerManager.StartConnection();
                    InstanceFinder.ClientManager.StartConnection();
                    break;
            }
        }

        private static Dictionary<string, string> GetCommandlineArgs()
        {
            Dictionary<string, string> argDictionary = new();

            var args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i].ToLower();
                if (arg.StartsWith("-"))
                {
                    var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    value = (value?.StartsWith("-") ?? false) ? null : value;

                    argDictionary.Add(arg, value);
                }
            }
            return argDictionary;
        }
    }
}

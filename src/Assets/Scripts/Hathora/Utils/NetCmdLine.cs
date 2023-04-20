// Created by dylan@hathora.dev
// Unity: Command Line Helper | https://docs-multiplayer.unity3d.com/netcode/current/tutorials/command-line-helper/index.html  

using System.Collections.Generic;
using Hathora.Net.Common;
using UnityEngine;

namespace Hathora.Utils
{
    public class NetCmdLine : NetMgrBase
    {
        private void Start()
        {
            if (Application.isEditor) 
                return;

            var args = GetCommandlineArgs();

            if (args.TryGetValue("-mode", out string mode))
                initMode(mode);

            if (args.TryGetValue("-memo", out string memoStr))
                NetUi.SetShowDebugMemoTxt(memoStr);
        }

        private void initMode(string mode)
        {
            switch (mode)
            {
                case "server":
                    NetMgr.StartServer();
                    break;
                
                case "host":
                    NetMgr.StartHost();
                    break;
                
                case "client":
                    NetMgr.StartClient();
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

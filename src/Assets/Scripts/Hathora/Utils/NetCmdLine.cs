// Created by dylan@hathora.dev
// Unity: Command Line Helper | https://docs-multiplayer.unity3d.com/netcode/current/tutorials/command-line-helper/index.html  

using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Hathora.Utils
{
    public class NetCmdLine : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI debugMemoText;
        
        private NetworkManager netManager;

        private void Start()
        {
            netManager = GetComponentInParent<NetworkManager>();

            if (Application.isEditor) 
                return;

            var args = GetCommandlineArgs();

            if (args.TryGetValue("-mode", out string mode))
                initMode(mode);

            if (args.TryGetValue("-memo", out string memoStr))
                initDebugMemoTxt(memoStr);
        }
        
        private void initDebugMemoTxt(string memoStr)
        {
            debugMemoText.text = memoStr;
            debugMemoText.gameObject.SetActive(true);
            Debug.Log($"[NetCmdLine] Debug Memo: '{memoStr}'");
        }

        private void initMode(string mode)
        {
            switch (mode)
            {
                case "server":
                    netManager.StartServer();
                    break;
                
                case "host":
                    netManager.StartHost();
                    break;
                
                case "client":
                    netManager.StartClient();
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

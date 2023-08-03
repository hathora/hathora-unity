// Created by dylan@hathora.dev

using Hathora.Demos._2_MirrorDemo.HathoraScripts.Client.ClientMgr;
using Mirror;
using Hathora.Demos.Shared.Scripts.Common;
using kcp2k;
using UnityEngine;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Common
{
    /// <summary>
    /// Commandline helper - run via `-mode {server|client|host} -memo {someStr}`
    /// </summary>
    public class HathoraMirrorArgHandler : HathoraArgHandlerBase
    {
        [SerializeField]
        private NetworkManager manager;

        
        private async void Start() => 
            await base.InitArgsAsync();

        protected override void InitArgMemo(string _memoStr)
        {
            base.InitArgMemo(_memoStr);
            HathoraMirrorClientMgrDemoUi.Singleton.SetShowDebugMemoTxt(_memoStr);
        }

        protected override void ArgModeStartServer()
        {
            base.ArgModeStartServer();

            // It's very possible this already started, if Mirror's NetworkManager
            // start on headless checkbox is true
            if (NetworkServer.active)
                return;

            Debug.Log("[HathoraMirrorArgHandler] Starting Mirror Server ...");
            manager.StartServer();
        }

        protected override void ArgModeStartClient()
        {
            base.ArgModeStartClient();
            
            // It's very possible this already started, if Mirror's NetworkManager
            // Auto join clients checkbox is true
            if (NetworkClient.active)
                return;

            Debug.Log("[HathoraMirrorArgHandler] Starting Client ...");

            // Go through Hathora ClientMgr middleware to ensure the correct Transport is used
            HathoraMirrorClientMgr.Singleton.StartClient();
        }
        
        protected override void ArgModeStartHost()
        {
            // base.StartHost(); // We don't want to just StartServer -> StartClient().

            // It's very possible this already started, if Mirror's NetworkManager
            // start on headless checkbox is true
            if (NetworkServer.active)
                return;
            
            Debug.Log("[HathoraMirrorArgHandler] Starting Host (Server+Client) ...");
            manager.StartHost(); // Different from FishNet
        }
    }
}

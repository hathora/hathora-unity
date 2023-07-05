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

        [SerializeField]
        private KcpTransport kcpTransport;

        
        private void Start() => base.Init();

        protected override void InitMemo(string _memoStr)
        {
            base.InitMemo(_memoStr);
            HathoraMirrorClientMgrUi.Singleton.SetShowDebugMemoTxt(_memoStr);
        }

        protected override void StartServer()
        {
            base.StartServer();

            if (!NetworkServer.active)
                return;
            
            // It's very possible this already started, if Mirror's NetworkManager
            // start on headless checkbox is true
            Debug.Log("[HathoraMirrorArgHandler] Starting Server ...");
            manager.StartServer();
        }

        protected override void StartClient()
        {
            base.StartClient();
            
            if (!NetworkClient.active)
                return;

            
            Debug.Log("[HathoraMirrorArgHandler] Starting Client to " +
                $"{manager.networkAddress}:{kcpTransport.Port} (TODO_PROTOCOL) ...");
            manager.StartClient();
        }
    }
}

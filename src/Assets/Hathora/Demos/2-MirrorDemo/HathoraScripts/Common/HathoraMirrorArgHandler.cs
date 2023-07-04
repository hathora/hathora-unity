// Created by dylan@hathora.dev

using Hathora.Demos._1_FishNetDemo.HathoraScripts.Client.ClientMgr;
using Mirror;
using Hathora.Demos.Shared.Scripts.Client;
using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using Hathora.Demos.Shared.Scripts.Common;
using kcp2k;
using UnityEngine;
using UnityEngine.Serialization;

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
            HathoraFishnetClientMgrUi.Singleton.SetShowDebugMemoTxt(_memoStr);
        }

        protected override void StartServer()
        {
            base.StartServer();

            if (!NetworkServer.active)
                return;
            
            // It's very possible this already started, if FishNet's NetworkManager.ServerMgr
            // start on headless checkbox is true
            Debug.Log("[HathoraFishnetArgHandler] Starting Server ...");
            manager.StartServer();
        }

        protected override void StartClient()
        {
            base.StartClient();
            
            if (!NetworkClient.active)
                return;

            
            Debug.Log("[HathoraFishnetArgHandler] Starting Client to " +
                $"{manager.networkAddress}:{kcpTransport.Port} (TODO_PROTOCOL) ...");
            manager.StartClient();
        }
    }
}

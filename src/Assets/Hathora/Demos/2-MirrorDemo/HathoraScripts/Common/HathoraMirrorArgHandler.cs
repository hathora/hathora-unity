// Created by dylan@hathora.dev

using FishNet;
using Hathora.Core.Scripts.Runtime.Common.Utils;
using Hathora.Demos.Shared.Scripts.Client;
using Hathora.Demos.Shared.Scripts.Common;
using UnityEngine;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Common
{
    /// <summary>
    /// Commandline helper - run via `-mode {server|client|host} -memo {someStr}`
    /// </summary>
    public class HathoraMirrorArgHandler : HathoraArgHandlerBase
    {
        private void Start() => base.Init();

        protected override void InitMemo(string _memoStr)
        {
            base.InitMemo(_memoStr);
            NetUI.Singleton.SetShowDebugMemoTxt(_memoStr);
        }

        protected override void StartServer()
        {
            base.StartServer();

            if (InstanceFinder.ServerManager.Started)
                return;
            
            // It's very possible this already started, if FishNet's NetworkManager.ServerMgr
            // start on headless checkbox is true
            Debug.Log("[HathoraFishnetArgHandler] Starting Server ...");
            InstanceFinder.ServerManager.StartConnection();
        }

        protected override void StartClient()
        {
            base.StartClient();

            if (InstanceFinder.ClientManager.Started)
                return;
            
            Debug.Log("[HathoraFishnetArgHandler] Starting Client ...");
            InstanceFinder.ClientManager.StartConnection();
        }
    }
}

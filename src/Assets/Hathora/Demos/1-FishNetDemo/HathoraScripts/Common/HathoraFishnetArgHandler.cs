// Created by dylan@hathora.dev

using FishNet;
using Hathora.Demos._1_FishNetDemo.HathoraScripts.Client.ClientMgr;
using Hathora.Demos.Shared.Scripts.Common;
using UnityEngine;

namespace Hathora.Demos._1_FishNetDemo.HathoraScripts.Common
{
    /// <summary>
    /// Commandline helper - run via `YourBuild.exe -mode {server|client|host}`.
    /// </summary>
    public class HathoraFishnetArgHandler : HathoraArgHandlerBase
    {
        protected override void Awake() {}
        protected override void Start() {}

        protected override void ArgModeStartServer()
        {
            base.ArgModeStartServer();

            // It's very possible this already started, if FishNet's NetworkManager.ServerMgr
            // start on headless checkbox is true
            if (InstanceFinder.ServerManager.Started || InstanceFinder.ServerManager.GetStartOnHeadless())
                return;

            Debug.Log("[HathoraFishnetArgHandler] Starting FishNet Server ...");
            InstanceFinder.ServerManager.StartConnection();
        }

        protected override void ArgModeStartClient()
        {
            base.ArgModeStartClient();

            if (InstanceFinder.ClientManager.Started)
                return;
            
            Debug.Log("[HathoraFishnetArgHandler] Starting Client ...");
            FishnetStateMgr.Singleton.StartClient();
        }

        protected override void ArgModeStartHost()
        {
            base.ArgModeStartHost();
            
            if (InstanceFinder.ClientManager.Started || InstanceFinder.ServerManager.Started)
                return;
            
            Debug.Log("[HathoraFishnetArgHandler] Starting host (server+client) ...");
        }
    }
}

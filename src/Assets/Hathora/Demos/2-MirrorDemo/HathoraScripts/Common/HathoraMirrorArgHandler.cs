// Created by dylan@hathora.dev

using Hathora.Demos._2_MirrorDemo.HathoraScripts.Client.ClientMgr;
using Mirror;
using Hathora.Demos.Shared.Scripts.Common;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Common
{
    /// <summary>
    /// Commandline helper - run via `-mode {server|client|host}`
    /// </summary>
    public class HathoraMirrorArgHandler : HathoraArgHandlerBase
    {
        protected override void Awake() {}
        protected override void Start() {}

        protected override void ArgModeStartServer()
        {
            base.ArgModeStartServer();

            // It's very possible this already started, if Mirror's NetworkManager
            // start on headless checkbox is true
            Assert.IsNotNull(NetworkManager.singleton, "Expected NetworkManager to be serialized: " +
                "See your 1st scene's NetworkManager.HathoraMirrorArgHandler and serialize in the NetworkManager");
            
            if (NetworkManager.singleton.autoStartServerBuild || NetworkManager.singleton.isNetworkActive)
                return;

            Debug.Log("[HathoraMirrorArgHandler] Starting Mirror Server ...");
            NetworkManager.singleton.StartServer();
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
            MirrorStateMgr.Singleton.StartClient();
        }
        
        protected override void ArgModeStartHost()
        {
            // base.StartHost(); // We don't want to just StartServer -> StartClient().

            // It's very possible this already started, if Mirror's NetworkManager
            // start on headless checkbox is true
            if (NetworkServer.active)
                return;
            
            Debug.Log("[HathoraMirrorArgHandler] Starting Host (Server+Client) ...");
            NetworkManager.singleton.StartHost(); // Different from FishNet
        }
    }
}

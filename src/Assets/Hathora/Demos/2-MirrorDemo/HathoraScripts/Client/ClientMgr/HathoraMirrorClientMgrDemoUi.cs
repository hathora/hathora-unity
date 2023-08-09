// Created by dylan@hathora.dev

using System.Text.RegularExpressions;
using Hathora.Core.Scripts.Runtime.Common.Utils;
using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using Mirror;
using UnityEngine;
using UnityEngine.Assertions;
using HathoraMirrorClientMgr = Hathora.Demos._2_MirrorDemo.HathoraScripts.Client.ClientMgr.HathoraMirrorClientMgr;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Client.ClientMgr
{
    /// <summary>
    /// Handles the non-Player UI so we can keep the logic separate.
    /// - Generally, this is going to be pre-connection UI such as create/join lobbies.
    /// - UI OnEvent entry points from Buttons start here.
    /// - This particular child should be used for both Mirror.
    /// </summary>
    public class HathoraMirrorClientMgrDemoUi : HathoraClientMgrDemoUi, IHathoraNetClientMgrUi
    {
        public static HathoraMirrorClientMgrDemoUi Singleton { get; private set; }
        private static HathoraMirrorClientMgr HathoraClientMgr => 
            HathoraMirrorClientMgr.Singleton;
        

        #region Init
        protected override void OnStart()
        {
            base.OnStart();
            InitOnStart(HathoraClientMgr);
        }

        protected override void SetSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraMirrorClientMgrUi]**ERR @ setSingleton: " +
                    "Destroying dupe");
                
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }
        #endregion // Init
        
        
        #region UI Interactions
        public override void OnStartServerBtnClick()
        {
            base.OnStartServerBtnClick();
            NetworkManager.singleton.StartServer();
        }

        /// <param name="_hostPortOverride">host:port provided by Hathora</param>
        public override void OnStartClientBtnClick(string _hostPortOverride = null)
        {
            // We want to override hostPort from the input field - np if null
            _hostPortOverride = HelloWorldDemoUi.ClientConnectInputField.text.Trim();
            
            if (!string.IsNullOrEmpty(_hostPortOverride))
            {
                // Validate input: "{ip||host}:{port}" || "localhost:7777"
                string pattern = HathoraUtils.GetHostIpPortPatternStr();
                bool isHostIpPatternMatch = Regex.IsMatch(_hostPortOverride, pattern);
                Assert.IsTrue(isHostIpPatternMatch, "Expected 'host:port' pattern, " +
                    "such as '1.proxy.hathora.dev:7777' || 'localhost:7777' || '192.168.1.1:7777");    
            }
            
            base.OnStartClientBtnClick(_hostPortOverride); // Logs
            HathoraClientMgr.StartClient(_hostPortOverride);
        }

        public override void OnStartHostBtnClick()
        {
            base.OnStartHostBtnClick();
            NetworkManager.singleton.StartHost();
        }

        public override void OnStopServerBtnClick()
        {
            base.OnStopServerBtnClick();
            NetworkManager.singleton.StopServer();
        }
        
        public override void OnStopClientBtnClick()
        {
            base.OnStopClientBtnClick();
            NetworkManager.singleton.StopClient();
        }
        
        public override void OnStopHostBtnClick()
        {
            base.OnStopHostBtnClick();
            NetworkManager.singleton.StopHost();
        }
        
        public override void OnJoinLobbyAsClientBtnClick()
        {
            base.OnJoinLobbyAsClientBtnClick();
            Connect();
        }
        
        public void Connect() => 
            HathoraClientMgr.ConnectAsClient();
        #endregion /Dynamic UI
    }
}

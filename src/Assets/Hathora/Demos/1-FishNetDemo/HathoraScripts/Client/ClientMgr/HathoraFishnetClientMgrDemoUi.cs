// Created by dylan@hathora.dev

using System.Text.RegularExpressions;
using Hathora.Core.Scripts.Runtime.Common.Utils;
using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hathora.Demos._1_FishNetDemo.HathoraScripts.Client.ClientMgr
{
    /// <summary>
    /// Handles the non-Player UI so we can keep the logic separate.
    /// - Generally, this is going to be pre-connection UI such as create/join lobbies.
    /// - UI OnEvent entry points from Buttons start here.
    /// - This particular child should be used for FishNet.
    /// </summary>
    public class HathoraFishnetClientMgrDemoUi : HathoraClientMgrDemoUi, IHathoraNetClientMgrUi
    {
        public static HathoraFishnetClientMgrDemoUi Singleton { get; private set; }
        private static HathoraFishnetClientMgr HathoraClientMgr => 
            HathoraFishnetClientMgr.Singleton;
        

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
                Debug.LogError("[HathoraFishnetClientMgrUi]**ERR @ SetSingleton: " +
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
            HathoraClientMgr.StartServer();
        }

        /// <param name="_hostPortOverride">
        /// Normally passes the host:port provided by Hathora, but FishNet
        /// specifically gets it from the Ui.clientConnectInputField
        /// </param>
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
            _ = HathoraClientMgr.StartHost();
        }

        public override void OnStopServerBtnClick()
        {
            base.OnStopServerBtnClick();
            HathoraClientMgr.StopServer();
        }
        
        public override void OnStopClientBtnClick()
        {
            base.OnStopClientBtnClick();
            HathoraClientMgr.StopClient();
        }
        
        public override void OnStopHostBtnClick()
        {
            base.OnStopHostBtnClick();
            HathoraClientMgr.StopHost();
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

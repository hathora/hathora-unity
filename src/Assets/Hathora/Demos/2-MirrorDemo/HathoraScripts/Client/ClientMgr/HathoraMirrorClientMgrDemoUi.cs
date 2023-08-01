// Created by dylan@hathora.dev

using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using Mirror;
using UnityEngine;

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
            base.OnStartClientBtnClick();
            NetworkManager.singleton.StartClient();
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

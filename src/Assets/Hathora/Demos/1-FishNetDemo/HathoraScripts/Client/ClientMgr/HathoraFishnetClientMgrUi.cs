// Created by dylan@hathora.dev

using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using UnityEngine;

namespace Hathora.Demos._1_FishNetDemo.HathoraScripts.Client.ClientMgr
{
    /// <summary>
    /// Handles the non-Player UI so we can keep the logic separate.
    /// - Generally, this is going to be pre-connection UI such as create/join lobbies.
    /// - UI OnEvent entry points from Buttons start here.
    /// - This particular child should be used for both FishNet.
    /// </summary>
    public class HathoraFishnetClientMgrUi : HathoraNetClientMgrUiBase, IHathoraNetClientMgrUi
    {
        public static HathoraFishnetClientMgrUi Singleton { get; private set; }
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

        public override void OnStartClientBtnClick()
        {
            base.OnStartClientBtnClick();
            HathoraClientMgr.StartClient();
        }

        public override void OnStartHostBtnClick()
        {
            base.OnStartHostBtnClick();
            HathoraClientMgr.StartHost();
        }

        public override void OnStopServerBtnClick()
        {
            base.OnStopServerBtnClick();
            HathoraClientMgr.StopServer(_sendDisconnectMsgToClients: true);
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
            HathoraClientMgr.Connect();
        #endregion /Dynamic UI
    }
}

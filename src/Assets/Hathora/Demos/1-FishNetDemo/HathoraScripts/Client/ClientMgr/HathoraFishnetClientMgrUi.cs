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
    public class HathoraFishnetClientMgrUi : HathoraNetClientMgrUi, IHathoraNetClientMgrUi
    {
        public static HathoraFishnetClientMgrUi Singleton { get; private set; }
        private static HathoraFishnetClient hathoraClient => 
            HathoraFishnetClient.Singleton;
        

        #region Init
        protected override void OnAwake()
        {
            base.OnAwake();
            InitOnAwake(hathoraClient);
            SetSingleton();
        }

        public HathoraNetClientMgrUiBaseContainer ui { get; set; }

        public void SetSingleton()
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
            hathoraClient.StartServer();
        }
        
        public override void OnStartClientBtnClick()
        {
            base.OnStartClientBtnClick();
            hathoraClient.StartClient();
        }

        public override void OnStartHostBtnClick()
        {
            base.OnStartHostBtnClick();
            hathoraClient.StartHost();
        }

        public override void OnStopServerBtnClick()
        {
            base.OnStopServerBtnClick();
            hathoraClient.StopServer(_sendDisconnectMsgToClients: true);
        }

        public override void OnJoinLobbyAsClientBtnClick()
        {
            base.OnJoinLobbyAsClientBtnClick();
            Connect();
        }

        public void Connect() => 
            hathoraClient.Connect();
        #endregion /Dynamic UI
    }
}

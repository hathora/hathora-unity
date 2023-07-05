// Created by dylan@hathora.dev

using Hathora.Demos.Shared.Scripts.Client.ClientMgr;
using UnityEngine;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Client.ClientMgr
{
    /// <summary>
    /// Handles the non-Player UI so we can keep the logic separate.
    /// - Generally, this is going to be pre-connection UI such as create/join lobbies.
    /// - UI OnEvent entry points from Buttons start here.
    /// - This particular child should be used for both Mirror.
    /// </summary>
    public class HathoraMirrorClientMgrUi : HathoraNetClientMgrUiBase, IHathoraNetClientMgrUi
    {
        public static HathoraMirrorClientMgrUi Singleton { get; private set; }
        private static HathoraMirrorClient hathoraClient => 
            HathoraMirrorClient.Singleton;
        

        #region Init
        protected override void OnStart()
        {
            base.OnStart();
            InitOnStart(hathoraClient);
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

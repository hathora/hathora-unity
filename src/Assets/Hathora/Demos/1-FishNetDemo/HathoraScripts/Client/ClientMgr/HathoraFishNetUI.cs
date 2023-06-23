// Created by dylan@hathora.dev

using Hathora.Demos._1_FishNetDemo.HathoraScripts.Client.ClientMgr;
using UnityEngine;

namespace Hathora.Demos.Shared.Scripts.Client
{
    /// <summary>
    /// Handles the non-Player UI so we can keep the logic separate.
    /// Generally, this is going to be pre-connection UI such as create/join lobbies.
    /// UI OnEvent entry points from Buttons start here.
    /// </summary>
    public class HathoraFishNetUI : HathoraNetUiBase
    {
        public static HathoraFishNetUI Singleton { get; private set; }
        private static HathoraFishnetClient hathoraClient => 
            HathoraFishnetClient.Singleton;
        

        #region Init
        private void Awake() =>
            base.InitOnAwake(hathoraClient);
        
        protected override void SetSingleton()
        {
            if (Singleton != null)
            {
                Debug.LogError("[HathoraFishNetUI]**ERR @ setSingleton: Destroying dupe");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }
        #endregion // Init
        
        
        #region UI Interactions
        protected override void Connect() => 
            hathoraClient.Connect();
        #endregion /Dynamic UI
    }
}

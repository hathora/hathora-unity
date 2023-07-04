// Created by dylan@hathora.dev

using UnityEngine;

namespace Hathora.Demos.Shared.Scripts.Client.ClientMgr
{
    public interface IHathoraNetClientMgrUi
    {
        [SerializeField]
        HathoraNetClientMgrUiBaseContainer ui { get; set; }
        
        /// <summary>Call after OnAwake</summary>
        void SetSingleton();
        
        /// <summary>Call after OnJoinLobbyAsClientBtnClick</summary>
        void Connect();
    }
}

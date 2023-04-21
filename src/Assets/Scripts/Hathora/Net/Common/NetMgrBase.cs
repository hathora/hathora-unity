// Created by dylan@hathora.dev

using Hathora.Net.Client;
using Hathora.Net.Server;
using Unity.Netcode;
using UnityEngine;

namespace Hathora.Net.Common
{
    public abstract class NetMgrBase : MonoBehaviour
    {
        [SerializeField]
        protected NetUI NetUi;
    
        protected NetworkManager s_NetMgr => NetworkManager.Singleton;
        protected NetServerMgr s_ServerMgr => NetServerMgr.Singleton;
        protected NetClientMgr s_ClientMgr => NetClientMgr.Singleton;
        protected NetCommonMgr s_NetCommonMgr => NetCommonMgr.Singleton;
    }
}

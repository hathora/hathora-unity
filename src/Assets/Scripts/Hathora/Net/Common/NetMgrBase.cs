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
    
        protected static NetworkManager s_NetMgr => NetworkManager.Singleton;
        protected static NetServerMgr s_ServerMgr => NetServerMgr.Singleton;
        protected static NetClientMgr s_ClientMgr => NetClientMgr.Singleton;
        protected static NetCommonMgr s_NetCommonMgr => NetCommonMgr.Singleton;
    }
}

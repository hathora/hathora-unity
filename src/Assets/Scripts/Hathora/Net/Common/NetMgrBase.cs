// Created by dylan@hathora.dev

using Unity.Netcode;
using UnityEngine;

namespace Hathora.Net.Common
{
    public abstract class NetMgrBase : MonoBehaviour
    {
        [SerializeField]
        protected NetUI NetUi;
    
        protected NetworkManager NetMgr => NetworkManager.Singleton;
    }
}

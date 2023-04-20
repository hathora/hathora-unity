// Created by dylan@hathora.dev

namespace Hathora.Net.Common
{
    public class NetCommonMgr : NetMgrBase
    {
        public enum NetMode
        {
            None,
            Server,
            Client,
        }
        
        public void Disconnect()
        {
            NetMgr.Shutdown(discardMessageQueue:true);
            NetUi.ToggleLobbyUi(show:true, NetMode.None);
            UnityEngine.Debug.Log("[NetCommonMgr] Disconnected");
        }
    }
}

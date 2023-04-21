// Created by dylan@hathora.dev

namespace Hathora.Net.Common
{
    public class NetCommonMgr : NetMgrBase
    {
        public static NetCommonMgr Singleton;

        private void Start() => setSingleton();
        
        private void setSingleton()
        {
            if (Singleton != null)
                Destroy(gameObject);

            Singleton = this;
        }
        
        public enum NetMode
        {
            None,
            Server,
            Client,
            Host,
        }
        
        public void Disconnect()
        {
            s_NetMgr.Shutdown(discardMessageQueue:true);
            NetUi.ToggleLobbyUi(show:true, NetMode.None);
            UnityEngine.Debug.Log("[NetCommonMgr] Disconnected");
        }

        /// <summary>
        /// Useful for local testing with a single instance.
        /// </summary>
        public void JoinAsHost()
        {
            UnityEngine.Debug.Log("[NetServerMgr] @ HostAsServer - Finding Server...");
            s_NetMgr.StartHost();
            NetUi.ToggleLobbyUi(show:false, NetMode.Host);
        }
    }
}

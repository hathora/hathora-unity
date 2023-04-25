// Created by dylan@hathora.dev

using FishNet;
namespace Hathora.Net.Common
{
    public class NetCommonMgr : NetBehaviourBase
    {
        public static NetCommonMgr Singleton;

        private void Awake() => 
            setSingleton();
        
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
            shutdown();
            s_netUi.ToggleLobbyUi(show:true, NetMode.None);
            UnityEngine.Debug.Log("[NetCommonMgr] Network shutdown/Disconnected");
        }
        
        private void shutdown()
        {
            UnityEngine.Debug.Log("[NetCommonMgr] Shutting down network ...");
            if (base.IsClient)
                InstanceFinder.ClientManager.StopConnection();
            if (base.IsServer || base.IsHost)
                InstanceFinder.ServerManager.StopConnection(true);
        }

        /// <summary>
        /// Useful for local testing with a single instance.
        /// </summary>
        public void JoinAsHost()
        {
            UnityEngine.Debug.Log("[NetServerMgr] @ HostAsServer - Finding Server...");
            
            s_ServerMgr.HostAsServer();
            s_ClientMgr.JoinAsClient();
            
            s_netUi.ToggleLobbyUi(show:false, NetMode.Host);
        }
    }
}

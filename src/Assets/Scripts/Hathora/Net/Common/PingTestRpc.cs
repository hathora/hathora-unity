// Created by dylan@hathora.dev
// Unity NGO: Getting Started | https://docs-multiplayer.unity3d.com/netcode/current/tutorials/get-started-ngo

using FishNet.Object;
using Hathora.Net.Server;
using UnityEngine;

namespace Hathora.Net.Common
{
    /// <summary>
    /// As soon as we connect to the server, we send a ping to the server.
    /// => Server will pong us back.
    /// </summary>
    public class PingTestRpc : NetworkBehaviour
    {
        private static NetServerMgr s_serverMgr => NetServerMgr.Singleton;
        private int numTimesRpcdToServer;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                TestPingToServer();
        }

        public void TestPingToServer()
        {
            string msg = $"Ping #{numTimesRpcdToServer++}!";
            Debug.Log($"[PingTestRpc] Sending test ping server == '{msg}'");
            
            s_serverMgr.SendMsgServerRpc(msg);   
        }
    }
}

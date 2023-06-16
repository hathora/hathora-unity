// Created by dylan@hathora.dev
// Unity NGO: Getting Started | https://docs-multiplayer.unity3d.com/netcode/current/tutorials/get-started-ngo

using FishNet.Object;
using UnityEngine;

namespace Hathora.Demos._1_FishNetDemo.Scripts.Client.Player
{
    /// <summary>
    /// As soon as we connect to the server, we send a ping to the server.
    /// => Server will pong us back.
    /// </summary>
    public class NetPingTestRpc : NetworkBehaviour
    {
        private int _numTimesRpcdToServer;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                TestPingToServer();
        }

        public void TestPingToServer()
        {
            string msg = $"Ping #{_numTimesRpcdToServer++}!";
            Debug.Log($"[NetPingTestRpc] Sending test ping server == '{msg}'");
            SendMsgServerRpc(msg);   
        }
        
        /// <summary>
        /// Send a msg to the server from an observer.
        /// </summary>
        /// <param name="msg">Arbitrary string</param>
        [ServerRpc]
        public void SendMsgServerRpc(string msg)
        {
            Debug.Log($"[NetPingTestRpc] SendMsgServerRpc: Received msg on server (from observed client) == '{msg}'");
            SendMsgObserversRpc(msg); // Ask server to send the msg back to all observers.
        }
        
        /// <summary>
        /// Send a msg to ALL observers.
        /// </summary>
        /// <param name="msg">Arbitrary string</param>
        // [ServerRpc(RequireOwnership = true)]
        [ObserversRpc]
        public void SendMsgObserversRpc(string msg) =>
            Debug.Log($"[NetPingTestRpc] SendMsgObserversRpc: Received on observer (from server) == '{msg}'");
    }
}

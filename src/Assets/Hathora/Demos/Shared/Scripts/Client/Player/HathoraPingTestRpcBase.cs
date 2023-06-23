// Created by dylan@hathora.dev

using FishNet.Object;
using Mirror;
using UnityEngine;
using NetworkBehaviour = FishNet.Object.NetworkBehaviour;

namespace Hathora.Demos.Shared.Scripts.Client.Player
{
    /// <summary>
    /// As soon as we connect to the server, we send a ping to the server => Server will pong us back.
    /// - Mirror NetworkBehaviour Doc | https://mirror-networking.gitbook.io/docs/manual/components/networkbehaviour 
    /// </summary>
    public class HathoraMirrorPingTestRpc : NetworkBehaviour
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
            Debug.Log($"[HathoraMirrorPingTestRpc] Sending test ping server == '{msg}'");
            SendMsgServerRpc(msg);   
        }
        
        /// <summary>
        /// Send a msg to the server from an observer.
        /// </summary>
        /// <param name="msg">Arbitrary string</param>
        [Command]
        public void CmdSendMsgServerRpc(string msg)
        {
            Debug.Log($"[HathoraMirrorPingTestRpc] SendMsgServerRpc: Received msg on server (from observed client) == '{msg}'");
            SendMsgObserversRpc(msg); // Ask server to send the msg back to all observers.
        }
        
        
        /// <summary>
        /// Send a msg to the server from an observer.
        /// </summary>
        /// <param name="msg">Arbitrary string</param>
        [ServerRpc]
        public void SendMsgServerRpc(string msg)
        {
            Debug.Log($"[HathoraMirrorPingTestRpc] SendMsgServerRpc: Received msg on server (from observed client) == '{msg}'");
            SendMsgObserversRpc(msg); // Ask server to send the msg back to all observers.
        }
        
        /// <summary>
        /// Send a msg to ALL observers.
        /// </summary>
        /// <param name="msg">Arbitrary string</param>
        // [ServerRpc(RequireOwnership = true)]
        [ObserversRpc]
        public void SendMsgObserversRpc(string msg) =>
            Debug.Log($"[HathoraMirrorPingTestRpc] SendMsgObserversRpc: Received on observer (from server) == '{msg}'");
    }
}

// Created by dylan@hathora.dev

using FishNet.Object;
using UnityEngine;

namespace Hathora.Demos._1_FishNetDemo.HathoraScripts.Client.Player
{
    /// <summary>
    /// As soon as we connect to the server, we send a ping to the server => Server will pong us back.
    /// FishNet NetworkBehaviour Doc | https://fish-networking.gitbook.io/docs/manual/guides/network-behaviour-guides  
    /// </summary>
    public class HathoraFishnetPingTestRpc : NetworkBehaviour
    {
        // TODO: Make syncvar?
        private int numTimesRpcdToServer;

        private static bool pressedInput_R() => 
            Input.GetKeyDown(KeyCode.R);
        
        private void Update()
        {
            if (pressedInput_R())
                pingServer();
        }

        private void pingServer()
        {
            string msg = $"Ping #{numTimesRpcdToServer++}!";
            Debug.Log("[HathoraFishnetPingTestRpc.pingServer] " +
                $"Sending msg: '{msg}' to server");
            
            RpcSendMsgToServer(msg);   
        }

        /// <summary>Send a msg to the server from an observer (client).</summary>
        /// <param name="_msgFromClient">Arbitrary string</param>
        [ServerRpc]
        public void RpcSendMsgToServer(string _msgFromClient) =>
            ServerSendMsgToClient(_msgFromClient);

        /// <summary>Send a msg from Server to Client.</summary>
        /// <param name="_msgFromClient"></param>
        [Server]
        public void ServerSendMsgToClient(string _msgFromClient)
        {
            Debug.Log("[HathoraFishnetPingTestRpc.ServerSendMsgToClient] Server received " +
                $"msgFromClient: '{_msgFromClient}'");
            
            RpcSendMsgToClientObservers(_msgFromClient); // Ask server to send the msg back to all observers.
        }

        /// <summary>Send a msg from server to ALL observers (clients).</summary>
        /// <param name="_msgFromServer">Arbitrary string</param>
        [ObserversRpc]
        public void RpcSendMsgToClientObservers(string _msgFromServer)
        {
            Debug.Log("[HathoraFishnetPingTestRpc.RpcSendMsgToClientObservers] Client received " +
                $"msgFromServer: '{_msgFromServer}'");
        }
    }
}

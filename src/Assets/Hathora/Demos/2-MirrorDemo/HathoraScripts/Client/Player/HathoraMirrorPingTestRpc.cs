// Created by dylan@hathora.dev

using Mirror;
using UnityEngine;
using NetworkBehaviour = Mirror.NetworkBehaviour;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Client.Player
{
    /// <summary>
    /// As soon as we connect to the server, we send a ping to the server => Server will pong us back.
    /// Mirror NetworkBehaviour Doc | https://mirror-networking.gitbook.io/docs/manual/components/networkbehaviour  
    /// </summary>
    public class HathoraMirrorPingTestRpc : NetworkBehaviour
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
            Debug.Log("[HathoraMirrorPingTestRpc.pingServer] " +
                $"Sending msg: '{msg}' to server");
            
            RpcSendMsgToServer(msg);   
        }

        /// <summary>Send a msg to the server from an observer (client).</summary>
        /// <param name="_msgFromClient">Arbitrary string</param>
        [Command]
        public void RpcSendMsgToServer(string _msgFromClient) => 
            ServerSendMsgToClient(_msgFromClient);

        /// <summary>Send a msg from Server to Client.</summary>
        /// <param name="_msgFromClient"></param>
        [Server]
        public void ServerSendMsgToClient(string _msgFromClient)
        {
            Debug.Log("[HathoraMirrorPingTestRpc.ServerSendMsgToClient] Server received " +
                $"msgFromClient: '{_msgFromClient}'");

            RpcSendMsgToClientObservers(_msgFromClient); // Ask server to send the msg back to all observers.        }
        }

        /// <summary>Send a msg from server to ALL observers (clients).</summary>
        /// <param name="_msgFromServer">Arbitrary string</param>
        [Command]
        public void RpcSendMsgToClientObservers(string _msgFromServer)
        {
            Debug.Log($"[HathoraMirrorPingTestRpc.RpcSendMsgToClientObservers] Client received " +
                $"msgFromServer: '{_msgFromServer}'");
        }
    }
}

// Created by dylan@hathora.dev
// Unity NGO: Getting Started | https://docs-multiplayer.unity3d.com/netcode/current/tutorials/get-started-ngo

using Hathora.Net.Client;
using Hathora.Net.Server;
using Unity.Netcode;
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
        private static NetClientMgr s_clientMgr => NetClientMgr.Singleton;
        private static NetworkManager NetMgr => NetworkManager.Singleton;
        private int numTimesRpcd = 0;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            // (!) Clients will always *start* as owners until Server assigns ownership
            // a few moments after the Server kicks in.
            if (!IsOwningClientOrHost())
                return;
             
            // Owning (local) client (!server) should ping the server
            // => server should pong the client back.
            TestServerPing();
        }

        /// <summary>
        /// Only send RPCs to the server on the client that owns the
        /// NetworkObject that owns this NetworkBehaviour instance.
        /// - a host is both a client *and* server.
        /// </summary>
        /// <returns></returns>
        private bool IsOwningClientOrHost() =>
            (IsClient || IsHost) && IsOwner;

        public void TestServerPing() =>
            s_serverMgr.TestServerRpc(numTimesRpcd++, NetworkObjectId);

        public void TestClientPing() =>
            s_clientMgr.TestClientRpc(numTimesRpcd++, NetworkObjectId);
    }
}

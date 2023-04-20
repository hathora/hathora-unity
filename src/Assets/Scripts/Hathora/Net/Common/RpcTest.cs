// Created by dylan@hathora.dev
// Unity NGO: Getting Started | https://docs-multiplayer.unity3d.com/netcode/current/tutorials/get-started-ngo

using Unity.Netcode;
using UnityEngine;

namespace Hathora.Net
{
    public class RpcTest : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            Debug.Log($"[RpcTest] OnNetworkSpawn");

            // Only send an RPC to the server on the client that owns the
            // NetworkObject that owns this NetworkBehaviour instance
            if (IsServer || !IsOwner)
                return;

            const int testVal = 0;
            TestServerRpc(testVal, NetworkObjectId);
        }

        [ClientRpc]
        void TestClientRpc(int val, ulong srcNetObjectId)
        {
            Debug.Log($"[RpcTest] TestClientRpc: Client Received RPC #{val++} on NetObj #{srcNetObjectId}");
        }

        [ServerRpc]
        void TestServerRpc(int val, ulong srcNetObjId)
        {
            Debug.Log($"[RpcTest] TestServerRpc: Server Received RPC #{val} on NetObj #{srcNetObjId}");
            TestClientRpc(val, srcNetObjId);
        }
    }
}

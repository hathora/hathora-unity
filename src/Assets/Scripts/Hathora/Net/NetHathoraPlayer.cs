// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using FishNet.Object;
using UnityEngine;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Hathora.Net
{
    /// <summary>
    /// Helpers for the NetworkPlayer. Since NetworkPlayer spawns dynamically
    /// </summary>
    public class NetHathoraPlayer : NetworkBehaviour
    {
        #region Serialized Fields
        // ---------------------------------------------------
        [Header("Hathora Config (TODO: Move to ScriptableObj")]
        [SerializeField, Tooltip("Required")]
        private string appId;
        
        [SerializeField, Tooltip("Required")]
        private string authToken;

        [SerializeField, Tooltip("Required (for Rooms/Lobbies)")]
        private Region region = Region.Seattle;
        
        // ---------------------------------------------------
        [Header("CreateLobbyRequest (Demo)")]
        [SerializeField, Tooltip("Optional")]
        private CreateLobbyRequest.VisibilityEnum lobbyVisibility = 
            CreateLobbyRequest.VisibilityEnum.Public;
        
        // ---------------------------------------------------
        [Header("CreateRoomRequest (Demo)")]
        [SerializeField, Tooltip("Optional - Recommended leave empty")]
        private string roomId;

        //// TODO: For this, we need to break it up and build our own Dictionary. 
        // [SerializeField, Tooltip("Optional - Initial state kvp")]
        // private Dictionary<string, string> roomInitConfig;
        #endregion // Serialized Fields
        

        private RoomV1Api roomApi;
        private LobbyV2Api lobbyApi;
        private AuthV1Api authApi;
        
        private void Start() =>
            NetworkSpawnLogs();

        private void NetworkSpawnLogs()
        {
            Debug.Log($"[NetHathoraPlayer] OnNetworkSpawn, id==={NetworkObject.ObjectId}");
            
            if (base.IsHost)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on host (server+client)");
            else if (base.IsServerOnly)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on server");
            else if (base.IsClient)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on client");

            initHathoraSdk();
        }
        
        private void initHathoraSdk()
        {
            Configuration hathoraConfig = new Configuration();
            authApi = new AuthV1Api(hathoraConfig);
            roomApi = new RoomV1Api(hathoraConfig);
            lobbyApi = new LobbyV2Api(hathoraConfig);
        }
        
        public void OnAuthBtnClick() => 
            authServerRpc();

        /// <summary>
        /// Auths with Hathora login server
        /// </summary>
        /// <returns>room name str</returns>
        [ServerRpc]
        private void authServerRpc() => 
            serverAuthAsync();
        
        private async Task serverAuthAsync()
        {
            // Client calls server
            if (base.IsServerOnly)
                return;

            try
            {
                LoginAnonymous200Response res = await authApi.LoginAnonymousAsync(appId);
                Debug.Log($"[NetHathoraPlayer] Login result: {!string.IsNullOrEmpty(res.Token)}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[NetHathoraPlayer]**ERR @ serverAuthAsync (LoginAnonymousAsync): // e=={e}");
                await Task.FromException<Exception>(e);
                return;
            }
        }

        public void OnCreateRoomBtnClick() => 
            createRoomServerRpc();

        /// <summary>
        /// Server creates a room
        /// </summary>
        /// <returns>room name str</returns>
        [ServerRpc]
        private void createRoomServerRpc() =>
            serverCreateRoomAsync();   

        /// <summary>
        /// Calls onCreateRoomObserverRpc
        /// </summary>
        private async Task serverCreateRoomAsync()
        {
            // Client calls server
            if (base.IsServerOnly)
                return;

            CreateRoomRequest request = new CreateRoomRequest(region);
            // request.AdditionalProperties = TODO;
            
            string roomName = await roomApi.CreateRoomAsync(appId, request, CancellationToken.None);
            Debug.Log($"[NetHathoraPlayer] SERVER serverCreateRoomAsync => returned: {roomName}");
            
            onCreateRoomObserverRpc(roomName);
        }

        private async Task<Lobby> serverCreateLobbyAsync()
        {
            // Client calls server
            if (base.IsServerOnly)
                return null;

            CreateLobbyRequest request = new CreateLobbyRequest(
                lobbyVisibility, 
                initialConfig:null, 
                region);
            
            Lobby lobby = await lobbyApi.CreateLobbyAsync(
                appId,
                authToken, 
                request, 
                roomId, 
                CancellationToken.None);

            return lobby;
        }

        #region TODO: #if UNITY_SERVER
        [ObserversRpc]
        private void onCreateRoomObserverRpc(string roomName) =>
            Debug.Log($"[NetHathoraPlayer] OBSERVER onCreateRoomObserverRpc => roomId: {roomName}");
        #endregion // TODO: #if UNITY_SERVER
    }
}

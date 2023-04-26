// Created by dylan@hathora.dev

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FishNet.Object;
using UnityEngine;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using UnityEngine.Serialization;

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
        
        private void Start() =>
            NetworkSpawnLogs();

        private void NetworkSpawnLogs()
        {
            Debug.Log($"[NetHathoraPlayer] OnNetworkSpawn, id==={NetworkObject.ObjectId}");
            
            if (base.IsHost)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on host (server+client)");
            else if (base.IsServer)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on server");
            else if (base.IsClient)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on client");

            initHathoraSdk();
        }
        
        private void initHathoraSdk()
        {
            Configuration hathoraConfig = new Configuration();
            roomApi = new RoomV1Api(hathoraConfig);
            lobbyApi = new LobbyV2Api(hathoraConfig);
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
            if (!base.IsServer)
                return;

            CreateRoomRequest request = new CreateRoomRequest(region);
            // request.AdditionalProperties = TODO;
            
            string roomName = await roomApi.CreateRoomAsync(appId, request, CancellationToken.None);
            Debug.Log($"[NetHathoraPlayer] SERVER serverCreateRoomAsync => returned: {roomName}");
            
            onCreateRoomObserverRpc(roomName);
        }

        private async Task<Lobby> serverCreateLobbyAsync()
        {
            if (!base.IsServer)
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

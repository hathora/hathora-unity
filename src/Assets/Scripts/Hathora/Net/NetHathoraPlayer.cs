// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Hathora.Net
{
    /// <summary>
    /// Helpers for the NetworkPlayer. Since NetworkPlayer spawns dynamically.
    /// TODO: Add `#if UNITY_SERVER` for ObserverRpc's.
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
        
        // ---------------------------------------------------
        [Header("Misc/UI")]
        [SerializeField]
        private NetPlayerUI netPlayerUI;

        private NetworkConnection GetLocalClientConnection() =>
            InstanceFinder.ClientManager.Clients[0];

        //// TODO: For this, we need to break it up and build our own Dictionary. 
        // [SerializeField, Tooltip("Optional - Initial state kvp")]
        // private Dictionary<string, string> roomInitConfig;
        #endregion // Serialized Fields

        
        private Configuration hathoraConfig;
        private RoomV1Api roomApi;
        private LobbyV2Api lobbyApi;
        private AuthV1Api authApi;
        
        private static NetUI netUi => NetUI.Singleton;
        
        /// <summary>
        /// The Server sets this, then auto-syncs with Client.
        /// </summary>
        [SerializeField]
        private NetSession playerSession;

        #region Init
        /// <summary>
        /// Better to use this instead of Start, in most situations.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!base.IsOwner)
                return;
            
            NetworkSpawnLogs();
            initHathoraSdkPreAuth();
            netUi.SetLocalNetPlayerUI(netPlayerUI);
        }

        private void NetworkSpawnLogs()
        {
            Debug.Log($"[NetHathoraPlayer] OnNetworkSpawn, id==={NetworkObject.ObjectId}");
            
            if (base.IsHost)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on host (server+client)");
            else if (base.IsServerOnly)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on server");
            else if (base.IsClient)
                Debug.Log("[NetHathoraPlayer] OnNetworkSpawn called on client");
        }
        
        private void initHathoraSdkPreAuth()
        {
            // Only init default config and AuthApi, to start; more post-auth.
            hathoraConfig = new Configuration();
            authApi = new AuthV1Api(hathoraConfig);
        }

        private void serverInitHathoraSdkPostAuth(string authToken)
        {
            if (!IsServer)
                return;

            hathoraConfig.AccessToken = authToken;
            // hathoraConfig.AddApiKeyPrefix("Authorization", "Bearer");
            // hathoraConfig.AddApiKey("Authorization", authToken);

            // Init the remaining APIs with the new Auth header
            roomApi = new RoomV1Api(hathoraConfig);
            lobbyApi = new LobbyV2Api(hathoraConfig);
        }
        #endregion // Init

        
        #region UI Interactions
        public void OnAuthBtnClick() => 
            authServerRpc();
        
        public void OnCreateRoomBtnClick() => 
            createRoomServerRpc();
        
        public void OnCreateLobbyBtnClick() => 
            createLobbyServerRpc();
        #endregion // UI Interactions


        #region Server RPCs (from Observer to Server)
        /// <summary>
        /// Auths with Hathora login server
        /// </summary>
        /// <returns>room name str</returns>
        [ServerRpc]
        private void authServerRpc() => 
            serverAuthAsync();
        
        /// <summary>
        /// Server creates a room
        /// </summary>
        /// <returns>room name str</returns>
        [ServerRpc]
        private void createRoomServerRpc() =>
            serverCreateRoomAsync();
        
        [ServerRpc]
        private void createLobbyServerRpc() =>
            throw new NotImplementedException();
        #endregion // Server RPCs (from Observer to Server)
        

        #region Observer RPCs (from Server to ALL Observers)
        [ObserversRpc]
        private void onCreateRoomObserverRpc(string roomName) =>
            Debug.Log($"[NetHathoraPlayer] OBSERVER onCreateRoomObserverRpc => roomId: {roomName}");
        #endregion // Observer RPCs (from Server to ALL Observers)
        
        
        #region Async Hathora SDK Calls
        private async Task serverAuthAsync()
        {
            if (!base.IsServer)
                return;

            LoginAnonymous200Response anonLoginResult;
            try
            {
                netPlayerUI.SetShowAuthTxt("<color=yellow>Logging in...</color>");
                anonLoginResult = await authApi.LoginAnonymousAsync(appId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraPlayer]**ERR @ serverAuthAsync (LoginAnonymousAsync): {e.Message}");
                onServerAuthFail();
                await Task.FromException<Exception>(e);
                return;
            }

            bool isAuthed = !string.IsNullOrEmpty(anonLoginResult?.Token); 
            Debug.Log($"[NetHathoraPlayer] isAuthed: {isAuthed}");
            if (isAuthed)
                onServerAuthSuccess(anonLoginResult?.Token);
            else
                onServerAuthFail();
        }

        /// <summary>
        /// SERVER ONLY.
        /// When done, calls onCreateRoomObserverRpc.
        /// </summary>
        private async Task serverCreateRoomAsync()
        {
            if (!base.IsServer)
                return;

            CreateRoomRequest request = new CreateRoomRequest(region);

            string roomName;
            try
            {
                roomName = await roomApi.CreateRoomAsync(appId, request, CancellationToken.None);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraPlayer]**ERR @ serverCreateRoomAsync (CreateRoomAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return;
            }

            Debug.Log($"[NetHathoraPlayer] SERVER serverCreateRoomAsync => returned: {roomName}");
            
            bool createdRoom = !string.IsNullOrEmpty(roomName);
            if (createdRoom)
                onServerCreateRoomSuccess(roomName);
        }

        private async Task serverCreateLobbyAsync()
        {
            // Client calls server
            if (!base.IsServer)
                return;
            
            CreateLobbyRequest request = new CreateLobbyRequest(
                lobbyVisibility, 
                initialConfig:null, 
                region);

            Lobby lobby;
            try
            {
                lobby = await lobbyApi.CreateLobbyAsync(
                    appId,
                    authToken, 
                    request, 
                    roomId, 
                    CancellationToken.None);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraPlayer]**ERR @ serverCreateLobbyAsync (CreateLobbyAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return;
            }

            if (lobby != null)
                onServerCreateLobbySuccess();
        }
        #endregion // Async Hathora SDK Calls

        
        #region Success Callbacks
        void onServerAuthSuccess(string authToken)
        {
            netPlayerUI.OnLoggedIn();
            playerSession.InitNetSession(authToken);
            serverInitHathoraSdkPostAuth(authToken);
        }
        
        void onServerCreateRoomSuccess(string roomName)
        {
            playerSession.CachedRoomName = roomName;
            netPlayerUI.OnCreatedRoom(roomName);
        }
        
        private void onServerCreateLobbySuccess()
        {
            throw new NotImplementedException();
        }
        #endregion // Success Callbacks
        
        
        #region Fail Callbacks
        private void onServerAuthFail() =>
            netPlayerUI.SetShowAuthTxt("<color=red>Login Failed</color>");
        #endregion //Fail Callbacks
    }
}

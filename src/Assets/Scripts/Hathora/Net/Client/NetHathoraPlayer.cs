// Created by dylan@hathora.dev

using FishNet.Object;
using FishNet.Object.Synchronizing;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Server;
using UnityEngine;

namespace Hathora.Net.Client
{
    /// <summary>
    /// Helpers for the NetworkPlayer. Since NetworkPlayer spawns dynamically.
    /// </summary>
    public class NetHathoraPlayer : NetworkBehaviour
    {
        #region Serialized Fields
        [SerializeField, Tooltip("Client will have limited access to this")]
        private NetHathoraServer hathoraServer;

        [SerializeField]
        private NetSession playerSession;
        
        [SerializeField]
        private NetPlayerUI netPlayerUI;
        #endregion // Serialized Fields

        
        #region Other Vars
        private static NetUI netUi => NetUI.Singleton;
        #endregion // Other Vars

        
        #region Init
        public override void OnStartServer()
        {
            base.OnStartServer();

#if UNITY_SERVER || DEBUG
            hathoraServer.InitFromPlayer(playerSession);
#endif
        }

        /// <summary>
        /// Better to use this instead of Start, in most situations.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!base.IsOwner)
                return;
            
            netUi.SetLocalNetPlayerUI(netPlayerUI);
            NetworkSpawnLogs();

            // Sub to server events
            hathoraServer.ServerApis.AuthApi.AuthComplete += OnAuthComplete;
            hathoraServer.ServerApis.RoomApi.CreateRoomComplete += OnCreateRoomComplete;
            hathoraServer.ServerApis.LobbyApi.CreateLobbyComplete += OnCreateLobbyComplete;
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
        #endregion // Init

        
        #region UI Interactions
        public void OnAuthBtnClick()
        {
            netPlayerUI.SetShowAuthTxt("<color=yellow>Logging in...</color>");
            authServerRpc();
        }

        public void OnCreateRoomBtnClick()
        {
            // TODO: Create room status txt
            createRoomServerRpc();
        }

        public void OnCreateLobbyBtnClick()
        {
            // TODO: Create lobby status txt
            createLobbyServerRpc();
        } 
        #endregion // UI Interactions


        #region Server RPCs (from Observer to Server)
        /// <summary>
        /// Auths with Hathora login server
        /// </summary>
        /// <returns>room name str</returns>
        [ServerRpc]
        private void authServerRpc()
        {
#if UNITY_SERVER || DEBUG
            hathoraServer.ServerApis.AuthApi.ServerAuthAsync();
#endif
        }

        /// <summary>
        /// Server creates a room
        /// </summary>
        /// <returns>room name str</returns>
        [ServerRpc]
        private void createRoomServerRpc()
        {
#if UNITY_SERVER || DEBUG
            hathoraServer.ServerApis.RoomApi.ServerCreateRoomAsync();   
#endif
        }
        
        [ServerRpc]
        private void createLobbyServerRpc(CreateLobbyRequest.VisibilityEnum lobbyVisibility = 
            CreateLobbyRequest.VisibilityEnum.Public)
        {
#if UNITY_SERVER || DEBUG
            hathoraServer.ServerApis.LobbyApi.ServerCreateLobbyAsync(lobbyVisibility);
#endif
        }
        #endregion // Server RPCs (from Observer to Server)
        

        #region Event Callbacks
        private void OnAuthComplete(object sender, bool isSuccess)
        {
            if (!isSuccess)
            {
                netPlayerUI.SetShowAuthTxt("<color=red>Login Failed</color>");
                return;
            }
            
            netPlayerUI.OnLoggedIn();
        }
        
        private void OnCreateRoomComplete(object sender, string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                // TODO: Create room status txt
                return;
            }
            
            netPlayerUI.OnCreatedRoom(roomName);
        }
        
        private void OnCreateLobbyComplete(object sender, Lobby lobby)
        {
            if (lobby == null)
            {
                // TODO: Create lobby status txt
                return;
            }

            // netPlayerUI.OnCreateLobby(lobby); // TODO
        }
        #endregion // Event Callbacks
        
        
     

        
  
    }
}

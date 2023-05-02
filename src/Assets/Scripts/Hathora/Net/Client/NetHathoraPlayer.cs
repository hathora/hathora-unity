// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using FishNet.Object;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Client.Models;
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
        [SerializeField, Tooltip("Auth, CreateLobby")]
        protected NetHathoraClient hathoraClient;

        [SerializeField]
        protected NetSession playerSession;
        
        [SerializeField]
        protected NetPlayerUI netPlayerUI;
        #endregion // Serialized Fields
    

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

            // Sub to events from client
            hathoraClient.Init(playerSession, netPlayerUI);
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
        public async void OnAuthBtnClick()
        {
            netPlayerUI.SetShowAuthTxt("<color=yellow>Logging in...</color>");
            AuthResult result = await hathoraClient.ClientApis.AuthApi.ClientAuthAsync();
            OnAuthComplete(result.IsSuccess);
        }

        public async void OnCreateLobbyBtnClick()
        {
            netPlayerUI.SetShowLobbyTxt("<color=yellow>Creating Lobby...</color>");
            
            // TODO: Consider `Local` visibility.
            const CreateLobbyRequest.VisibilityEnum visibility = CreateLobbyRequest.VisibilityEnum.Public;
            Lobby lobby = await hathoraClient.ClientApis.LobbyApi.ClientCreateLobbyAsync(visibility);
            
            OnCreateLobbyComplete(lobby.RoomId);
        }
        
        /// <summary>
        /// The player pressed ENTER || unfocused the roomId input.
        /// </summary>
        public async void OnJoinLobbyInputEnd()
        {
            string roomIdInputStr = netPlayerUI.OnJoinLobbyBtnClickGetRoomId();
            throw new NotImplementedException("TODO");
            // await hathoraClient.ClientApis.LobbyApi.ClientJoinLobbyAsync(roomIdInputStr);
        }
        
        public async void OnViewLobbiesBtnClick()
        {
            netPlayerUI.SetShowLobbyTxt("<color=yellow>Getting Lobby List...</color>");
            throw new NotImplementedException("TODO");
            // await hathoraClient.ClientApis.LobbyApi.ClientViewLobbiesAsync(roomIdInputStr);
        }
        #endregion // UI Interactions
        
        #region Callbacks
        private void OnAuthComplete(bool isSuccess)
        {
            if (!isSuccess)
            {
                netPlayerUI.SetShowAuthTxt("<color=red>Login Failed</color>");
                return;
            }
            
            netPlayerUI.OnLoggedIn();
        }
        
        private void OnCreateLobbyComplete(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                netPlayerUI.SetShowLobbyTxt("<color=red>Create Lobby Failed</color>");
                return;
            }

            netPlayerUI.OnJoinedOrCreatedLobby(roomId);
        }
        #endregion
    }
}

// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
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

            AuthResult result = null;
           try
            {
                result = await hathoraClient.ClientApis.AuthApi.ClientAuthAsync();
            }
            catch (Exception e)
            {
                OnAuthComplete(isSuccess:false);
                return;
            }
           
            OnAuthComplete(result.IsSuccess);
        }

        public async void OnCreateLobbyBtnClick()
        {
            netPlayerUI.SetShowLobbyTxt("<color=yellow>Creating Lobby...</color>");
            
            // TODO: Consider `Local` visibility.
            const CreateLobbyRequest.VisibilityEnum visibility = CreateLobbyRequest.VisibilityEnum.Public;

            Lobby lobby = null;
            try
            {
                lobby = await hathoraClient.ClientApis.LobbyApi.ClientCreateLobbyAsync(visibility);
            }
            catch (Exception e)
            {
                OnCreateOrJoinLobbyComplete(null);
                return;
            }
            
            OnCreateOrJoinLobbyComplete(lobby);
        }
        
        /// <summary>
        /// The player pressed ENTER || unfocused the roomId input.
        /// </summary>
        public async void OnJoinLobbyInputEnd()
        {
            netPlayerUI.SetShowLobbyTxt("<color=yellow>Joining Lobby...</color>");
            string roomIdInputStr = netPlayerUI.OnJoinLobbyBtnClickGetRoomId();

            Lobby lobby = null;
            try
            {
                lobby = await hathoraClient.ClientApis.LobbyApi.ClientGetJoinInfoAsync(roomIdInputStr);
            }
            catch (Exception e)
            {
                OnCreateOrJoinLobbyComplete(null);
                return;
            }

            OnCreateOrJoinLobbyComplete(lobby);
        }
        
        /// <summary>Public lobbies only.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public async void OnViewLobbiesBtnClick()
        {
            List<Lobby> lobbies = null;
            try
            {
                lobbies = await hathoraClient.ClientApis.LobbyApi.ClientListPublicLobbiesAsync();
            }
            catch (Exception e)
            {
                throw new NotImplementedException("TODO: Get lobbies err handling UI");
            }
            
            OnViewPublicLobbiesComplete(lobbies);
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

        private void OnViewPublicLobbiesComplete(List<Lobby> lobbies)
        {
            int numLobbiesFound = lobbies?.Count ?? 0;
            Debug.Log($"[NetHathoraPlayer] OnViewPublicLobbiesComplete: # Lobbies found: {numLobbiesFound}");

            if (lobbies == null || numLobbiesFound == 0)
            {
                throw new NotImplementedException("TODO: !Lobbies handling");
            }
            
            List<Lobby> sortedLobbies = lobbies.OrderBy(lobby => lobby.CreatedAt).ToList();
            netPlayerUI.OnViewLobbies(sortedLobbies);
        }
        
        private void OnCreateOrJoinLobbyComplete(Lobby lobby)
        {
            if (string.IsNullOrEmpty(lobby?.RoomId))
            {
                netPlayerUI.OnJoinedOrCreatedLobbyFail();
                return;
            }

            netPlayerUI.OnJoinedOrCreatedLobby(lobby.RoomId);
            
            // Once we get the lobby, we want to join it.
            // TODO
        }
        #endregion
    }
}

#if UNITY_SERVER || DEBUG
// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using FishNet.Object;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Server.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hathora.Net.Server
{
    public class NetHathoraServer : NetworkBehaviour
    {
        [Header("Config")]
        [SerializeField] 
        private HathoraServerConfig hathoraServerConfig;

        [FormerlySerializedAs("ServerApiCs")]
        [FormerlySerializedAs("ServerApiContainer")]
        [Header("Hathora SDK API Wrappers")]
        [SerializeField]
        public ServerApiContainer ServerApis;

        private Configuration hathoraSdkConfig { get; set; }
        private NetSession playerSession { get; set; }

        private LobbyV2Api lobbyApi { get; set; }
        private AuthV1Api authApi { get; set; }
        
        
        #region Event Delegates
        /// <summary>
        /// isSuccess
        /// </summary>
        public event EventHandler<bool> AuthComplete;
        
        /// <summary>
        /// TODO: Lobby
        /// </summary>
        public event EventHandler<Lobby> CreateLobbyComplete;
        #endregion // Event Delegates
        
        
        /// <summary>
        /// If a Player inits as a Server from NetHathoraPlayer, we'll get the session instance.
        /// </summary>
        /// <param name="_playerSession"></param>
        public void InitFromPlayer(NetSession _playerSession) =>
            this.playerSession = _playerSession;


        /// <summary>
        /// Init Hathora Config + server APIs.
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();
            
#if UNITY_EDITOR
            // Ensure a Config file is serialized; load default if not.
            // Since it's common to copy the config containing secret keys,
            // collaborating may deserialize the ignored file.
            if (hathoraServerConfig == null)
            {
                Debug.LogError("[NetHathoraServer]**ERR @ OnStartServer: " +
                    "MISSING `hathoraServerConfig` - Open `Player` and serialize a `HathoraServerConfig` " +
                    "to the Player's `NetHathoraServer` component. (!) This file contains secrets you " +
                    "may not want to push to version control: We recommend copying it per dev/env. " +
                    "By default, each custom copy with a similar naming schema is .gitignored, " +
                    "by default (eg: `HathoraServerConfig-Dev`).");
                return;
            }
#endif

            hathoraSdkConfig = new Configuration
            {
                // NOT to be confused with a PLAYER token from AuthV1
                AccessToken = hathoraServerConfig.DevAuthToken,
                // AppId // TODO?
            };
      
            // Init server APIs
            authApi = new AuthV1Api(hathoraSdkConfig);

            ServerApis.RoomApi.Init(hathoraSdkConfig, hathoraServerConfig, playerSession);
            
            lobbyApi = new LobbyV2Api(hathoraSdkConfig);
        }


        #region Server Async Hathora SDK Calls
        public async Task ServerAuthAsync()
        {
            if (!base.IsServer)
                return;

            LoginAnonymous200Response anonLoginResult;
            try
            {
                anonLoginResult = await authApi.LoginAnonymousAsync(hathoraServerConfig.AppId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraPlayer]**ERR @ ServerAuthAsync (LoginAnonymousAsync): {e.Message}");
                onServerAuthFail();
                await Task.FromException<Exception>(e);
                return;
            }

            bool isAuthed = !string.IsNullOrEmpty(anonLoginResult?.Token); 
            Debug.Log($"[NetHathoraPlayer] isAuthed: {isAuthed}");
            
            if (isAuthed)
                onServerAuthSuccess(anonLoginResult.Token);
            else
                onServerAuthFail();
        }

        public async Task ServerCreateLobbyAsync(CreateLobbyRequest.VisibilityEnum lobbyVisibility)
        {
            if (!base.IsServer)
                return;

            LobbyInitConfig lobbyInitConfig = new();
            CreateLobbyRequest request = new CreateLobbyRequest(
                lobbyVisibility, 
                lobbyInitConfig, 
                hathoraServerConfig.Region);

            Lobby lobby;
            try
            {
                lobby = await lobbyApi.CreateLobbyAsync(
                    hathoraServerConfig.AppId,
                    hathoraServerConfig.DevAuthToken,
                    request);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraPlayer]**ERR @ ServerCreateLobbyAsync (CreateLobbyAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                return;
            }

            if (lobby != null)
                onServerCreateLobbySuccess(lobby);
        }
        #endregion // Server Async Hathora SDK Calls
        
        
        #region Success Callbacks
        void onServerAuthSuccess(string playerAuthToken)
        {
            playerSession.InitNetSession(playerAuthToken, hathoraServerConfig.DevAuthToken);
            
            const bool isSucccess = true;
            AuthComplete?.Invoke(this, isSucccess);
        }

        private void onServerCreateLobbySuccess(Lobby lobby)
        {
            throw new NotImplementedException();
            // playerSession.Lobby = lobby;
            // CreateLobbyComplete?.Invoke(this, lobby);
        }
        #endregion // Success Callbacks
        
        
        #region Server Fail Callbacks
        private void onServerAuthFail()
        {
            const bool isSucccess = false;
            AuthComplete?.Invoke(this,isSucccess);
        }
        #endregion // Success Callbacks
    }
}
#endif // UNITY_SERVER

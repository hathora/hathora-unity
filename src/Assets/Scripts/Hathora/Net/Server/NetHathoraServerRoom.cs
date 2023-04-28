// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Net.Server.Models;
using UnityEngine;

namespace Hathora.Net.Server
{
    /// <summary>
    /// Call base.Init() to pass dev tokens, etc.
    /// </summary>
    public class NetHathoraServerRoom : HathoraNetServerApiBase
    {
        private RoomV1Api roomApi;

        public override void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig, 
            NetSession _playerSession)
        {
            base.Init(_hathoraSdkConfig, _hathoraServerConfig, _playerSession);
            this.roomApi = new RoomV1Api(_hathoraSdkConfig);
        }


        #region Event Delegates
        /// <summary>roomName</summary>
        public event EventHandler<string> CreateRoomComplete;
        #endregion // Event Delegates

        
        #region Server Room Async Hathora SDK Calls
        /// <summary>
        /// SERVER ONLY.
        /// When done, calls onCreateRoomObserverRpc.
        /// </summary>
        public async Task ServerCreateRoomAsync()
        {
            if (!base.IsServer)
                return;

            CreateRoomRequest request = new CreateRoomRequest(hathoraServerConfig.Region);

            string roomName;
            try
            {
                roomName = await roomApi.CreateRoomAsync(
                    hathoraServerConfig.AppId, 
                    request, 
                    CancellationToken.None);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetHathoraPlayer]**ERR @ ServerCreateRoomAsync (CreateRoomAsync): {e.Message}");
                await Task.FromException<Exception>(e);
                onCreateRoomFail();
                return;
            }

            Debug.Log($"[NetHathoraPlayer] SERVER ServerCreateRoomAsync => returned: {roomName}");
            
            bool createdRoom = !string.IsNullOrEmpty(roomName);
            if (createdRoom)
                onServerCreateRoomSuccess(roomName);
            else
                onCreateRoomFail();
        }
        #endregion // Server Room Async Hathora SDK Calls
        
        
        #region Success Callbacks
        void onServerCreateRoomSuccess(string roomName)
        {
            PlayerSession.RoomName = roomName;
            CreateRoomComplete?.Invoke(this, roomName);
        }
        #endregion // Success Callbacks
        
        
        #region Fail Callbacks
        private void onCreateRoomFail()
        {
            const string roomName = null;
            CreateRoomComplete?.Invoke(this, roomName);
        }
        #endregion // Fail Callbacks
    }
}

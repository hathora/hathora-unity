// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using HathoraSdk;
using HathoraSdk.Models.Operations;
using HathoraSdk.Models.Shared;
using HathoraSdk.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
    /// <summary>
    /// Handles Application API calls to Hathora Server.
    /// - Passes API key from HathoraServerConfig to SDK
    /// - Passes Auth0 (Dev Token) from hathoraServerConfig to SDK
    /// - API Docs | https://hathora.dev/api#tag/LobbyV2
    /// </summary>
    public class HathoraServerLobbyApi : HathoraServerApiWrapperBase
    {
        private readonly LobbyV2SDK lobbyApi;
        
        /// <summary>
        /// </summary>
        /// <param name="_hathoraServerConfig"></param>
        public HathoraServerLobbyApi( 
            HathoraServerConfig _hathoraServerConfig,
            SDKConfig _hathoraSdkConfig = null)
            : base(_hathoraServerConfig, _hathoraSdkConfig)
        {
            Debug.Log("[HathoraServerLobbyApi] Initializing API...");
            
            // TODO: Overloading VxSDK constructor with nulls, for now, until we know how to properly construct
            SpeakeasyHttpClient httpClient = null;
            string serverUrl = null;
            this.lobbyApi = new LobbyV2SDK(
                httpClient,
                httpClient, 
                serverUrl,
                HathoraSdkConfig);
        }
        
        
        #region Server Lobby Async Hathora SDK Calls
        /// <summary>
        /// When you get the result, check Status for Active.
        /// (!) If !Active, getting the ConnectionInfoV2 will result in !ExposedPort.
        /// </summary>
        /// <param name="_roomId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<Lobby> GetLobbyInfoAsync(
            string _roomId, 
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerLobbyApi)}.{nameof(GetLobbyInfoAsync)}]";

            // Prepare request
            GetLobbyInfoRequest getLobbyInfoRequest = new()
            {
                //AppId = base.AppId, // TODO: SDK already has Config via constructor - redundant
                RoomId = _roomId,
            };
            
            // Get response async =>
            GetLobbyInfoResponse getLobbyInfoResult = null;

            try
            {
                getLobbyInfoResult = await lobbyApi.GetLobbyInfoAsync(getLobbyInfoRequest);
                Assert.IsNotNull(getLobbyInfoResult, $"{logPrefix} !getLobbyInfoResult");
            }
            catch (TaskCanceledException)
            {
                // The user explicitly cancelled, or the Task timed out
                Debug.Log($"{logPrefix} Task cancelled");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(lobbyApi.GetLobbyInfoAsync)} => Error: {e.Message}");
                return null; // fail
            }

            // Process result
            Debug.Log($"{logPrefix} Success: <color=yellow>" +
                $"{nameof(getLobbyInfoResult)}: {ToJson(getLobbyInfoResult)}</color>");

            return getLobbyInfoResult.Lobby;
        }
        #endregion // Server Lobby Async Hathora SDK Calls
    }
}

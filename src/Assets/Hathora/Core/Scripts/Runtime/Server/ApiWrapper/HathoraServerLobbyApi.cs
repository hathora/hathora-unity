// Created by dylan@hathora.dev

using System;
using System.Threading;
using System.Threading.Tasks;
using HathoraSdk;
using HathoraSdk.Models.Shared;
using HathoraSdk.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
    /// <summary>
    /// V2 API Doc | https://hathora.dev/api#tag/LobbyV2
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
        /// <param name="_lobbyId"></param>
        /// <param name="_cancelToken"></param>
        /// <returns></returns>
        public async Task<Lobby> GetLobbyInfoAsync(
            string _lobbyId, 
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerLobbyApi)}.{nameof(GetLobbyInfoAsync)}]";
            Lobby getLobbyInfoResult;

            try
            {
                getLobbyInfoResult = await lobbyApi.GetLobbyInfoAsync(
                    AppId,
                    _lobbyId,
                    _cancelToken);
                
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

            Debug.Log($"{logPrefix} Success: <color=yellow>" +
                $"{nameof(getLobbyInfoResult)}: {ToJson(getLobbyInfoResult)}</color>");

            return getLobbyInfoResult;
        }
        #endregion // Server Lobby Async Hathora SDK Calls
    }
}

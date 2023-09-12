// Created by dylan@hathora.dev

using System.Threading;
using System.Threading.Tasks;
using HathoraSdk;
using HathoraSdk.Models.Shared;
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
        /// TODO: `Configuration` is missing in the new SDK - cleanup, if permanently gone.
        /// </summary>
        /// <param name="_hathoraServerConfig"></param>
        public HathoraServerLobbyApi( 
            HathoraServerConfig _hathoraServerConfig,
            SDKConfig _hathoraSdkConfig = null)
            : base(_hathoraServerConfig, _hathoraSdkConfig)
        {
            Debug.Log("[HathoraServerLobbyApi] Initializing API...");
            
            // TODO: Manually init w/out constructor, or add constructor support to model
            // TODO: `Configuration` is missing in the new SDK - cleanup, if permanently gone.
            // this.lobbyApi = new LobbyV2SDK(base.HathoraSdkConfig);
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
                // TODO: The old SDK passed `AppId` -- how does the new SDK handle this if we don't pass AppId and don't init with a Sdk Configuration?
                // TODO: Manually init w/out constructor, or add constructor support to model
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
            catch (ApiException apiErr)
            {
                // HTTP err from Hathora Cloud
                HandleApiException(
                    nameof(HathoraServerLobbyApi),
                    nameof(GetLobbyInfoAsync), 
                    apiErr);
                return null;
            }

            // TODO: `ToJson()` no longer exists in request/response models, but should soon make a return?
            // Debug.Log($"{logPrefix} Success: <color=yellow>getLobbyInfoResult: " +
            //     $"{getLobbyInfoResult.ToJson()}</color>");
            Debug.Log($"{logPrefix} Success");

            return getLobbyInfoResult;
        }
        #endregion // Server Lobby Async Hathora SDK Calls
    }
}

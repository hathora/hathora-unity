// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Common.Models;
using Hathora.Core.Scripts.Runtime.Server.Models;
using HathoraSdk;
using HathoraSdk.Models.Operations;
using HathoraSdk.Models.Shared;
using HathoraSdk.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
    /// <summary>
    /// Handles Deployment API calls to Hathora Server.
    /// - Passes API key from HathoraServerConfig to SDK
    /// - Passes Auth0 (Dev Token) from hathoraServerConfig to SDK
    /// - API Docs | https://hathora.dev/api#tag/BuildV1
    /// </summary>
    public class HathoraServerDeployApi : HathoraServerApiWrapperBase
    {
        private readonly DeploymentV1SDK deployApi;
        private HathoraDeployOpts deployOpts => HathoraServerConfig.HathoraDeployOpts;

        
        /// <summary>
        /// </summary>
        /// <param name="_hathoraServerConfig"></param>
        /// <param name="_hathoraSdkConfig">
        /// Passed along to base for API calls as `HathoraSdkConfig`; potentially null in child.
        /// </param>
        public HathoraServerDeployApi( 
            HathoraServerConfig _hathoraServerConfig,
            SDKConfig _hathoraSdkConfig = null)
            : base(_hathoraServerConfig, _hathoraSdkConfig)
        {
            Debug.Log("[HathoraServerDeployApi] Initializing API...");

            // TODO: Overloading VxSDK constructor with nulls, for now, until we know how to properly construct
            SpeakeasyHttpClient httpClient = null;
            string serverUrl = null;
            this.deployApi = new DeploymentV1SDK(
                httpClient,
                httpClient, 
                serverUrl,
                HathoraSdkConfig);
        }
        
        
        #region Server Deploy Async Hathora SDK Calls
        /// <summary>
        /// Wrapper for `CreateDeploymentAsync` to upload and deploy a cloud deploy to Hathora.
        /// </summary>
        /// <param name="_buildId"></param>
        /// <param name="_env">
        /// Optional - env vars. We recommend you pass the ones from your previous
        /// build, via `deployApi.GetDeploymentsAsync()`.
        /// </param>
        /// <param name="_additionalContainerPorts">
        /// Optional - For example, you may want to expose 2 ports to support both UDP and
        /// TLS transports simultaneously (eg: FishNet's `Multipass` transport)
        /// </param>
        /// <param name="_cancelToken">TODO</param>
        /// <returns>Returns Deployment on success</returns>
        public async Task<Deployment> CreateDeploymentAsync(
            int _buildId,
            List<DeploymentEnv> _env = null,
            List<ContainerPort> _additionalContainerPorts = null,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerDeployApi)}.{nameof(CreateDeploymentAsync)}]";
            
            // Prepare request
            TransportType selectedTransportType = deployOpts.SelectedTransportType;
            
            #region DeploymentEnvConfigInner Workaround
            // #######################################################################################
            // (!) Hathora SDK's DeploymentEnv is identical to DeploymentConfigEnv >> Port it over
            List<DeploymentConfigEnv> envWorkaround = _env?.Select(envVar => 
                new DeploymentConfigEnv
                {
                    Name = envVar.Name, 
                    Value = envVar.Value,
                }).ToList();
            // #######################################################################################
            #endregion DeploymentEnvConfigInner Workaround
            
            DeploymentConfig deployConfig = new()
            {
                Env = envWorkaround ?? new List<DeploymentConfigEnv>(),
                RoomsPerProcess = deployOpts.RoomsPerProcess, 
                PlanName = deployOpts.SelectedPlanName, 
                AdditionalContainerPorts = _additionalContainerPorts ?? new List<ContainerPort>(),
                TransportType = selectedTransportType,
                ContainerPort = deployOpts.ContainerPort.Port,
            };

            CreateDeploymentRequest createDeploymentRequest = new()
            {
                //AppId = base.AppId, // TODO: SDK already has Config via constructor - redundant
                DeploymentConfig = deployConfig,
                BuildId = _buildId,
            };
            
            Debug.Log($"{logPrefix} <color=yellow>{nameof(deployConfig)}: {ToJson(deployConfig)}</color>");

            // Get response async =>
            CreateDeploymentResponse createDeploymentResponse = null;
            
            try
            {
                createDeploymentResponse = await deployApi.CreateDeploymentAsync(
                    new CreateDeploymentSecurity { Auth0 = base.Auth0DevToken }, // TODO: Redundant - already has Auth0 from constructor via SDKConfig.DeveloperToken
                    createDeploymentRequest);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(deployApi.CreateDeploymentAsync)} => Error: {e.Message}");
                return null; // fail
            }

            Debug.Log($"{logPrefix} <color=yellow>" +
                $"{nameof(createDeploymentResponse)}: {ToJson(createDeploymentResponse)}</color>");

            return createDeploymentResponse.Deployment;
        }

        /// <summary>
        /// Wrapper for `CreateDeploymentAsync` to upload and deploy a cloud deploy to Hathora.
        /// </summary>
        /// <param name="_cancelToken">TODO</param>
        /// <returns>Returns Deployment on success</returns>
        public async Task<List<Deployment>> GetDeploymentsAsync(
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerDeployApi)}.{nameof(CreateDeploymentAsync)}]";

            // Prepare request
            GetDeploymentsRequest getDeploymentsRequest = new()
            {
                //AppId = base.AppId, // TODO: SDK already has Config via constructor - redundant
            };

            // Get response async =>
            GetDeploymentsResponse getDeploymentsResponse = null;
            
            try
            {
                getDeploymentsResponse = await deployApi.GetDeploymentsAsync(
                    new GetDeploymentsSecurity { Auth0 = base.Auth0DevToken }, // TODO: Redundant - already has Auth0 from constructor via SDKConfig.DeveloperToken
                    getDeploymentsRequest);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(deployApi.GetDeploymentsAsync)} => Error: {e.Message}");
                return null; // fail
            }

            // Process response
            List<Deployment> deployments = getDeploymentsResponse.Deployments;
            Debug.Log($"{logPrefix} <color=yellow>num: '{deployments?.Count}'</color>");
            
            return deployments;
        }
        #endregion // Server Deploy Async Hathora SDK Calls
    }
}

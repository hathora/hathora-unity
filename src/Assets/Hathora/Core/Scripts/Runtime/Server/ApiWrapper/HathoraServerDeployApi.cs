// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Core.Scripts.Runtime.Common.Models;
using Hathora.Core.Scripts.Runtime.Server.Models;
using HathoraSdk;
using HathoraSdk.Models.Shared;
using HathoraSdk.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Hathora.Core.Scripts.Runtime.Server.ApiWrapper
{
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
        /// <param name="_cancelToken"></param>
        /// <returns>Returns Deployment on success</returns>
        public async Task<Deployment> CreateDeploymentAsync(
            double _buildId,
            List<DeploymentEnv> _env = null,
            List<ContainerPort> _additionalContainerPorts = null,
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerDeployApi)}.{nameof(CreateDeploymentAsync)}]";
            
            // (!) Throws on constructor Exception - fallback to new objects on null
            DeploymentConfig deployConfig = null;
            try
            {
                // Hathora SDK's TransportType enum's index starts at 1: But so does deployOpts to match
                TransportType selectedTransportType = deployOpts.SelectedTransportType;
                
                #region DeploymentEnvConfigInner Workaround
                // #######################################################################################
                // (!) Hathora SDK's DeploymentConfigEnvInner is Obsolete for DeploymentEnvInner
                // (!) These two are identical in properties: For now, we'll re-serialize
                List<DeploymentConfigEnv> envWorkaround = null;
                if (_env?.Count > 0)
                {
                    string envWorkaroundJson = JsonConvert.SerializeObject(_env);
                    envWorkaround = JsonConvert.DeserializeObject<List<DeploymentConfigEnv>>(envWorkaroundJson);    
                }
                // #######################################################################################
                #endregion DeploymentEnvConfigInner Workaround
                
                deployConfig = new DeploymentConfig(
                    envWorkaround ?? new List<DeploymentConfigEnv>(),  // DEPRECATED: To be replaced by below line
                    // _env ?? new List<DeploymentEnvInner>(),              // TODO: To replace the above line
                    deployOpts.RoomsPerProcess, 
                    deployOpts.SelectedPlanName, 
                    _additionalContainerPorts ?? new List<ContainerPort>(),
                    selectedTransportType,
                    deployOpts.ContainerPort.Port
                );
                
                Debug.Log($"{logPrefix} <color=yellow>{nameof(deployConfig)}: {ToJson(deployConfig)}</color>");
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} Error: {e}");
                throw;
            }

            Deployment createDeploymentResult = null;
            try
            {
                createDeploymentResult = await deployApi.CreateDeploymentAsync(
                    AppId,
                    _buildId,
                    deployConfig,
                    _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(deployApi.CreateDeploymentAsync)} => Error: {e.Message}");
                return null; // fail
            }

            Debug.Log($"{logPrefix} <color=yellow>" +
                $"{nameof(createDeploymentResult)}: {ToJson(createDeploymentResult)}</color>");

            return createDeploymentResult;
        }

        /// <summary>
        /// Wrapper for `CreateDeploymentAsync` to upload and deploy a cloud deploy to Hathora.
        /// </summary>
        /// <param name="_cancelToken"></param>
        /// <returns>Returns Deployment on success</returns>
        public async Task<List<Deployment>> GetDeploymentsAsync(
            CancellationToken _cancelToken = default)
        {
            string logPrefix = $"[{nameof(HathoraServerDeployApi)}.{nameof(CreateDeploymentAsync)}]";

            List<Deployment> getDeploymentsResult = null;
            
            try
            {
                getDeploymentsResult = await deployApi.GetDeploymentsAsync(AppId, _cancelToken);
            }
            catch (Exception e)
            {
                Debug.LogError($"{logPrefix} {nameof(deployApi.GetDeploymentsAsync)} => Error: {e.Message}");
                return null; // fail
            }

            Debug.Log($"{logPrefix} <color=yellow>num: '{getDeploymentsResult?.Count}'</color>");
            return getDeploymentsResult;
        }
        #endregion // Server Deploy Async Hathora SDK Calls


        // /// TODO
        // /// <summary>
        // /// Convert List of HathoraEnvVars to List of DeploymentConfigEnvInner.
        // /// </summary>
        // /// <returns>
        // /// Returns empty list (NOT null) on empty, since Env is required for DeploymentConfig.
        // /// </returns>
        // private List<DeploymentConfigEnvInner> parseEnvFromConfig()
        // {
        //     // Validate
        //     List<HathoraEnvVars> envVars = HathoraServerConfig.HathoraDeployOpts.EnvVars;
        //     if (envVars == null || envVars.Count == 0) 
        //         return new List<DeploymentConfigEnvInner>();
        //     
        //     // Parse
        //     return HathoraServerConfig.HathoraDeployOpts.EnvVars.Select(_env => 
        //         new DeploymentConfigEnvInner(_env.Key, _env.StrVal)).ToList();
        // }
    }
}

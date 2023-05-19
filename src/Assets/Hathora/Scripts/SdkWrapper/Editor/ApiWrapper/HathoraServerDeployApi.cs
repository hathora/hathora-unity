// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Common;
using Hathora.Scripts.SdkWrapper.Models;
using Hathora.Scripts.Utils;
using UnityEngine;

namespace Hathora.Scripts.SdkWrapper.Editor.ApiWrapper
{
    public class HathoraServerDeployApi : HathoraServerApiBase
    {
        private readonly DeploymentV1Api deployApi;
        
        public HathoraServerDeployApi(
            Configuration _hathoraSdkConfig, 
            NetHathoraConfig _netHathoraConfig)
            : base(_hathoraSdkConfig, _netHathoraConfig)
        {
            Debug.Log("[HathoraServerDeployApi] Initializing API...");
            this.deployApi = new DeploymentV1Api(_hathoraSdkConfig);
        }
        
        
        #region Server Deploy Async Hathora SDK Calls
        /// <summary>
        /// Wrapper for `CreateDeploymentAsync` to upload and deploy a cloud deploy to Hathora.
        /// </summary>
        /// <returns>Returns Deployment on success</returns>
        public async Task<Deployment> CreateDeploymentAsync(double buildId)
        {
            HathoraDeployOpts deployOpts = NetHathoraConfig.HathoraDeployOpts;
            List<ContainerPort> extraContainerPorts = deployOpts.AdvancedDeployOpts.GetExtraContainerPorts();
            
            // (!) Throws on constructor Exception
            DeploymentConfig deployConfig = null;
            try
            {
                deployConfig = new DeploymentConfig(
                    parseEnvFromConfig() ?? new List<DeploymentConfigEnvInner>(),
                    deployOpts.RoomsPerProcess, 
                    deployOpts.PlanName, 
                    extraContainerPorts,
                    deployOpts.ContainerPortWrapper.TransportType,
                    deployOpts.ContainerPortWrapper.PortNumber
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                throw;
            }

            Deployment cloudDeployResult;
            try
            {
                cloudDeployResult = await deployApi.CreateDeploymentAsync(
                    NetHathoraConfig.HathoraCoreOpts.AppId,
                    buildId,
                    deployConfig);
            }
            catch (ApiException apiErr)
            {
                HandleServerApiException(
                    nameof(HathoraServerBuildApi),
                    nameof(CreateDeploymentAsync), 
                    apiErr);
                return null;
            }

            Debug.Log($"[HathoraServerDeployApi] " +
                $"BuildId: '{cloudDeployResult?.BuildId}', " +
                $"DeployId: '{cloudDeployResult?.DeploymentId}");

            return cloudDeployResult;
        }
        #endregion // Server Deploy Async Hathora SDK Calls


        /// <summary>
        /// Convert List of HathoraEnvVars to List of DeploymentConfigEnvInner.
        /// </summary>
        /// <returns>
        /// Returns empty list (NOT null) on empty, since Env is required for DeploymentConfig.
        /// </returns>
        private List<DeploymentConfigEnvInner> parseEnvFromConfig()
        {
            // Validate
            List<HathoraEnvVars> envVars = NetHathoraConfig.HathoraDeployOpts.EnvVars;
            if (envVars == null || envVars.Count == 0) 
                return new List<DeploymentConfigEnvInner>();
            
            // Parse
            return NetHathoraConfig.HathoraDeployOpts.EnvVars.Select(env => 
                new DeploymentConfigEnvInner(env.Key, env.StrVal)).ToList();
        }
    }
}

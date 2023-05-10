// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Server;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ApiWrapper
{
    public class HathoraServerDeployApi : HathoraServerApiBase
    {
        private DeploymentV1Api deployApi;
        
        public override void Init(
            Configuration _hathoraSdkConfig, 
            HathoraServerConfig _hathoraServerConfig)
        {
            Debug.Log("[HathoraServerDeployApi] Initializing API...");
            base.Init(_hathoraSdkConfig, _hathoraServerConfig);
            this.deployApi = new DeploymentV1Api(_hathoraSdkConfig);
        }
        
        
        
        #region Server Deploy Async Hathora SDK Calls
        /// <summary>
        /// Wrapper for `CreateDeployAsync` to upload and deploy a cloud deploy to Hathora.
        /// </summary>
        /// <returns>Returns Deployment on success</returns>
        public async Task<Deployment> DeployDeployToHathora(double buildId)
        {
            HathoraUtils.HathoraDeployOpts deployOpts = hathoraServerConfig.HathoraDeployOpts; 
            DeploymentConfig deployConfig = new()
            {
                PlanName = deployOpts.PlanName,
                RoomsPerProcess = deployOpts.RoomsPerProcess,
                ContainerPort = deployOpts.TransportInfo.PortNumber,
                TransportType = deployOpts.TransportInfo.TransportType,
                Env = parseEnvFromConfig(),
                // AdditionalProperties = // TODO
            };

            Deployment cloudDeployResult;
            try
            {
                cloudDeployResult = await deployApi.CreateDeploymentAsync(
                    hathoraServerConfig.AppId,
                    buildId,
                    deployConfig);
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraServerDeployApi]**ERR @ DeployDeployToHathora " +
                    $"(CreateDeployAsync): {e.Message}");
                await Task.FromException<Exception>(e);
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
        /// <returns></returns>
        private List<DeploymentConfigEnvInner> parseEnvFromConfig()
        {
            return hathoraServerConfig.HathoraDeployOpts.EnvVars.Select(env => 
                new DeploymentConfigEnvInner(env.Key, env.StrVal)).ToList();
        }
    }
}

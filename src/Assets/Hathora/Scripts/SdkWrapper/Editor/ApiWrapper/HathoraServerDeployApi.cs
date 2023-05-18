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
            DeploymentConfig deployConfig = new()
            {
                PlanName = deployOpts.PlanName,
                RoomsPerProcess = deployOpts.RoomsPerProcess,
                ContainerPort = deployOpts.TransportInfo.PortNumber,
                TransportType = deployOpts.TransportInfo.TransportType,
                Env = parseEnvFromConfig() ?? new List<DeploymentConfigEnvInner>(),
                // AdditionalProperties = // TODO
            };

            Deployment cloudDeployResult;
            try
            {
                cloudDeployResult = await deployApi.CreateDeploymentAsync(
                    NetHathoraConfig.HathoraCoreOpts.AppId,
                    buildId,
                    deployConfig);
            }
            catch (Exception e)
            {
                Debug.LogError($"[HathoraServerDeployApi.CreateDeploymentAsync]" +
                    $"**ERR @ DeployDeployToHathora (CreateDeployAsync): {e.Message}");
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
            return NetHathoraConfig.HathoraDeployOpts.EnvVars.Select(env => 
                new DeploymentConfigEnvInner(env.Key, env.StrVal)).ToList();
        }
    }
}

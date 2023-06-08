// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Core.Scripts.Editor.Common;
using Hathora.Core.Scripts.Runtime.Server;
using Hathora.Core.Scripts.Runtime.Server.ApiWrapper;
using Hathora.Core.Scripts.Runtime.Server.Models;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

namespace Hathora.Core.Scripts.Editor.Server
{
    public delegate void ZipCompleteHandler();
    public delegate void OnBuildReqComplete(Build _buildInfo);
    public delegate void OnUploadComplete();
    
    public static class HathoraServerDeploy
    {
        public static bool IsDeploying => 
            DeploymentStep != DeploymentSteps.Done;
        public enum DeploymentSteps
        {
            Done, // Same as not deployment
            Init,
            Zipping,
            RequestingUploadPerm,
            Uploading,
            Deploying,
        }
        
        private static int maxDeploySteps => 
            Enum.GetValues(typeof(DeploymentSteps)).Length;
        
        public static DeploymentSteps DeploymentStep { get; private set; }
        
        public static event ZipCompleteHandler OnZipComplete;
        public static event OnBuildReqComplete OnBuildReqComplete;
        public static event OnUploadComplete OnUploadComplete;

        
        public static string GetDeployFriendlyStatus() => DeploymentStep switch
        {
            DeploymentSteps.Done => "Done",
            DeploymentSteps.Init => $"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>(1/{maxDeploySteps})</color> Initializing...",
            DeploymentSteps.Zipping => $"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>(2/{maxDeploySteps})</color> Zipping...",
            DeploymentSteps.RequestingUploadPerm => $"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>(3/{maxDeploySteps})</color> Requesting Upload Permission...",
            DeploymentSteps.Uploading => $"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>(4/{maxDeploySteps})</color> Uploading Build...",
            DeploymentSteps.Deploying => $"<color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>(5/{maxDeploySteps})</color> Deploying Build...",
            _ => throw new ArgumentOutOfRangeException(),
        };

        /// <summary>
        /// Deploys with HathoraServerConfig opts. Optionally sub to events:
        /// - OnZipComplete
        /// - OnBuildReqComplete
        /// - OnUploadComplete
        /// </summary>
        /// <param name="_serverConfig">Find via menu `Hathora/Find UserConfig(s)`</param>
        /// <param name="_cancelToken"></param>
        public static async Task<Deployment> DeployToHathoraAsync(
            HathoraServerConfig _serverConfig,
            CancellationToken _cancelToken = default)
        {
            DeploymentStep = DeploymentSteps.Init;
            
            Debug.Log("[HathoraServerBuild.DeployToHathoraAsync] " +
                "<color=yellow>Starting...</color>");
            
            Assert.IsNotNull(_serverConfig, "[HathoraServerBuild.DeployToHathoraAsync] " +
                "Cannot find HathoraServerConfig ScriptableObject");
            
            // Prepare paths and file names that we didn't get from UserConfig  
            HathoraServerPaths serverPaths = new(_serverConfig);
            
                        
            #region Dockerfile >> Compress to .tar.gz
            // ----------------------------------------------
            DeploymentStep = DeploymentSteps.Zipping;

            // Compress build into .tar.gz (gzipped tarball)
            await HathoraTar.ArchiveFilesAsTarGzToDotHathoraDir(
                serverPaths, 
                _cancelToken);
            
            OnZipComplete?.Invoke();
            #endregion // Dockerfile >> Compress to .tar.gz
            

            #region Request to build
            // ----------------------------------------------
            DeploymentStep = DeploymentSteps.RequestingUploadPerm;

            // Get a buildId from Hathora
            HathoraServerBuildApi buildApi = new(_serverConfig);

            Build buildInfo = null;
            try
            {
                buildInfo = await getBuildInfoAsync(buildApi, _cancelToken);
            }
            catch (Exception e)
            {
                return null;
            }
            Assert.IsNotNull(buildInfo, "[HathoraServerBuild.DeployToHathoraAsync] Expected buildInfo");
            
            // Building seems to unselect Hathora _serverConfig on success
            HathoraServerConfigFinder.ShowWindowOnly();
            
            OnBuildReqComplete?.Invoke(buildInfo);
            _cancelToken.ThrowIfCancellationRequested();
            #endregion // Request to build

            
            #region Upload Build
            // ----------------------------------------------
            DeploymentStep = DeploymentSteps.Uploading;

            // Upload the build to Hathora
            byte[] buildBytes = null;
            try
            {
                buildBytes = await uploadBuildAsync(
                    buildApi, 
                    buildInfo.BuildId, 
                    serverPaths);
            }
            catch (Exception e)
            {
                return null;
            }
            Assert.IsNotNull(buildBytes, "[HathoraServerBuild.DeployToHathoraAsync] Expected buildBytes");
            
            OnUploadComplete?.Invoke();
            _cancelToken.ThrowIfCancellationRequested();
            #endregion // Upload Build

            
            #region Deploy Build
            // ----------------------------------------------
            // Deploy the build
            DeploymentStep = DeploymentSteps.Deploying;
            HathoraServerDeployApi deployApi = new(_serverConfig);

            Deployment deployment = null;
            try
            {
                deployment = await deployBuildAsync(deployApi, buildInfo.BuildId);
            }
            catch (Exception e)
            {
                return null;
            }

            Assert.IsTrue(deployment?.BuildId > 0,  
                "[HathoraServerBuild.DeployToHathoraAsync] Expected deployment");
            #endregion // Deploy Build
            

            DeploymentStep = DeploymentSteps.Done;
            return deployment;
        }

        private static async Task<Deployment> deployBuildAsync(
            HathoraServerDeployApi _deployApi, 
            double _buildInfoBuildId)
        {
            Debug.Log("[HathoraServerDeploy.deployBuildAsync] " +
                "Deploying the uploaded build...");
            
            Deployment createDeploymentResult = null;
            try
            {
                createDeploymentResult = await _deployApi.CreateDeploymentAsync(_buildInfoBuildId);
            }
            catch (Exception e)
            {
                return null;
            }

            return createDeploymentResult;
        }

        private static async Task<byte[]> uploadBuildAsync(
            HathoraServerBuildApi _buildApi,
            double buildId,
            HathoraServerPaths _serverPaths)
        {
            Debug.Log("[HathoraServerDeploy.uploadBuildAsync] " +
                "Uploading the local build to Hathora...");
            
            // Pass BuildId and tarball (File stream) to Hathora
            string normalizedPathToTarball = Path.GetFullPath(
                $"{_serverPaths.PathToDotHathoraDir}/{_serverPaths.ExeBuildName}.tar.gz");
            
            byte[] runBuildResult;
            await using (FileStream fileStream = new(normalizedPathToTarball, FileMode.Open, FileAccess.Read))
            {
                runBuildResult = await _buildApi.RunCloudBuildAsync(
                    buildId, 
                    fileStream);
            }

            string successStr = runBuildResult.Length > 0 ? "Success" : "Failed";
            Debug.Log($"runBuildResult=={successStr}");
            
            return runBuildResult;
        }

        private static async Task<Build> getBuildInfoAsync(
            HathoraServerBuildApi _buildApi,
            CancellationToken _cancelToken = default)
        {
            Debug.Log("[HathoraServerDeploy.getBuildInfoAsync] " +
                "Getting build info (notably for buildId)...");
            
            Build createBuildResult = null;
            try
            {
                createBuildResult = await _buildApi.CreateBuildAsync(_cancelToken);
            }
            catch (Exception e)
            {
                return null;
            }

            return createBuildResult;
        }

    }
}

// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hathora.Cloud.Sdk.Model;
using Hathora.Scripts.Net.Server;
using Hathora.Scripts.SdkWrapper.Editor.ApiWrapper;
using Hathora.Scripts.Utils;
using Hathora.Scripts.Utils.Editor;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;
using Debug = UnityEngine.Debug;
using Configuration = Hathora.Cloud.Sdk.Client.Configuration;

namespace Hathora.Scripts.SdkWrapper.Editor
{
    public static class HathoraServerDeploy
    {
        /// <summary>
        /// Deploys with NetHathoraConfig opts.
        /// TODO: Support cancel token.
        /// </summary>
        /// <param name="_netConfig">Find via menu `Hathora/Find UserConfig(s)`</param>
        public static async Task DeployToHathoraAsync(NetHathoraConfig _netConfig)
        {
            Debug.Log("[HathoraServerBuild.DeployToHathoraAsync] " +
                "<color=yellow>Starting...</color>");
            
            Assert.IsNotNull(_netConfig, "[HathoraServerBuild.DeployToHathoraAsync] " +
                "Cannot find NetHathoraConfig ScriptableObject");
            
            // Prepare paths and file names that we didn't get from UserConfig
            HathoraUtils.HathoraDeployPaths deployPaths = new(_netConfig);
            
            // Generate the Dockerfile: Paths will be different for each collaborator\
            string dockerFileContent = generateDockerFileStr(deployPaths);
            await writeDockerFileAsync(
                deployPaths.PathToDockerfile,
                dockerFileContent);

            // Compress build into .tar.gz (gzipped tarball)
            List<string> filePathsToCompress = new()
            {
                deployPaths.PathToBuildExe, 
                deployPaths.PathToDockerfile,
            };

            await HathoraEditorUtils.TarballFilesVia7zAsync(
                deployPaths, 
                filePathsToCompress);

            // ----------------------------------------------
            // Get a buildId from Hathora
            Configuration sdkConfig = new()
            {
                AccessToken = _netConfig.HathoraCoreOpts.DevAuthOpts.DevAuthToken,
            };
            Assert.IsNotNull(sdkConfig.AccessToken);
            
            HathoraServerBuildApi buildApi = new(sdkConfig, _netConfig);

            Build buildInfo = null;
            try
            {
                buildInfo = await getBuildInfoAsync(buildApi);
            }
            catch (Exception e)
            {
                await Task.FromException<Exception>(e);
                return;
            }
            Assert.IsNotNull(buildInfo, "[HathoraServerBuild.DeployToHathoraAsync] Expected buildInfo");

            // ----------------------------------------------
            // Upload the build to Hathora
            byte[] buildBytes = null;
            try
            {
                buildBytes = await uploadBuildAsync(
                    buildApi, 
                    buildInfo.BuildId, 
                    deployPaths);
            }
            catch (Exception e)
            {
                await Task.FromException<Exception>(e);
                return;
            }
            Assert.IsNotNull(buildBytes, "[HathoraServerBuild.DeployToHathoraAsync] Expected buildBytes");
            
            // ----------------------------------------------
            // Deploy the build
            HathoraServerDeployApi deployApi = new(sdkConfig, _netConfig);

            Deployment deployment = null;
            try
            {
                deployment = await deployBuildAsync(deployApi, buildInfo.BuildId);
            }
            catch (Exception e)
            {
                await Task.FromException<Exception>(e);
                return;
            }
            Assert.IsNotNull(deployment, "[HathoraServerBuild.DeployToHathoraAsync] Expected deployment");
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
                await Task.FromException(e);
                return null;
            }

            return createDeploymentResult;
        }

        private static async Task<byte[]> uploadBuildAsync(
            HathoraServerBuildApi _buildApi,
            double buildId,
            HathoraUtils.HathoraDeployPaths _deployPaths)
        {
            // Pass BuildId and tarball (File stream) to Hathora
            string normalizedPathToTarball = Path.GetFullPath(
                $"{_deployPaths.TempDirPath}/{_deployPaths.ExeBuildName}.tar.gz");
            
            byte[] runBuildResult;
            await using (FileStream fileStream = new(normalizedPathToTarball, FileMode.Open, FileAccess.Read))
            {
                runBuildResult = await _buildApi.RunCloudBuildAsync(buildId, fileStream);
            }

            Debug.Log($"runBuildResult=={runBuildResult}");
            return runBuildResult;
        }

        private static async Task<Build> getBuildInfoAsync(HathoraServerBuildApi _buildApi)
        {
            Debug.Log("[HathoraServerDeploy.getBuildInfoAsync] " +
                "Getting build info (notably for buildId)...");
            
            Build createBuildResult = null;
            try
            {
                createBuildResult = await _buildApi.CreateBuildAsync();
            }
            catch (Exception e)
            {
                await Task.FromException(e);
                return null;
            }

            return createBuildResult;
        }


        #region Dockerfile
        /// <summary>
        /// Deletes an old one, if exists, to ensure updated paths.
        /// TODO: Use this to customize the Dockerfile without editing directly.
        /// </summary>
        /// <param name="pathToDockerfile"></param>
        /// <param name="dockerfileContent"></param>
        /// <returns>path/to/Dockerfile</returns>
        private static async Task writeDockerFileAsync(
            string pathToDockerfile, 
            string dockerfileContent)
        {
            // TODO: if (!overwriteDockerfile)
            if (File.Exists(pathToDockerfile))
            {
                Debug.LogWarning("[HathoraServerDeploy.writeDockerFileAsync] " +
                    "Deleting old Dockerfile...");
                File.Delete(pathToDockerfile);
            }

            try
            {
                await File.WriteAllTextAsync(pathToDockerfile, dockerfileContent);
            }
            catch (Exception e)
            {
                Debug.LogError("[HathoraServerDeploy.writeDockerFileAsync] " +
                    $"Failed to write Dockerfile to {pathToDockerfile}:\n{e}");
                
                await Task.FromException(e);
            }
        }

        /// <summary>
        /// Writes dynamic paths
        /// TODO: Use this to customize the Dockerfile without editing directly.
        /// </summary>
        /// <param name="deployPaths"></param>
        /// <returns>"path/to/DockerFile"</returns>
        private static string generateDockerFileStr(HathoraUtils.HathoraDeployPaths deployPaths)
        {
            return $@"# This file is auto-generated by HathoraServerDeploy.cs

FROM ubuntu

COPY ./Build-Server .

CMD ./{deployPaths.ExeBuildName}.tar.gz -mode server -batchmode -nographics
";
        }
        #endregion // Dockerfile
        
    }
}

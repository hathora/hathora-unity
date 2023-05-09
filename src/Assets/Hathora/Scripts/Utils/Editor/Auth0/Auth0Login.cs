// Created by dylan@hathora.dev

using System;
using System.IO;
using System.Threading.Tasks;
using Hathora.Scripts.Net.Server;
using Hathora.Scripts.Utils.Editor.Auth0.Models;
using Hathora.Scripts.Utils.Extensions;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Assertions;

namespace Hathora.Scripts.Utils.Editor.Auth0
{
    /// <summary>
    /// 1. Get device auth code from 
    /// </summary>
    public class Auth0Login
    {
        private const string clientId = "tWjDhuzPmuIWrI8R9s3yV3BQVw2tW0yq";
        private const string issuerUri = "https://auth.hathora.com";
        private const string audienceUri = "https://cloud.hathora.com";


        public async Task<string> GetTokenAsync(HathoraServerConfig hathoraServerConfig)
        {
            // Share the same path as the CLI
            string tokenPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config",
                "hathora",
                "token"
            );

            if (File.Exists(tokenPath))
            {
                if (!hathoraServerConfig.HathoraCoreOpts.DevAuthOpts.ForceNewToken)
                {
                    // TODO: Add a force refresh option (deletes the file)
                    Debug.Log($"A token file already present at {tokenPath}. We'll use this " +
                        "token, instead. If you'd like to get a new one, please remove this file.");
                
                    return await File.ReadAllTextAsync(tokenPath);
                }                
                
                // Delete this so we can make a new one
                File.Delete(tokenPath);
            }

            Auth0DeviceResponse deviceAuthorizationResponse = await requestDeviceAuthorizationAsync();

            if (deviceAuthorizationResponse == null)
            {
                Debug.Log("Error: Failed to get device authorization.");
                return null;
            }

            return await openBrowserAwaitAuth(deviceAuthorizationResponse, tokenPath);
        }

        private async Task<string> openBrowserAwaitAuth(
            Auth0DeviceResponse deviceAuthorizationResponse, 
            string tokenPath)
        {
            Debug.Log("Openening browser for login; ensure you see the " +
                $"following code: '<color=yellow>{deviceAuthorizationResponse.UserCode}</color>'.");

            // Open browser with the provided verification URI.
            Application.OpenURL(deviceAuthorizationResponse.VerificationUriComplete);

            string refreshToken = await pollForTokenAsync(deviceAuthorizationResponse.DeviceCode);

            if (refreshToken == null)
            {
                Debug.Log("Error: Failed to get tokens.");

                return null;
            }

            File.WriteAllText(tokenPath, refreshToken);
            Assert.AreEqual(refreshToken, File.ReadAllText(tokenPath)); // Sanity check
            Debug.Log($"Successfully logged in! Saved credentials to {tokenPath}");

            return refreshToken;
        }

        /// <summary>
        /// This simply POSTs for the request token code.
        /// After this, we pass the code to a uri for the end-user to login.
        /// </summary>
        /// <returns></returns>
        private async Task<Auth0DeviceResponse> requestDeviceAuthorizationAsync()
        {
            string url = $"{issuerUri}/oauth/device/code";
            UnityWebRequest request = new(url, "POST");

            Auth0DeviceRequest requestBody = new()
            {
                ClientId = clientId,
                Scope = "openid email offline_access",
                Audience = audienceUri,
            };

            // Convert the Auth0DeviceRequest object to JSON string using Newtonsoft.Json
            string bodyJson = JsonConvert.SerializeObject(requestBody);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJson);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest().AsTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Error: {request.error}");

                return null;
            }

            // Deserialize the response using Newtonsoft.Json
            return JsonConvert.DeserializeObject<Auth0DeviceResponse>(
                request.downloadHandler.text);
        }


        private async Task<string> pollForTokenAsync(string deviceCode)
        {
            string url = $"{issuerUri}/oauth/token";
            string refreshToken = null;

            while (refreshToken == null)
            {
                await Task.Delay(5000);

                using UnityWebRequest request = new(url, "POST");

                const string colon = "%3A";
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(
                    $"grant_type=urn{colon}ietf{colon}params{colon}oauth{colon}grant-type{colon}device_code" +
                    $"&device_code={deviceCode}&client_id={clientId}"
                );

                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

                await request.SendWebRequest().AsTask();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Auth0TokenResponse tokenResponse = JsonConvert.DeserializeObject<Auth0TokenResponse>(
                        request.downloadHandler.text);
                    
                    refreshToken = tokenResponse.RefreshToken;
                }
                else if (request.responseCode != 400 || !request.downloadHandler.text.Contains("authorization_pending"))
                {
                    Debug.Log($"Error: {request.error}");
                    return null;
                }
            }

            return refreshToken;
        }
    }
}
        
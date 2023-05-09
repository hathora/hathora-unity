// Created by dylan@hathora.dev

using System;
using System.IO;
using System.Threading.Tasks;
using Hathora.Scripts.Net.Server;
using Hathora.Scripts.Utils.Editor.Auth0.Models;
using Hathora.Scripts.Utils.Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace Hathora.Scripts.Utils.Editor.Auth0
{
    public class Auth0Login
    {
        private const string ClientId = "tWjDhuzPmuIWrI8R9s3yV3BQVw2tW0yq";
        private const string Auth0Issuer = "https://auth.hathora.com";
        private const string Audience = "https://cloud.hathora.com";


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
                if (hathoraServerConfig.HathoraCoreOpts.DevAuthOpts.forc)
                // TODO: Add a force refresh option (deletes the file)
                Debug.Log($"A token file already present at {tokenPath}. We'll use this " +
                    "token, instead. If you'd like to get a new one, please remove this file.");
                
                return await File.ReadAllTextAsync(tokenPath);
            }

            Auth0DeviceResponse deviceAuthorizationResponse = await requestDeviceAuthorizationAsync();

            if (deviceAuthorizationResponse == null)
            {
                Debug.Log("Error: Failed to get device authorization.");

                return null;
            }

            Debug.Log("Open browser for login? You should see the " +
                $"following code: {deviceAuthorizationResponse.UserCode}.");

            // Open browser with the provided verification URI.
            Application.OpenURL(deviceAuthorizationResponse.VerificationUriComplete);

            string refreshToken = await PollForTokenAsync(deviceAuthorizationResponse.DeviceCode);

            if (refreshToken == null)
            {
                Debug.Log("Error: Failed to get tokens.");

                return null;
            }

            File.WriteAllText(tokenPath, refreshToken);
            Debug.Log($"Successfully logged in! Saved credentials to {tokenPath}");

            return refreshToken;
        }

        private async Task<Auth0DeviceResponse> requestDeviceAuthorizationAsync()
        {
            string url = $"{Auth0Issuer}/oauth/device/code";
            UnityWebRequest request = new(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(
                $"client_id={ClientId}&scope=openid%20email%20offline_access&audience={Audience}"
            );

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            await request.SendWebRequest().AsTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Error: {request.error}");

                return null;
            }

            return JsonUtility.FromJson<Auth0DeviceResponse>(request.downloadHandler.text);
        }

        private async Task<string> PollForTokenAsync(string deviceCode)
        {
            string url = $"{Auth0Issuer}/oauth/token";
            string refreshToken = null;

            while (refreshToken == null)
            {
                await Task.Delay(5000);

                using UnityWebRequest request = new(url, "POST");

                const string colon = "%3A";
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(
                    $"grant_type=urn{colon}ietf{colon}params{colon}oauth{colon}grant-type{colon}device_code" +
                    $"&device_code={deviceCode}&client_id={ClientId}"
                );

                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

                await request.SendWebRequest().AsTask();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Auth0TokenResponse tokenResponse = JsonUtility.FromJson<Auth0TokenResponse>(request.downloadHandler.text);
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
        
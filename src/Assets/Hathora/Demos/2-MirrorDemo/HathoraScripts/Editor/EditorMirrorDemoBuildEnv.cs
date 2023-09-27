// Created by dylan@hathora.dev

using System;
using Hathora.Core.Scripts.Editor.Common;
using Hathora.Core.Scripts.Runtime.Client;
using Hathora.Core.Scripts.Runtime.Server;
using HathoraCloud.Models.Shared;
using kcp2k;
using Mirror;
using Mirror.SimpleWeb;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hathora.Demos._2_MirrorDemo.HathoraScripts.Editor
{
    /// <summary>
    /// Validates the correct env for building to certain targets.
    /// Ensure you're currently in the build scene that's supposed to be at the top.
    /// </summary>
    public static class EditorMirrorDemoBuildEnv
    {
        private static class MirrorDemoTransports
        {
            public static Type Kcp_Udp => typeof(KcpTransport);
            public static Type SimpleWebTransport_WssTls => typeof(SimpleWebTransport);
        }

        
        #region Menu
        /// <summary>Validates the WebGL-WSS-TLS env for building to WebGL, when using the vanilla Mirror Demo.</summary>
        [MenuItem("Hathora/Mirror Demo/Validate WebGL (WSS-TLS) Client Env")]
        public static void ValidateWebglWssTlsEnv()
        {
            string logPrefix = $"[EditorMirrorDemoBuildEnv.{nameof(ValidateWebglWssTlsEnv)}]";
            Debug.Log($"{logPrefix} Starting - <color=yellow>Expecting:</color>\n" +
                $"- {nameof(isCurrentSceneAtTopAndEnabled)}\n" +
                $"- {nameof(hasValidSceneClientAppId)}\n" +
                $"- {nameof(hasValidSceneServerAppId)}\n" +
                $"- {nameof(ensureClientServerAppIdsMatch)}\n" +
                $"- {nameof(ensureServerConfigTransportType)} ({nameof(TransportType.Tls)})\n" +
                $"- {nameof(ensureMirrorNetworkManagerTransport)} ({nameof(MirrorDemoTransports.SimpleWebTransport_WssTls)})");
            
            // Common
            Assert.IsTrue(isCurrentSceneAtTopAndEnabled(), $"{logPrefix} Expected {nameof(isCurrentSceneAtTopAndEnabled)}");
            Assert.IsTrue(hasValidSceneClientAppId(), $"{logPrefix} Expected {nameof(hasValidSceneClientAppId)}");
            Assert.IsTrue(hasValidSceneServerAppId(), $"{logPrefix} Expected {nameof(hasValidSceneServerAppId)}");
            Assert.IsTrue(ensureClientServerAppIdsMatch(), $"{logPrefix} Expected {nameof(ensureClientServerAppIdsMatch)}");
            // TODO: Ensure we're on WebGL Client platform target (Warn, since user may be planning to build Server; not Client)

            // Webgl (TLS/WSS) Specific
            Assert.IsTrue(ensureServerConfigTransportType(TransportType.Tls), 
                $"{logPrefix} Expected {nameof(ensureServerConfigTransportType)} ({nameof(TransportType.Tls)})");
            
            // Mirror Specific
            // TODO: Ensure Mirror NetworkManager port matches HathoraServerManager's (Warn, since it could be multi-port)
            
            // Mirror (TLS/WSS) Specific
            Assert.IsTrue(ensureMirrorNetworkManagerTransport(MirrorDemoTransports.SimpleWebTransport_WssTls), 
                $"{logPrefix} Expected {nameof(ensureMirrorNetworkManagerTransport)} ({nameof(MirrorDemoTransports.SimpleWebTransport_WssTls)})");
            // TODO: Ensure NetworkManager's "Start on Headless" is checked (Warn)
            // TODO: Ensure !SSL checked (Fatal)
            
            // Success
            Debug.Log($"{logPrefix} <color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>Passed.</color>");
        }
        
        /// <summary>Validates the !WebGL UDP env for building standalone clients, when using the vanilla Mirrror Demo.</summary>
        [MenuItem("Hathora/Mirror Demo/Validate Standalone (UDP) Client Env")]
        public static void ValidateStandaloneUdpEnv()
        {
            string logPrefix = $"[EditorMirrorDemoBuildEnv.{nameof(ValidateStandaloneUdpEnv)}]";
            Debug.Log($"{logPrefix} Starting - <color=yellow>Expecting:</color>\n" +
                $"- {nameof(isCurrentSceneAtTopAndEnabled)}\n" +
                $"- {nameof(hasValidSceneClientAppId)}\n" +
                $"- {nameof(hasValidSceneServerAppId)}\n" +
                $"- {nameof(ensureClientServerAppIdsMatch)}\n" +
                $"- {nameof(ensureServerConfigTransportType)} ({nameof(TransportType.Udp)})\n" +
                $"- {nameof(ensureMirrorNetworkManagerTransport)} ({nameof(MirrorDemoTransports.Kcp_Udp)})");

            // Common
            Assert.IsTrue(isCurrentSceneAtTopAndEnabled(), $"{logPrefix} Expected {nameof(isCurrentSceneAtTopAndEnabled)}");
            Assert.IsTrue(hasValidSceneClientAppId(), $"{logPrefix} Expected {nameof(hasValidSceneClientAppId)}");
            Assert.IsTrue(hasValidSceneServerAppId(), $"{logPrefix} Expected {nameof(hasValidSceneServerAppId)}");
            Assert.IsTrue(ensureClientServerAppIdsMatch(), $"{logPrefix} Expected {nameof(ensureClientServerAppIdsMatch)}");
            // TODO: Ensure we're on !WebGL Client platform target (Fatal)

            // Standalone (UDP) Specific
            Assert.IsTrue(ensureServerConfigTransportType(TransportType.Udp), 
                $"{logPrefix} Expected {nameof(ensureServerConfigTransportType)} ({nameof(TransportType.Udp)})");
            
            // Mirror Specific
            // TODO: Ensure Mirror NetworkManager port matches HathoraServerManager's (Warn, since it could be multi-port)
            
            // Mirror (UDP) Specific
            Assert.IsTrue(ensureMirrorNetworkManagerTransport(MirrorDemoTransports.Kcp_Udp), 
                $"{logPrefix} Expected {nameof(ensureMirrorNetworkManagerTransport)} ({nameof(MirrorDemoTransports.Kcp_Udp)})");
            
            // TODO: Ensure Mirror NetworkManager port matches HathoraServerManager's (Fatal)
            // TODO: Ensure Mirror NetworkManager's "Start on Headless" is checked (Warn)
            
            // Success
            Debug.Log($"{logPrefix} <color={HathoraEditorUtils.HATHORA_GREEN_COLOR_HEX}>Passed.</color>");
        }
        #endregion // Menu
        
        
        #region Common Utils
        /// <summary>
        /// Ensures that the TransportType of the ServerManager is {transportType}.
        /// </summary>
        /// <returns>True if ServerManager's TransportType is {transportType}</returns>
        private static bool ensureServerConfigTransportType(TransportType _transportType)
        {
            string logPrefix = $"[EditorMirrorDemoBuildEnv.{nameof(ensureServerConfigTransportType)}]";

            // Find HathoraServerMgr in the scene (that contains HathoraServerConfig field)
            HathoraServerMgr serverMgr = GameObject.FindObjectOfType<HathoraServerMgr>();
            if (serverMgr == null)
            {
                Debug.LogError($"{logPrefix} {nameof(HathoraServerMgr)} not found in the open scene");
                return false;
            }
            
            // Check if HathoraServerConfig is set
            if (serverMgr.HathoraServerConfig == null)
            {
                Debug.LogError($"{logPrefix} {nameof(HathoraServerConfig)} field is not serialized in {nameof(HathoraServerMgr)}");
                return false;
            }
            
            HathoraServerConfig serverConfig = serverMgr.HathoraServerConfig;

            // Check if TransportType is set and of type Tls
            if (serverConfig == null)
            {
                Debug.LogError($"{logPrefix} {nameof(HathoraServerConfig)} field is not serialized in {nameof(HathoraServerMgr)}");
                return false;
            }

            if (serverConfig.HathoraDeployOpts.TransportType != _transportType)
            {
                Debug.LogError($"{logPrefix} HathoraServerConfig TransportType<{serverConfig.HathoraDeployOpts.TransportType}> " +
                    $"is not of expected type<{_transportType}>");
                
                return false;
            }

            return true; // isValidServerConfigTransportType
        }
        
        /// <summary>
        /// Ensures the Transport in TransportManager is of type {transpoprt}.
        /// </summary>
        /// <returns>True if {transport} is set as the Transport.</returns>
        private static bool ensureMirrorNetworkManagerTransport(Type _transportType)
        {
            string logPrefix = $"[EditorMirrorDemoBuildEnv.{nameof(ensureMirrorNetworkManagerTransport)}]";

            // Find NetworkManagger in the scene
            NetworkManager mirrorNetworkMgr = GameObject.FindObjectOfType<NetworkManager>();
            if (mirrorNetworkMgr == null)
            {
                Debug.LogError($"{logPrefix} Mirror `{nameof(NetworkManager)}` not found in the scene");
                return false;
            }

            // Check if Transport is set and of type {transport}
            if (mirrorNetworkMgr.transport == null)
            {
                Debug.LogError($"{logPrefix} Transport field is not serialized in " +
                    $"{nameof(mirrorNetworkMgr)}.{nameof(mirrorNetworkMgr.transport)}");
                
                return false;
            }

            Type networkMgrTransportType = mirrorNetworkMgr.transport.GetType();
            if (networkMgrTransportType != _transportType)
            {
                Debug.LogError($"{logPrefix} The set transport<{networkMgrTransportType.Name}> is not " +
                    $"of expected type<{_transportType.Name}>");
                return false;
            }

            return true;  // Transport is of type SWT
        }
        #endregion // Common Utils

        
        /// <summary>
        /// Ensures the AppIds in HathoraClientConfig and HathoraServerConfig match.
        /// </summary>
        /// <returns>True if the AppIds match</returns>
        private static bool ensureClientServerAppIdsMatch()
        {
            string logPrefix = $"[EditorMirrorDemoBuildEnv.{nameof(ensureClientServerAppIdsMatch)}]";

            // Find HathoraClientMgr and its config
            HathoraClientMgr clientMgr = GameObject.FindObjectOfType<HathoraClientMgr>();
            if (clientMgr == null || clientMgr.HathoraClientConfig == null)
            {
                Debug.LogError($"{logPrefix} HathoraClientMgr or HathoraClientConfig not found");
                return false;
            }
            string clientAppId = clientMgr.HathoraClientConfig.AppId;

            // Find HathoraServerMgr and its config
            HathoraServerMgr serverMgr = GameObject.FindObjectOfType<HathoraServerMgr>();
            if (serverMgr == null || serverMgr.HathoraServerConfig == null)
            {
                Debug.LogError($"{logPrefix} HathoraServerMgr or HathoraServerConfig not found");
                return false;
            }
            
            string serverAppId = serverMgr.HathoraServerConfig.HathoraCoreOpts.AppId;

            // Compare AppIds
            if (string.IsNullOrEmpty(clientAppId) || string.IsNullOrEmpty(serverAppId))
            {
                Debug.LogError($"{logPrefix} AppId in HathoraClientConfig or HathoraServerConfig is null or empty");
                return false;
            }

            if (clientAppId != serverAppId)
            {
                Debug.LogError($"{logPrefix} AppIds do not match between client and server");
                return false;
            }

            return true;  // AppIds match
        }

        
        /// <summary>
        /// Checks the scene for a HathoraServerMgr component, then ensures that the HathoraServerConfig field
        /// is serialized with a valid AppId.
        /// </summary>
        /// <returns>True if HathoraServerConfig has a valid AppId, otherwise false.</returns>
        private static bool hasValidSceneServerAppId()
        {
            string logPrefix = $"[EditorMirrorDemoBuildEnv.{nameof(hasValidSceneServerAppId)}]";

            // Get all GameObjects in the current scene.
            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

            if (allGameObjects.Length == 0)
            {
                Debug.LogError($"{logPrefix}  No GameObjects found in the current scene");
                return false;
            }

            foreach (GameObject go in allGameObjects)
            {
                HathoraServerMgr serverMgr = go.GetComponent<HathoraServerMgr>();

                if (serverMgr == null)
                    continue;

                if (serverMgr.HathoraServerConfig == null)
                {
                    Debug.LogError($"{logPrefix} HathoraServerConfig field is not serialized");
                    return false;
                }

                if (serverMgr == null || 
                    serverMgr.HathoraServerConfig == null || 
                    string.IsNullOrEmpty(serverMgr.HathoraServerConfig.HathoraCoreOpts?.AppId))
                {
                    Debug.LogError($"{logPrefix} AppId field in HathoraServerConfig is null or empty");
                    return false;
                }

                return true;
            }

            Debug.LogError($"{logPrefix} HathoraServerMgr component not found in any GameObject");
            return false;
        }


        /// <summary>
        /// Checks the scene for a HathoraClientManager component -> Then checks that the HathoraClientConfig field
        /// is serialized with 
        /// </summary>
        /// <returns></returns>
        private static bool hasValidSceneClientAppId()
        {
            string logPrefix = $"[EditorMirrorDemoBuildEnv.{nameof(hasValidSceneClientAppId)}]";

            // Get all GameObjects in the current scene.
            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

            if (allGameObjects.Length == 0)
            {
                Debug.LogError($"{logPrefix} No GameObjects found in the current scene");
                return false;
            }

            foreach (GameObject go in allGameObjects)
            {
                HathoraClientMgr clientMgr = go.GetComponent<HathoraClientMgr>();
        
                if (clientMgr == null)
                    continue;
        
                if (clientMgr.HathoraClientConfig == null)
                {
                    Debug.LogError($"{logPrefix} HathoraClientConfig field is not serialized");
                    return false;
                }

                if (string.IsNullOrEmpty(clientMgr.HathoraClientConfig.AppId))
                {
                    Debug.LogError($"{logPrefix} AppId field in HathoraClientConfig is null or empty");
                    return false;
                }

                return true;
            }

            Debug.LogError($"{logPrefix} HathoraClientMgr component not found in any GameObject");
            return false;
        }

        /// <summary>
        /// Checks if the current scene is at the top of the build settings and enabled.
        /// </summary>
        /// <returns>True if the current scene is at the top and enabled</returns>
        private static bool isCurrentSceneAtTopAndEnabled()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

            // Check if buildScenes has any elements first.
            if (buildScenes.Length == 0)
                return false;
        
            EditorBuildSettingsScene topScene = buildScenes[0];
            return topScene.path == currentScene.path && topScene.enabled;
        }
    }
}

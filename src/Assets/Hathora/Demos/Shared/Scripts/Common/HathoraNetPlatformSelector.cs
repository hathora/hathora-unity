// Created by dylan@hathora.dev

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hathora.Demos.Shared.Scripts.Common
{
    /// <summary>
    /// "Select a NetCode Platform": Fishnet | Mirror | NGO
    /// </summary>
    public class HathoraNetPlatformSelector : MonoBehaviour
    {
        [Serializable]
        public enum NetPlatform
        {
            FishNet,
            Mirror,
            NGO, // TODO
        }
        
        public void OnFishnetBtnClick() => 
            OnPlatformSelectBtnClick(NetPlatform.FishNet);
        
        public void OnMirrorBtnClick() => 
            OnPlatformSelectBtnClick(NetPlatform.Mirror);
        
        public void OnNGOBtnClick() =>
            OnPlatformSelectBtnClick(NetPlatform.NGO);

        private void OnPlatformSelectBtnClick(NetPlatform _platform)
        {
            const string logPrefix = "[HathoraNetPlatformSelector] OnPlatformSelectBtnClick:";
            
            switch (_platform)
            {
                case NetPlatform.FishNet:
                    Debug.Log($"{logPrefix} FishNet");
                    _ = loadSceneAsync("HathoraDemoScene-FishNet");
                    break;

                case NetPlatform.Mirror:
                    Debug.Log($"{logPrefix} Mirror");
                    _ = loadSceneAsync("HathoraDemoScene-Mirror");
                    break;

                case NetPlatform.NGO:
                    Debug.Log($"{logPrefix} NGO");
                    // loadSceneAsync("HathoraDemoScene-UnityNGO");
                    throw new NotImplementedException("TODO");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_platform), _platform, null);
            }
        }
        
        // Load scene based on NetPlatform
        private static async Task loadSceneAsync(string _sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
                _sceneName, 
                LoadSceneMode.Single);

            while (!asyncLoad.isDone)
                await Task.Yield();
        }
        
        
    }
}

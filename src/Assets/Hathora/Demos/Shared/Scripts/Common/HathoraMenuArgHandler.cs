// Created by dylan@hathora.dev

using UnityEngine;

namespace Hathora.Demos.Shared.Scripts.Common
{
    /// <summary>This scene only supports `-scene`</summary>
    public class HathoraMenuArgHandler : HathoraArgHandlerBase
    {
        protected override void InitMemo(string _memoStr)
        {
            // base.InitMemo(_memoStr);
        }

        protected override void StartClient()
        {
            Debug.LogError("[HathoraMenuArgHandler.StartClient] " +
                "You tried to start a Client from -scene menu -- which platform to use?");
        }
    
        protected override void StartServer()
        {
            Debug.LogError("[HathoraMenuArgHandler.StartServer] " +
                "You tried to start a Server from -scene menu -- which platform to use?");
        }
    
        protected override void StartHost()
        {
            Debug.LogError("[HathoraMenuArgHandler.StartHost] " +
                "You tried to start a Host from -scene menu -- which platform to use?");
        }
        
        protected override void InitScene(string _sceneName)
        {
            if (_sceneName.EndsWith("Menu"))
                Debug.LogError($"[HathoraMenuArgHandler.InitScene] {_sceneName}");
            
            base.InitScene(_sceneName);
        }
    }
}

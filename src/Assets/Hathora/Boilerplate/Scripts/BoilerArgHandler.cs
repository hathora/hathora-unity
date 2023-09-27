// Created by dylan@hathora.dev

using Hathora.Demos.Shared.Scripts.Common;
using UnityEngine;

namespace Hathora.Demos.Boilerplate.Scripts
{
    public class BoilerArgHandler : HathoraArgHandlerBase
    {
        protected override void Awake() {}
        protected override void Start() {}
        
        protected override void ArgModeStartServer()
        {
            base.ArgModeStartServer();

            bool alreadyStartedServer = false; // TODO: Check your NetworkManager to ensure it's not already started
            if (alreadyStartedServer)
                return;
            
            Debug.Log($"[{nameof(BoilerArgHandler)}] {nameof(ArgModeStartServer)}");
            // NetworkManager.Instance.StartServer(); // TODO
        }

        protected override void ArgModeStartClient()
        {
            base.ArgModeStartClient();

            bool alreadyStartedClient = false; // TODO: Check your NetworkManager to ensure it's not already started
            if (alreadyStartedClient)
                return;
            
            Debug.Log($"[{nameof(BoilerArgHandler)}] {nameof(ArgModeStartClient)}");
            // NetworkManager.Instance.StartClient(); // TODO
        }

        protected override void ArgModeStartHost()
        {
            base.ArgModeStartHost();
            
            bool alreadyStartedClientOrServer = false; // TODO: Check your NetworkManager to ensure it's not already started
            if (alreadyStartedClientOrServer)
                return;
            
            Debug.Log($"[{nameof(BoilerArgHandler)}] {nameof(ArgModeStartClient)} (Server+Client)");
            // NetworkManager.Instance.StartHost(); // TODO
        }
    }
}

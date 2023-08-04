// Created by dylan@hathora.dev

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hathora.Demos.Shared.Scripts.Client.ClientMgr
{
    /// <summary>
    /// Only for use with the Hello World UI. Most component events are
    /// currently handled within component Event click logic (unlike the SDK demo).
    /// </summary>
    public class HathoraClientMgrHelloWorldDemoUi : MonoBehaviour
    {
        #region Vars
        [Header("Hello World Demo")]
        [SerializeField, Tooltip("When we connect as a Client, this is the optional host:ip optionally passed along")]
        private TMP_InputField clientConnectInputField;
        public TMP_InputField ClientConnectInputField => clientConnectInputField;

        [SerializeField, Tooltip("WebGL builds should hide this")]
        private Button netStartHostBtn;
        #endregion // Vars

        
        private void Awake()
        {
            #if UNITY_WEBGL
            Debug.Log("[HathoraClientMgrHelloWorldDemoUi.Awake] Hiding netStartHostBtn for WebGL builds");
            netStartHostBtn.gameObject.SetActive(false);
            #endif
        }
    }
}

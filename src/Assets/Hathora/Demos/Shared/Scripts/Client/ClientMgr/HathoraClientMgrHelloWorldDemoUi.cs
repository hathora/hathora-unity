// Created by dylan@hathora.dev

using TMPro;
using UnityEngine;

namespace Hathora.Demos.Shared.Scripts.Client.ClientMgr
{
    /// <summary>
    /// Only for use with the Hello World UI. Most component events are
    /// currently handled within component Event click logic (unlike the SDK demo).
    /// </summary>
    public class HathoraClientMgrHelloWorldDemoUi : MonoBehaviour
    {
        [Header("Hello World Demo")]
        [SerializeField, Tooltip("When we connect as a Client, this is the optional host:ip optionally passed along")]
        private TMP_InputField clientConnectInputField;
        public TMP_InputField ClientConnectInputField => clientConnectInputField;
    }
}

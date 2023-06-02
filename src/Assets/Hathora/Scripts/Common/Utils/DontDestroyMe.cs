// Created by dylan@hathora.dev

using UnityEngine;

namespace Hathora.Scripts.Common.Utils
{
    /// <summary>
    /// Only add this to ROOT objects that you want to persist across scenes.
    /// </summary>
    public class DontDestroyMe : MonoBehaviour
    {
        private void Awake() =>
            DontDestroyOnLoad(gameObject);
    }
}

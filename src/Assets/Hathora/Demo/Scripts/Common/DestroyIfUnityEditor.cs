// Created by dylan@hathora.dev

using UnityEngine;

namespace Hathora.Demo.Scripts.Common
{
    public class DestroyIfUnityEditor : MonoBehaviour
    {
        private void Awake()
        {   
#if UNITY_EDITOR
            Destroy(gameObject);
#endif
        }
    }
}

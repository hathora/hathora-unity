// Created by dylan@hathora.dev

using Hathora.Net.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hathora.Net
{
    /// <summary>
    /// Handles the non-Player UI so we can keep the logic separate.
    /// </summary>
    public class NetUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI debugMemoTxt;
        
        public static NetUI Singleton;

        private void Awake()
        {
            setSingleton();
            DontDestroyOnLoad(gameObject);
        }

        private void setSingleton()
        {
            if (Singleton != null)
                Destroy(gameObject);

            Singleton = this;
        }

        public void SetShowDebugMemoTxt(string memoStr)
        {
            debugMemoTxt.text = memoStr;
            debugMemoTxt.gameObject.SetActive(true);
            Debug.Log($"[NetCmdLine] Debug Memo: '{memoStr}'");
        }


    }
}

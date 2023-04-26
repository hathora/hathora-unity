// Created by dylan@hathora.dev

using TMPro;
using UnityEngine;

namespace Hathora.Net
{
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

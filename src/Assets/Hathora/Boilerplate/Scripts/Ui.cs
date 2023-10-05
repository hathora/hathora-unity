// Created by dylan@hathora.dev

using TMPro;
using UnityEngine;

namespace HathoraBoiler.Scripts
{
    /// <summary>UI Event handlers to forward to logic</summary>
    public class Ui : MonoBehaviour
    {
        #region Vars
        [SerializeField, Tooltip("Initially hide (or leave empty), then show when connected/started")]
        private TextMeshProUGUI clientStartedTxt;
        #endregion // Vars
        
        
        #region Init
        /// <summary>
        /// Subscribe to StateMgr events so we can handle UI as NetworkManager states change.
        /// </summary>
        private void Start()
        {
            StateMgr.OnClientStartedEvent += OnClientStarted;
            // TODO: ^ If you add more, don't forget to cleanup (-=) at OnDestroy()
        }

        private void OnClientStarted()
        {
            clientStartedTxt.text = "Client Started!";
            clientStartedTxt.gameObject.SetActive(true);
        }
        #endregion // Init
        
        
        #region UI Interactions
        public void OnStartServerBtnClick()
        {
            Debug.Log($"[{nameof(Ui)}] {nameof(OnStartServerBtnClick)}");
            StateMgr.Singleton.StartServer();
        }

        public void OnStartClientBtnClick()
        {
            Debug.Log($"[{nameof(Ui)}] {nameof(OnStartClientBtnClick)}");
            StateMgr.Singleton.StartClient();
        }
        #endregion UI Interactions
        
        
        #region Utils
        /// <summary>Dispose of events we subbed to at Start()</summary>
        private void OnDestroy()
        {
            StateMgr.OnClientStartedEvent -= OnClientStarted;
        }
        #endregion // Utils
    }
}

using UnityEngine;

namespace Hathora.Utils
{
    public class HidePlayerOnEscape : MonoBehaviour
    {
        private bool playerVisible = true;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePlayerVisibility();
        }

        void TogglePlayerVisibility()
        {
            playerVisible = !playerVisible;
            gameObject.SetActive(playerVisible);
        }
    }
}
using Hathora.Scripts.Net.Server;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor
{
    [CustomEditor(typeof(HathoraServerConfig))]
    public class HathoraConfigObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Display the banner image at the top
            Texture2D bannerTexture = Resources.Load<Texture2D>("HathoraConfigBanner");
            if (bannerTexture != null)
            {
                GUILayout.Label(bannerTexture);
                GUILayout.Space(10);
            }

            // Display the default ScriptableObject inspector
            DrawDefaultInspector();
        }
    }
}

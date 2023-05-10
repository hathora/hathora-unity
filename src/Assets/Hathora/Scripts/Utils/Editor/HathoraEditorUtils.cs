// Created by dylan@hathora.dev

using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor
{
    /// <summary>
    /// Editor script to add the Hathora banner to an editor window
    /// </summary>
    public static class HathoraEditorUtils
    {
        /// <summary>
        /// Aligns right. Shrinks to fit.
        /// </summary>
        public static void InsertBanner()
        {
            Texture2D bannerTexture = Resources.Load<Texture2D>("HathoraConfigBanner");
            if (bannerTexture == null)
                return;
        
            float windowWidth = EditorGUIUtility.currentViewWidth;
            float bannerWidth = bannerTexture.width;
            float bannerHeight = bannerTexture.height;

            float maxBannerWidth = windowWidth * 0.9f;
            if (bannerWidth > maxBannerWidth)
            {
                float scale = maxBannerWidth / bannerWidth;
                bannerWidth = maxBannerWidth;
                bannerHeight *= scale;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(bannerTexture, GUILayout.Width(bannerWidth), GUILayout.Height(bannerHeight));
            EditorGUILayout.EndHorizontal();
        }
    }
}

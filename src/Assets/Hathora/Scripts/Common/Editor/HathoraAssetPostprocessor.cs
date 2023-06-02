// Created by dylan@hathora.dev

using System.Linq;
using Hathora.Scripts.Server.Config;
using UnityEditor;

namespace Hathora.Scripts.Common.Editor
{
    public class HathoraAssetPostprocessor : AssetPostprocessor
    {
        /// <summary>
        /// If we add a new Config, refresh the Config Finder window
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        /// <returns></returns>
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // Check if any of the imported, moved, or deleted assets are of the HathoraClientConfig type
            bool configChanged = importedAssets.Concat(movedAssets).Concat(deletedAssets).Any(
                assetPath => AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(HathoraClientConfig));

            if (configChanged)
            {
                // If a config has changed, refresh the config list and repaint the window
                HathoraServerConfigFinder.RefreshConfigsAndRepaint();
            }
        }
    }
}

// Created by dylan@hathora.dev

using System.Linq;
using Hathora.Scripts.Net.Common;
using UnityEditor;

namespace Hathora.Scripts.Utils.Editor
{
    public class HathoraAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // Check if any of the imported, moved, or deleted assets are of the NetHathoraConfig type
            bool configChanged = importedAssets.Concat(movedAssets).Concat(deletedAssets).Any(
                assetPath => AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(NetHathoraConfig));

            if (configChanged)
            {
                // If a config has changed, refresh the config list and repaint the window
                HathoraServerConfigFinder.RefreshConfigsAndRepaint();
            }
        }
    }
}

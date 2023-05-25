// Created by dylan@hathora.dev

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Hathora.Scripts.SdkWrapper.Models;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Hathora.Scripts.Utils.Editor
{
    /// <summary>
    /// Editor script to add the Hathora banner to an editor window
    /// </summary>
    public static class HathoraEditorUtils
    {
        #region Links
        public const string HATHORA_HOME_URL = "https://hathora.dev";
        public const string HATHORA_DOCS_URL = "https://docs.hathora.dev";
        public const string HATHORA_DOCS_DEMO_PROJECTS_URL = "https://github.com/hathora";
        public const string HATHORA_DOCS_GETTING_STARTED_URL = "https://hathora.dev/docs/get-started";
        public const string HATHORA_DOCS_UNITY_TUTORIAL_URL = "https://github.com/hathora/hathora-unity";
        public const string HATHORA_DISCORD_URL = "https://discord.gg/hathora";
        #endregion // Links
        
        
        #region Style/Color
        // HEX OPACITY GUIDE >>
        // ##########################################
        // 0%: 00 (fully transparent)
        // 20%: 33
        // 40%: 66
        // 60%: 99
        // 80%: CC
        // 100%: FF (fully opaque)
        // ##########################################
        
        public const string HATHORA_GREEN_COLOR_HEX = "#76FDBA";
        public const string HATHORA_VIOLET_COLOR_HEX = "#EEDDFF";
        public const string HATHORA_DARK_INDIGO_COLOR_HEX = "#20124D";
        public const string HATHORA_GRAY_TRANSPARENT_COLOR_HEX = "#919191CC"; // 60% opacity
        public const string HATHORA_LINK_COLOR_HEX = "#0000EE";
        
        public static GUIStyle GetRichFoldoutHeaderStyle() => new(EditorStyles.foldoutHeader) { richText = true };
        public static readonly RectOffset DefaultPadding = new(left: 1, right: 1, top: 0, bottom: 0);
        public static readonly RectOffset DefaultBtnPadding = new(left: 10, right: 10, top: 5, bottom: 5);
        public static readonly RectOffset NoPadding = new(left: 0, right: 0, top: 0, bottom: 0);
        public static readonly RectOffset DefaultMargin = new(left: 3, right: 3, top: 2, bottom: 2);
        public static readonly RectOffset NoMargin = new(left: 0, right: 0, top: 0, bottom: 0);
        
        public static GUIStyle GetRichButtonStyle(
            int _fontSize = 13, 
            bool _wordWrap = true) => new(GUI.skin.button)
        {
            richText = true,
            fontSize = _fontSize,
            wordWrap = _wordWrap,
            // margin = DefaultMargin,
            padding = DefaultBtnPadding,
        };        
        public static GUIStyle GetBigButtonStyle(
            int _fontSize = 13, 
            bool _wordWrap = true) => new(GUI.skin.button)
        {
            richText = true,
            fontSize = _fontSize,
            wordWrap = _wordWrap,
            // margin = DefaultMargin,
            // padding = DefaultPadding,
            fixedHeight = 50,
        };
        
        public static GUIStyle GetRichLabelStyle(
            TextAnchor _align, 
            bool _wordWrap = true,
            int _fontSize = 13) => new(EditorStyles.label)
        {
            richText = true,
            alignment = _align,
            wordWrap = _wordWrap,
            fontSize = _fontSize,
            // margin = DefaultMargin,
            padding = DefaultPadding,
        };

        /// <summary>
        /// No padding on right
        /// </summary>
        /// <returns></returns>
        public static GUIStyle GetPreLinkLabelStyle()
        {
            GUIStyle style = new(EditorStyles.label)
            {
                margin = DefaultMargin,
                padding = DefaultMargin,
                wordWrap = true,
                richText = true,
            };
            
            style.padding.right = 0;

            return style;
        }

        /// <summary>
        /// There's no native label links, so we fake it.
        /// </summary>
        /// <returns></returns>
        public static GUIStyle GetRichLinkStyle(TextAnchor _align) => new(GUI.skin.label)
        {
            normal =
            {
                textColor = HexToColor(HATHORA_VIOLET_COLOR_HEX),
            },
            fontStyle = FontStyle.Bold,
            alignment = _align,
            richText = true,
            margin = DefaultMargin,
            padding = DefaultPadding,
        };
        
        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte a = 255;

            if (hex.Length == 8)
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32(r, g, b, a);
        }
        #endregion // Style/Color
        

        /// <summary>
        /// Add 
        /// --
        /// Wrapped in a Rect. Aligns center. Dark bg on transparent img,
        /// showing an illusion of dynamic size. Shrinks to fit.
        /// </summary>
        /// <param name="_wrapperExtension">Extend the black banner down</param>
        /// <param name="_labelPadding">Push the content below down further</param>
        /// <param name="_includeVerticalGroup">
        /// (!) If not, include `GUILayout.EndVertical()` shortly after this call.
        /// You would set it to `false` if you wanted to include middleware in
        /// between to add labels/buttons INSIDE the banner.
        /// </param>
        public static void InsertBanner(
            float _wrapperExtension = 3, 
            float _labelPadding = 10f,
            bool _includeVerticalGroup = true)
        {
            Texture2D bannerTexture = Resources.Load<Texture2D>("HathoraConfigBanner");
            if (bannerTexture == null)
                return;

            float windowWidth = EditorGUIUtility.currentViewWidth;
            float bannerWidth = bannerTexture.width;
            float bannerHeight = bannerTexture.height;

            const float minBannerWidth = 200f; // Set the minimum banner width
            float maxBannerWidth = Mathf.Max(windowWidth * 0.5f, minBannerWidth);
            if (bannerWidth > maxBannerWidth)
            {
                float scale = maxBannerWidth / bannerWidth;
                bannerWidth = maxBannerWidth;
                bannerHeight *= scale;
            }

            // Calculate the banner's position and size
            const float bannerBgPaddingTop = 5f;
            const float bannerBgPaddingBottom = 7f;

            float bannerX = (windowWidth - bannerWidth) * 0.5f; // center
            float bannerY = bannerBgPaddingTop;

            // Create the padded rect with extended wrapper
            float adjustedBannerHeight = bannerHeight + bannerBgPaddingTop + bannerBgPaddingBottom + _wrapperExtension;
            Rect paddedRect = new(
                x: 0,
                y: 0,
                width: windowWidth,
                height: adjustedBannerHeight);

            // Draw a dark background for the entire horizontal area with padding
            Color indigoColor = HexToColor(HATHORA_DARK_INDIGO_COLOR_HEX);
            EditorGUI.DrawRect(paddedRect, indigoColor);

            // Draw the banner texture centered within the padded rect
            Rect bannerRect = new(bannerX, bannerY, bannerWidth, bannerHeight);
            GUI.DrawTexture(bannerRect, bannerTexture);

            GUILayout.BeginVertical(GUILayout.Height(paddedRect.height));
            GUILayout.Space(bannerHeight + _labelPadding);
            
            if (_includeVerticalGroup)
                GUILayout.EndVertical();
        }
        
        public static void InsertHathoraSloganLbl()
        {
            GUILayout.Label("Multiplayer Server Hosting", 
                GetRichLabelStyle(_align: TextAnchor.MiddleCenter));
        }

        /// <summary>
        /// Useful for 7z compression handling.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static async Task<string> ExecuteCrossPlatformShellCmdAsync(string cmd, string args)
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string shell = isWindows ? "cmd.exe" : "/bin/bash";
            string escapedArgs = isWindows 
                ? $"/c {cmd} {args}" 
                : $"-c \"{cmd} {args}\"";
            
            Debug.Log($"[HathoraEditorUtils.ExecuteCrossPlatformShellCmdAsync] " +
                $"shell: {shell}, cmd args: <color=yellow>`{cmd} {args}`</color>");

            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = shell,
                    Arguments = escapedArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            process.Start();

            // Read the output and error asynchronously
            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            // Wait for the process to finish and read the output and error
            await process.WaitForExitAsync();

            // Combine output and error
            string output = await outputTask;
            string error = await errorTask;
            string result = output + error;

            return result;
        }

        
        #region 7z Utils
        /// <summary>
        /// - Uses the included utils @ project root `/.hathora/7zip/`
        /// - Adds /.hathora/Dockerfile
        /// - Adds /{buildDir}
        /// </summary>
        /// <param name="deployPaths">Path info</param>
        /// <param name="filesToCompress"></param>
        public static async Task TarballDeployFilesVia7zAsync(
            HathoraDeployPaths deployPaths,
            List<string> filesToCompress)
        {
            // file [ Dockerfile, {buildDir} ] >> file.tar
            string pathToOutputTar = await compressWithTarVia7zAsync(deployPaths, filesToCompress);
            
            // file.tar >> file.tar.gz ("Tarball")
            string pathToOutputTarGz = await compressTarAsGzVia7zAsync(
                deployPaths, 
                pathToOutputTar,
                _deleteOldTar: true);

            // Assert the tarball exists
            Assert.IsTrue(File.Exists(pathToOutputTarGz),
                $"[HathoraEditorUtils.TarballDeployFilesVia7zAsync] Expected {pathToOutputTarGz} to exist");
        }

        /// <summary>
        /// Turns file.tar into file.tar.gz ("Gzipped" / "Tarball").
        /// </summary>
        /// <param name="_deployPaths"></param>
        /// <param name="_pathToOutputTar"></param>
        /// <param name="_deleteOldTar"></param>
        /// <returns></returns>
        private static async Task<string> compressTarAsGzVia7zAsync(
            HathoraDeployPaths _deployPaths,
            string _pathToOutputTar, 
            bool _deleteOldTar)
        {
            string pathToOutputTarGz = $"{_pathToOutputTar}.gz";
            string gzipArgs = $@"a -tgzip ""{pathToOutputTarGz}"" ""{_pathToOutputTar}""";
            
            string gzipResultLogs = await ExecuteCrossPlatformShellCmdAsync(
                _deployPaths.PathTo7zCliExe, 
                gzipArgs);
            
            // TODO: if (verboseLogs)
            Debug.Log($"[HathoraEditorUtils.compressTarAsGzVia7zAsync] " +
                $"tarResultLogs:\n<color=yellow>{gzipResultLogs}</color>");

            if (_deleteOldTar)
                File.Delete(_pathToOutputTar);

            return pathToOutputTarGz;
        }

        /// <summary>
        /// - Uses the included utils @ project root `/.hathora/7zip/`
        /// - Adds /.hathora/Dockerfile
        /// - Adds /{buildDir}
        /// - You generally want to .gz, after, to create a tarball.
        /// </summary>
        /// <param name="deployPaths"></param>
        /// <param name="filesToCompress"></param>
        /// <returns>"path/to/output.tar"</returns>
        private static async Task<string> compressWithTarVia7zAsync(
            HathoraDeployPaths deployPaths, 
            List<string> filesToCompress)
        {
            string pathToOutputTar = $"{deployPaths.TempDirPath}/{deployPaths.ExeBuildName}.tar";
            string joinedFilesToCompress = string.Join(@""" """, filesToCompress);
            // const string excludePattern = @"-x !*\*DoNotShip\*";
            string tarArgs = $@"a -ttar ""{pathToOutputTar}"" ""{joinedFilesToCompress}""";
            
            string tarResultLogs = await ExecuteCrossPlatformShellCmdAsync(
                deployPaths.PathTo7zCliExe, 
                tarArgs);
            
            // TODO: if (verboseLogs)
            Debug.Log($"[HathoraEditorUtils.compressWithTarVia7zAsync] " +
                $"tarResultLogs:\n<color=yellow>{tarResultLogs}</color>");

            Assert.IsNotNull(tarResultLogs, "[HathoraEditorUtils.compressWithTarVia7zAsync] " +
                $"Error while creating tar archive: {tarResultLogs}");
            
            return pathToOutputTar;
        }
        #endregion // 7z Utils

    }
}

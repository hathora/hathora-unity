// Created by dylan@hathora.dev

using System;
using Hathora.Scripts.Net.Common;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public abstract class HathoraConfigUIBase
    {
        #region Vars
        /// <summary>The selected Config instance</summary>
        protected NetHathoraConfig Config { get; private set; }
        
        /// <summary>Do we have an Auth token?</summary>
        protected bool IsAuthed => 
            Config.HathoraCoreOpts.DevAuthOpts.HasAuthToken;
        
        protected GUIStyle CenterAlignLabelStyle;
        protected GUIStyle CenterAlignLargerTxtLabelNoWrapStyle;
        protected GUIStyle LeftAlignLabelStyle;
        protected GUIStyle CenterLinkLabelStyle;
        protected GUIStyle RightAlignLabelStyle;
        protected GUIStyle PreLinkLabelStyle;
        protected GUIStyle GeneralButtonStyle;
        protected GUIStyle BigButtonStyle;
        protected GUIStyle BtnsFoldoutStyle;
        
        public event Action RequestRepaint;
        #endregion // Vars

        
        #region Init
        protected HathoraConfigUIBase(NetHathoraConfig _config)
        {
            this.Config = _config;
            initStyles();
        }

        private void initStyles()
        {
            initButtonStyles();
            initBtnFoldoutStyles();
            initLabelStyles();
        }
        
        /// <summary>
        /// Adds padding, rich text, and sets font size to 13.
        /// </summary>
        private void initButtonStyles()
        {
            this.GeneralButtonStyle ??= HathoraEditorUtils.GetRichButtonStyle();
            this.BigButtonStyle ??= HathoraEditorUtils.GetBigButtonStyle();
        }
 
        private void initBtnFoldoutStyles()
        {
            this.BtnsFoldoutStyle ??= HathoraEditorUtils.GetRichFoldoutHeaderStyle();
        }
        
        private void initLabelStyles()
        {
            this.LeftAlignLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleLeft);
            this.CenterAlignLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleCenter);
            this.RightAlignLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleRight);
            this.CenterLinkLabelStyle ??= HathoraEditorUtils.GetRichLinkStyle(TextAnchor.MiddleCenter);
            this.PreLinkLabelStyle ??= HathoraEditorUtils.GetPreLinkLabelStyle();
            this.CenterAlignLargerTxtLabelNoWrapStyle ??= HathoraEditorUtils.GetRichLabelStyle(
                TextAnchor.MiddleCenter,
                _wordWrap: false,
                _fontSize: 15);
        }
        #endregion // Init

        
        /// <summary>
        /// Add to this event to request a repaint from the main editor UI.
        /// </summary>
        protected void InvokeRequestRepaint() =>
            RequestRepaint?.Invoke();
        
        protected void DrawHorizontalLine(float thickness, Color color)
        {
            Rect lineRect = EditorGUILayout.GetControlRect(hasLabel: false, thickness);
            lineRect.height = thickness;
            EditorGUI.DrawRect(lineRect, color);
        }
        
        protected void InsertLinkLabel(string _label, string _url)
        {
            if (EditorGUILayout.LinkButton(
                    _label,
                    GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(_url);
            }
        }
    }
}

// Created by dylan@hathora.dev

using System;
using Hathora.Scripts.Net.Common;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle
{
    public abstract class HathoraConfigUIBase
    {
        #region Vars
        /// <summary>The selected Config instance</summary>
        protected NetHathoraConfig Config { get; }
        protected SerializedObject SerializedConfig { get; }
        
        /// <summary>Do we have an Auth token?</summary>
        protected bool IsAuthed => 
            Config.HathoraCoreOpts.DevAuthOpts.HasAuthToken;
        
        protected GUIStyle CenterAlignLabelStyle { get; private set; }
        protected GUIStyle CenterAlignLargerTxtLabelNoWrapStyle { get; private set; }
        protected GUIStyle LeftAlignLabelStyle { get; private set; }
        protected GUIStyle CenterLinkLabelStyle { get; private set; }
        protected GUIStyle RightAlignLabelStyle { get; private set; }
        protected GUIStyle PreLinkLabelStyle { get; private set; }
        protected GUIStyle GeneralButtonStyle { get; private set; }
        protected GUIStyle BigButtonStyle { get; private set; }
        protected GUIStyle BtnsFoldoutStyle { get; private set; }

        public event Action RequestRepaint;
        #endregion // Vars

        
        #region Init
        protected HathoraConfigUIBase(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
        {
            Assert.That(_config, Is.Not.Null, "Config cannot be null");
            Assert.That(_serializedConfig, Is.Not.Null, "SerializedConfig cannot be null");
            
            this.Config = _config;
            this.SerializedConfig = _serializedConfig;
            
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
        /// Calling this will also unfocus any fields.
        /// </summary>
        protected void InvokeRequestRepaint()
        {
            RequestRepaint?.Invoke();
            unfocusFields();
        }

        /// <summary>
        /// Creates an invisible dummy ctrl - somewhat hacky.
        /// </summary>
        private void unfocusFields()
        {
            GUI.SetNextControlName("Dummy");
            GUI.TextField(new Rect(0, 0, 0, 0), "");
            GUI.FocusControl("Dummy");
        }
        
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
        
        protected static SerializedProperty FindNestedProperty(
            SerializedObject serializedObject, 
            params string[] propertyNames)
        {
            if (serializedObject == null || propertyNames == null || propertyNames.Length == 0)
            {
                Debug.LogError("SerializedObject or propertyNames is null or empty.");
                return null;
            }

            SerializedProperty currentProperty = serializedObject.FindProperty(propertyNames[0]);

            if (currentProperty == null)
            {
                Debug.LogError($"Could not find property '{propertyNames[0]}' in SerializedObject.");
                return null;
            }

            for (int i = 1; i < propertyNames.Length; i++)
            {
                if (currentProperty.isArray && i < propertyNames.Length - 1)
                {
                    int arrayIndex = int.Parse(propertyNames[i]);
                    currentProperty = currentProperty.GetArrayElementAtIndex(arrayIndex);
                    i++;
                }
                else
                {
                    currentProperty = currentProperty.FindPropertyRelative(propertyNames[i]);
                }

                if (currentProperty == null)
                {
                    Debug.LogError($"Could not find nested property '{propertyNames[i]}' in SerializedObject.");
                    return null;
                }
            }

            return currentProperty;
        }

    }
}

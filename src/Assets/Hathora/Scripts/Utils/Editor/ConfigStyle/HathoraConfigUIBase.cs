// Created by dylan@hathora.dev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        protected GUIStyle CenterAlignSmLabelStyle { get; private set; }
        protected GUIStyle CenterAlignLargerTxtLabelNoWrapStyle { get; private set; }
        protected GUIStyle LeftAlignLabelStyle { get; private set; }
        protected GUIStyle LeftAlignNoWrapLabelStyle { get; private set; }
        protected GUIStyle CenterLinkLabelStyle { get; private set; }
        protected GUIStyle RightAlignLabelStyle { get; private set; }
        protected GUIStyle PreLinkLabelStyle { get; private set; }
        protected GUIStyle GeneralButtonStyle { get; private set; }
        protected GUIStyle BigButtonStyle { get; private set; }
        protected GUIStyle BtnsFoldoutStyle { get; private set; }

        public event Action RequestRepaint;

        public enum GuiAlign
        {
            Stretched,
            SmallLeft,
            SmallRight,
        }
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
            this.LeftAlignNoWrapLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleLeft, _wordWrap:false);
            this.CenterAlignLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleCenter);
            this.RightAlignLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(TextAnchor.MiddleRight);
            this.CenterLinkLabelStyle ??= HathoraEditorUtils.GetRichLinkStyle(TextAnchor.MiddleCenter);
            this.PreLinkLabelStyle ??= HathoraEditorUtils.GetPreLinkLabelStyle();
            
            this.CenterAlignSmLabelStyle ??= HathoraEditorUtils.GetRichLabelStyle(
                TextAnchor.MiddleCenter,
                _fontSize: 9);
            
            this.CenterAlignLargerTxtLabelNoWrapStyle ??= HathoraEditorUtils.GetRichLabelStyle(
                TextAnchor.MiddleCenter,
                _wordWrap: false,
                _fontSize: 15);
        }
        #endregion // Init

        
        /// <summary>Are we logged in, already (is Config dev auth token set)?</summary>
        /// <returns></returns>
        protected bool CheckHasAuthToken() =>
            !string.IsNullOrEmpty(Config.HathoraCoreOpts.DevAuthOpts.DevAuthToken);
        
        protected bool CheckHasSelectedApp() => 
            !string.IsNullOrEmpty(Config.HathoraCoreOpts.AppId);

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
        
        protected void InsertHorizontalLine(float thickness, Color color, int _space = 0)
        {
            Rect lineRect = EditorGUILayout.GetControlRect(hasLabel: false, thickness);
            lineRect.height = thickness;
            EditorGUI.DrawRect(lineRect, color);
            
            if (_space > 0)
                EditorGUILayout.Space(_space);
        }
        
        /// <param name="_centerAlign">Wrap in a horizontal layout with flex space</param>
        protected void InsertLinkLabel(
            string _label, 
            string _url, 
            bool _centerAlign)
        {
            if (_centerAlign)
                StartCenterHorizAlign();
            
            if (EditorGUILayout.LinkButton(
                    _label,
                    GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(_url);
            }
            
            if (_centerAlign)
                EndCenterHorizAlign();
        }
        
        /// <summary>
        /// Handle the click event yourself
        /// </summary>
        /// <param name="_label"></param>
        /// <param name="_centerAlign">Wrap in a horizontal layout with flex space</param>
        /// <returns>clickedLabelLink</returns>
        protected bool InsertLinkLabelEvent(string _label, bool _centerAlign)
        {
            if (_centerAlign)
                StartCenterHorizAlign();

            // Use label rect to capture click events.
            Rect labelRect = GUILayoutUtility.GetRect(
                new GUIContent(_label), 
                CenterAlignLabelStyle);

            GUI.Label(labelRect, _label, CenterAlignLabelStyle);

            // Check if left mouse button is clicked within label rect
            bool clickedLabelLink = Event.current.type == EventType.MouseDown 
                && Event.current.button == 0 
                && labelRect.Contains(Event.current.mousePosition);

            if (_centerAlign)
                EndCenterHorizAlign();

            return clickedLabelLink;
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

        protected void InsertTooltipIcon(string _tooltipStr)
        {
            // Load and display the _tooltip icon
            Texture2D infoIcon = Resources.Load<Texture2D>("Icons/infoIcon");
            GUIContent iconContent = new(infoIcon, _tooltipStr);
            GUILayout.Label(iconContent, GUILayout.ExpandWidth(false));
        }

        /// <summary>
        /// Add _tooltip str to include a _tooltip icon + hover text
        /// </summary>
        /// <param name="_labelStr"></param>
        /// <param name="_tooltip"></param>
        /// <param name="_selectable">Want to select some text for copying?</param>
        protected void InsertLeftLabel(
            string _labelStr,
            string _tooltip = null,
            bool _selectable = false)
        {
            GUIContent labelContent = new(_labelStr);
            GUILayoutOption expandWidthOpt = GUILayout.ExpandWidth(false);
            
            if (_selectable)
            {
                EditorGUILayout.SelectableLabel(
                    _labelStr,
                    LeftAlignLabelStyle,
                    expandWidthOpt);
            }
            else
            {
                GUILayout.Label(
                    labelContent,
                    LeftAlignLabelStyle,
                    expandWidthOpt);
            }
            

            if (!string.IsNullOrEmpty(_tooltip))
                InsertTooltipIcon(_tooltip);
        }

        protected void InsertCenterLabel(string labelStr)
        {
            StartCenterHorizAlign();
            GUILayout.Label(labelStr, PreLinkLabelStyle);
            EndCenterHorizAlign();
        }

        /// <summary>
        /// Useful for smaller buttons you want centered for less emphasis.
        /// </summary>
        protected void StartCenterHorizAlign()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
        }

        protected void EndCenterHorizAlign()
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// </summary>
        /// <param name="_btnLabelStr"></param>
        /// <param name="_btnStyle"></param>
        /// <param name="_percentWidthOfScreen"></param>
        /// <returns>OnClick bool</returns>
        protected bool insertSmallCenteredBtn(
            string _btnLabelStr, 
            GUIStyle _btnStyle = null, 
            float _percentWidthOfScreen = 0.35f)
        {
            // This should be smaller than Login btn: Set to 35% of screen width
            GUILayoutOption regBtnWidth = GUILayout.Width(Screen.width * _percentWidthOfScreen);

            GUIStyle btnStyle = _btnStyle == null
                ? GeneralButtonStyle
                : _btnStyle;
            
            // USER INPUT >>
            bool clickedBtn = GUILayout.Button(_btnLabelStr, btnStyle, regBtnWidth);
            EditorGUILayout.Space(10f);

            return clickedBtn;
        }

        protected void insertLeftSelectableLabel(string _contentStr)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;

            Rect labelRect = GUILayoutUtility.GetRect(
                new GUIContent(_contentStr), 
                LeftAlignLabelStyle, 
                GUILayout.ExpandWidth(true), 
                GUILayout.Height(lineHeight));
            
            EditorGUI.SelectableLabel(labelRect, _contentStr, LeftAlignLabelStyle);
        }

        /// <returns>bool clicked</returns>
        protected bool insertLeftGeneralBtn(string _content) =>
            GUILayout.Button(_content, GeneralButtonStyle);
        
        /// <summary>
        /// {label} {tooltip} {input}
        /// </summary>
        /// <param name="_labelStr"></param>
        /// <param name="_tooltip"></param>
        /// <param name="_val"></param>
        /// <returns>inputStr</returns>
        protected string insertHorizLabeledTextField(
            string _labelStr,
            string _tooltip,
            string _val)
        {
            EditorGUILayout.BeginHorizontal();

            InsertLeftLabel(_labelStr, _tooltip);

            // USER INPUT >>
            string inputStr = GUILayout.TextField(
                _val, 
                GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();
            return inputStr;
        }
        
        /// <summary>
        /// Useful for Popup lists (dropdowns) for GUI selection.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        protected static string[] GetStrArrOfEnumMemberKeys<TEnum>() 
            where TEnum : Enum => Enum
                .GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => e.ToString())
                .ToArray();

        /// <summary>
        /// {label} {tooltip} {popupList}
        /// </summary>
        /// <param name="_labelStr"></param>
        /// <param name="_tooltip"></param>
        /// <param name="_displayOptsStrArr"></param>
        /// <param name="_selectedIndex"></param>
        /// <returns>Returns selected index</returns>
        protected int insertHorizLabeledPopupList(
            string _labelStr,
            string _tooltip,
            string[] _displayOptsStrArr,
            int _selectedIndex,
            GuiAlign _alignPopup = GuiAlign.Stretched)
        {
            EditorGUILayout.BeginHorizontal();

            InsertLeftLabel(_labelStr, _tooltip);
            
            if (_alignPopup == GuiAlign.SmallRight)
                GUILayout.FlexibleSpace();
            
            // USER INPUT >>
            int newSelectedIndex = EditorGUILayout.Popup(
                _selectedIndex, 
                _displayOptsStrArr,
                GUILayout.ExpandWidth(_alignPopup == GuiAlign.Stretched));
            
            if (_alignPopup == GuiAlign.SmallLeft)
                GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            return newSelectedIndex;
        }

        protected void SaveConfigChange(string _key, string _newVal)
        {
            Debug.Log($"[HathoraConfigUIBase] Set new Config vals for: `{_key}` to: `{_newVal}`");
            
            SerializedConfig.ApplyModifiedProperties();
            EditorUtility.SetDirty(Config); // Mark the object as dirty
            AssetDatabase.SaveAssets(); // Save changes to the ScriptableObject asset
        }
    }
}

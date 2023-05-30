// Created by dylan@hathora.dev

using Hathora.Scripts.Net.Common;
using UnityEditor;

namespace Hathora.Scripts.Utils.Editor.ConfigStyle.PostAuth
{
    public class HathoraConfigPostAuthBodyDeployAdvUI : HathoraConfigUIBase
    {
        #region Vars
        // Foldouts
        private bool isAdvancedDeployFoldout;
        #endregion // Vars


        #region Init
        public HathoraConfigPostAuthBodyDeployAdvUI(
            NetHathoraConfig _config, 
            SerializedObject _serializedConfig)
            : base(_config, _serializedConfig)
        {
            if (!HathoraConfigUI.ENABLE_BODY_STYLE)
                return;
        }
        #endregion // Init
        
        
        #region UI Draw
        public void Draw()
        {
            if (!IsAuthed)
                return; // You should be calling HathoraConfigPreAuthBodyUI.Draw()

            insertAdvancedDeployFoldout();
            InsertSpace2x();
        }
       
        private void insertAdvancedDeployFoldout()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            isAdvancedDeployFoldout = EditorGUILayout.Foldout(
                isAdvancedDeployFoldout, 
                "Advanced");

            EditorGUI.indentLevel--;
            if (!isAdvancedDeployFoldout)
            {
                EditorGUILayout.EndVertical(); // End of foldout box skin
                return;
            }
            
            InsertSpace2x();
            
            insertAdvancedDeployFoldoutComponents();
            
            EditorGUILayout.EndVertical(); // End of foldout box skin
        }

        private void insertAdvancedDeployFoldoutComponents()
        {
            // TODO
        }
        #endregion // UI Draw

        
        #region Event Logic
        // TODO
        #endregion // Event Logic
    }
}

#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DamageNumbersPro.Internal
{
    [CustomEditor(typeof(DNPPreset))]
    public class DNPPresetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Prepare:
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.richText = true;


            //Copying:
            EditorGUILayout.Space(4);
            DamageNumber dn = (DamageNumber) EditorGUILayout.ObjectField(null, typeof(DamageNumber), true,GUILayout.Height(80));
            GUIStyle dropStyle = new GUIStyle(GUI.skin.box);
            dropStyle.alignment = TextAnchor.MiddleCenter;
            Rect lastRect = GUILayoutUtility.GetLastRect();
            GUI.Box(lastRect, "Drop damage number here.", dropStyle);
            if(dn != null)
            {
                DNPPreset preset = (DNPPreset)target;
                Undo.RegisterCompleteObjectUndo(preset, "Copied damage number.");
                preset.Get(dn);

                serializedObject.ApplyModifiedProperties();
            }


            //Get First Property:
            SerializedProperty currentProperty = serializedObject.FindProperty("changeFontAsset");

            //Display Properties:
            EditorGUILayout.BeginVertical();
            bool visible = true;
            do
            {
                bool isNewCategory = currentProperty.name.StartsWith("change") || currentProperty.name == "hideVerticalTexts";
                if (isNewCategory)
                {
                    visible = true;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("Helpbox");
                }

                if(visible)
                {
                    if(isNewCategory)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("<size=14><b>" + currentProperty.displayName + "</b></size>", labelStyle);
                        EditorGUILayout.PropertyField(currentProperty, GUIContent.none, true);
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(currentProperty, true);
                    }
                }

                if (isNewCategory)
                {
                    visible = currentProperty.boolValue;

                    if(visible && currentProperty.name.StartsWith("change"))
                    {
                        DNPEditorInternal.Lines();
                    }
                }

            } while (currentProperty.NextVisible(false));

            EditorGUILayout.EndVertical();

            //Save Changes:
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
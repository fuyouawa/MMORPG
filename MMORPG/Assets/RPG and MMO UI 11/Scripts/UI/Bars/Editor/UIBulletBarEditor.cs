using UnityEngine;
using UnityEditor;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
    [CustomEditor(typeof(UIBulletBar), true)]
    public class UIBulletBarEditor : Editor
    {
        private SerializedProperty m_BarType;
        private SerializedProperty m_FixedSize;
        private SerializedProperty m_BulletSize;
        private SerializedProperty m_BulletSprite;
        private SerializedProperty m_BulletSpriteColor;
        private SerializedProperty m_SpriteRotation;
        private SerializedProperty m_BulletSpriteActive;
        private SerializedProperty m_BulletSpriteActiveColor;
        private SerializedProperty m_ActivePosition;
        private SerializedProperty m_AngleMin;
        private SerializedProperty m_AngleMax;
        private SerializedProperty m_BulletCount;
        private SerializedProperty m_Distance;
        private SerializedProperty m_FillAmount;
        private SerializedProperty m_InvertFill;

        protected virtual void OnEnable()
        {
            this.m_BarType = base.serializedObject.FindProperty("m_BarType");
            this.m_FixedSize = base.serializedObject.FindProperty("m_FixedSize");
            this.m_BulletSize = base.serializedObject.FindProperty("m_BulletSize");
            this.m_BulletSprite = base.serializedObject.FindProperty("m_BulletSprite");
            this.m_BulletSpriteColor = base.serializedObject.FindProperty("m_BulletSpriteColor");
            this.m_SpriteRotation = base.serializedObject.FindProperty("m_SpriteRotation");
            this.m_BulletSpriteActive = base.serializedObject.FindProperty("m_BulletSpriteActive");
            this.m_BulletSpriteActiveColor = base.serializedObject.FindProperty("m_BulletSpriteActiveColor");
            this.m_ActivePosition = base.serializedObject.FindProperty("m_ActivePosition");
            this.m_AngleMin = base.serializedObject.FindProperty("m_AngleMin");
            this.m_AngleMax = base.serializedObject.FindProperty("m_AngleMax");
            this.m_BulletCount = base.serializedObject.FindProperty("m_BulletCount");
            this.m_Distance = base.serializedObject.FindProperty("m_Distance");
            this.m_FillAmount = base.serializedObject.FindProperty("m_FillAmount");
            this.m_InvertFill = base.serializedObject.FindProperty("m_InvertFill");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
            EditorGUILayout.PropertyField(this.m_BarType, new GUIContent("Type"));
            EditorGUILayout.PropertyField(this.m_BulletCount, new GUIContent("Bullet Count"));
            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Bullet Sprites", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
            EditorGUILayout.PropertyField(this.m_BulletSprite, new GUIContent("Background Sprite"));
            EditorGUILayout.PropertyField(this.m_BulletSpriteColor, new GUIContent("Background Color"));
            EditorGUILayout.PropertyField(this.m_BulletSpriteActive, new GUIContent("Fill Sprite"));
            EditorGUILayout.PropertyField(this.m_BulletSpriteActiveColor, new GUIContent("Fill Color"));
            EditorGUILayout.PropertyField(this.m_FixedSize, new GUIContent("Fixed Size"));

            if (this.m_FixedSize.boolValue)
            {
                EditorGUILayout.PropertyField(this.m_BulletSize, new GUIContent("Size"));
            }

            EditorGUILayout.PropertyField(this.m_SpriteRotation, new GUIContent("Rotation"));
            EditorGUILayout.PropertyField(this.m_ActivePosition, new GUIContent("Fill Offset"));
            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);

            EditorGUILayout.Space();

            UIBulletBar.BarType barType = (UIBulletBar.BarType)this.m_BarType.enumValueIndex;

            if (barType == UIBulletBar.BarType.Radial)
            {
                EditorGUILayout.LabelField("Radial Bar Properties", EditorStyles.boldLabel);
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                EditorGUILayout.PropertyField(this.m_AngleMin, new GUIContent("Min Angle"));
                EditorGUILayout.PropertyField(this.m_AngleMax, new GUIContent("Max Angle"));
                EditorGUILayout.PropertyField(this.m_Distance, new GUIContent("Radius"));
                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);

                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Fill Properties", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
            EditorGUILayout.PropertyField(this.m_FillAmount, new GUIContent("Fill Amount"));
            EditorGUILayout.PropertyField(this.m_InvertFill, new GUIContent("Invert Fill"));
            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);

            EditorGUILayout.Space();
            base.serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Generate Bullets"))
            {
                foreach (Object target in this.targets)
                    (target as UIBulletBar).ConstructBullets();
            }

            EditorGUILayout.Space();
        }
    }
}

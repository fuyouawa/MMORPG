using UnityEditor;
using UnityEngine;

public static class EditorGUIHelper
{
    public class ListGroup
    {
        public static readonly float DefaultLineHeight = EditorGUIUtility.singleLineHeight + 0.5f;

        public int LineCount { get; private set; }
        public float PrevLineHeight { get; private set; }
        public float Height { get; private set; }

        public void NextLines(int count)
        {
            for (int i = 0; i < count; i++)
            {
                NextLine();
            }
        }

        public void NextLine()
        {
            LineCount++;
            PrevLineHeight = DefaultLineHeight;
            Height += PrevLineHeight;
        }

        public void NextLine(SerializedProperty property)
        {
            LineCount++;
            PrevLineHeight = EditorGUI.GetPropertyHeight(property);
            Height += PrevLineHeight;
        }

        public void NextLine(float height)
        {
            LineCount++;
            PrevLineHeight = height;
            Height += PrevLineHeight;
        }

        public void NextLine(ref Rect position)
        {
            NextLine();
            position.y += PrevLineHeight;
        }

        public void NextLine(SerializedProperty property, ref Rect position)
        {
            NextLine(property);
            position.y += PrevLineHeight;
        }
    }
}

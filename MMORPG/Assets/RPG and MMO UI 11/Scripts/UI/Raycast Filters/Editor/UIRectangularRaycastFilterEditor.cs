using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UIRectangularRaycastFilter))]
	public class UIRectangularRaycastFilterEditor : Editor {
		
		public const string PREFS_KEY = "UIRectRaycastFilter_DG";
		private bool m_DisplayGeometry = true;
		
		protected void OnEnable()
		{
			this.m_DisplayGeometry = EditorPrefs.GetBool(PREFS_KEY, true);
		}
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            this.m_DisplayGeometry = EditorGUILayout.Toggle("Display Geometry", this.m_DisplayGeometry);
			
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool(PREFS_KEY, this.m_DisplayGeometry);
                SceneView.RepaintAll();
            }
		}
		
		public Vector3[] scaledWorldCorners
		{
			get
			{
				UIRectangularRaycastFilter filter = this.target as UIRectangularRaycastFilter;
				Rect scaledRect = filter.scaledRect;
                
				RectTransform rt = (RectTransform)filter.transform;
				Vector3[] corners = new Vector3[4];

                 corners[0] = new Vector3(rt.rect.x + scaledRect.x, rt.rect.height + rt.rect.y + scaledRect.y, 0f);
                 corners[1] = new Vector3(rt.rect.x + scaledRect.x, rt.rect.height + rt.rect.y + scaledRect.y + scaledRect.height, 0f);
                 corners[2] = new Vector3(rt.rect.x + scaledRect.x + scaledRect.width, rt.rect.height + rt.rect.y + scaledRect.y + scaledRect.height, 0f);
                 corners[3] = new Vector3(rt.rect.x + scaledRect.x + scaledRect.width, rt.rect.height + rt.rect.y + scaledRect.y, 0f);
                 
                for (int i = 0; i < 4; i++)
                {
                    corners[i] += new Vector3(rt.rect.width * rt.pivot.x, 0f, 0f);
                    corners[i] += new Vector3(0f, (rt.rect.height * (1f - rt.pivot.y)) * -1f, 0f);
                    corners[i] = rt.TransformPoint(corners[i]);
                }

                return corners;
			}
		}
		
		protected void OnSceneGUI()
		{
			if (!this.m_DisplayGeometry)
				return;
			
			Vector3[] worldCorners = this.scaledWorldCorners;
			
			Handles.color = Color.green;
			Handles.DrawLine(worldCorners[0], worldCorners[1]); // Left line
			Handles.DrawLine(worldCorners[1], worldCorners[2]); // Top line
			Handles.DrawLine(worldCorners[2], worldCorners[3]); // Right line
			Handles.DrawLine(worldCorners[3], worldCorners[0]); // Bottom line
		}
	}
}

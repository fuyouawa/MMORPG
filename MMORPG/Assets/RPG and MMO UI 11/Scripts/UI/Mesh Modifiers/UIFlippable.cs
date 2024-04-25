using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DuloGames.UI
{
	[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Graphic)), DisallowMultipleComponent, AddComponentMenu("UI/Flippable", 8)]
#if UNITY_5_2 || UNITY_5_3_OR_NEWER
    public class UIFlippable : MonoBehaviour, IMeshModifier {
#else
    public class UIFlippable : MonoBehaviour, IVertexModifier {
#endif
        [SerializeField] private bool m_Horizontal = false;
		[SerializeField] private bool m_Veritical = false;
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UnityEngine.UI.UIFlippable"/> should be flipped horizontally.
		/// </summary>
		/// <value><c>true</c> if horizontal; otherwise, <c>false</c>.</value>
		public bool horizontal
		{
			get { return this.m_Horizontal; }
			set { this.m_Horizontal = value; this.GetComponent<Graphic>().SetVerticesDirty(); }
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UnityEngine.UI.UIFlippable"/> should be flipped vertically.
		/// </summary>
		/// <value><c>true</c> if vertical; otherwise, <c>false</c>.</value>
		public bool vertical
		{
			get { return this.m_Veritical; }
			set { this.m_Veritical = value; this.GetComponent<Graphic>().SetVerticesDirty(); }
		}
		
#if UNITY_EDITOR
		protected void OnValidate()
		{
			this.GetComponent<Graphic>().SetVerticesDirty();
		}
#endif

#if UNITY_5_2 || UNITY_5_3_OR_NEWER
        public void ModifyMesh(VertexHelper vertexHelper)
		{
			if (!this.enabled)
				return;
			
			List<UIVertex> list = new List<UIVertex>();
			vertexHelper.GetUIVertexStream(list);
			
			ModifyVertices(list);  // calls the old ModifyVertices which was used on pre 5.2
			
			vertexHelper.Clear();
			vertexHelper.AddUIVertexTriangleStream(list);
		}

        public void ModifyMesh(Mesh mesh)
        {
            if (!this.enabled)
                return;

            List<UIVertex> list = new List<UIVertex>();
            using (VertexHelper vertexHelper = new VertexHelper(mesh))
            {
                vertexHelper.GetUIVertexStream(list);
            }

            ModifyVertices(list);  // calls the old ModifyVertices which was used on pre 5.2

            using (VertexHelper vertexHelper2 = new VertexHelper())
            {
                vertexHelper2.AddUIVertexTriangleStream(list);
                vertexHelper2.FillMesh(mesh);
            }
        }
#endif

        public void ModifyVertices(List<UIVertex> verts)
		{
            if (!this.enabled)
                return;

            RectTransform rt = this.transform as RectTransform;
			
			for (int i = 0; i < verts.Count; ++i)
			{
				UIVertex v = verts[i];
				
				// Modify positions
				v.position = new Vector3(
					(this.m_Horizontal ? (v.position.x + (rt.rect.center.x - v.position.x) * 2) : v.position.x),
					(this.m_Veritical ?  (v.position.y + (rt.rect.center.y - v.position.y) * 2) : v.position.y),
					v.position.z
				);
				
				// Apply
				verts[i] = v;
			}
		}
	}
}

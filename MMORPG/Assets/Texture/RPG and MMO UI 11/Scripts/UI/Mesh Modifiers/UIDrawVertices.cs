using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    [ExecuteInEditMode]
    public class UIDrawVertices : MonoBehaviour, IMeshModifier
    {
        [SerializeField] private Color color = Color.green;
        private Mesh mesh;

#if UNITY_EDITOR
        protected void OnValidate()
        {
            this.GetComponent<Graphic>().SetVerticesDirty();
        }
#endif

        public void ModifyMesh(VertexHelper vertexHelper)
        {
            if (this.mesh == null) this.mesh = new Mesh();

            vertexHelper.FillMesh(this.mesh);
        }

        public void ModifyMesh(Mesh mesh) { }

        public void OnDrawGizmos()
        {
            if (this.mesh == null) return;

            Gizmos.color = this.color;
            Gizmos.DrawWireMesh(this.mesh, this.transform.position);
        }
    }
}

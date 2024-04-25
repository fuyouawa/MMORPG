using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuloGames.UI
{
    [ExecuteInEditMode, AddComponentMenu("UI/Color Scheme Element", 48)]
    public class ColorSchemeElement : MonoBehaviour, IColorSchemeElement
    {
        [SerializeField] private ColorSchemeShade m_Shade = ColorSchemeShade.Primary;

        public ColorSchemeShade shade
        {
            get { return this.m_Shade; }
            set { this.m_Shade = value; }
        }
        
        protected void Awake()
        {
            // Apply the actie color scheme to this element
            if (ColorSchemeManager.Instance != null && ColorSchemeManager.Instance.activeColorScheme != null)
                ColorSchemeManager.Instance.activeColorScheme.ApplyToElement(this);
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            // Apply the actie color scheme to this element
            if (ColorSchemeManager.Instance != null && ColorSchemeManager.Instance.activeColorScheme != null)
                ColorSchemeManager.Instance.activeColorScheme.ApplyToElement(this);
        }
#endif

        public void Apply(Color newColor)
        {
            // Get the a graphic component
            Graphic graphic = this.gameObject.GetComponent<Graphic>();

            if (graphic == null)
                return;
            
            // Keep the graphic alpha
            graphic.color = new Color(newColor.r, newColor.g, newColor.b, graphic.color.a);

#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorUtility.SetDirty(graphic);
#endif
        }
    }
}

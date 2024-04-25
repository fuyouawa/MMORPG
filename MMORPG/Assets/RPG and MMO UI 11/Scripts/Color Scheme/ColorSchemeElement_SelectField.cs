using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuloGames.UI
{
    [ExecuteInEditMode, AddComponentMenu("UI/Color Scheme Element - Select Field", 48)]
    public class ColorSchemeElement_SelectField : MonoBehaviour, IColorSchemeElement
    {
        public enum ElementType
        {
            List,
            Separator
        }

        [SerializeField] private ElementType m_ElementType = ElementType.List;
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
            // Get the image component
            UISelectField select = this.gameObject.GetComponent<UISelectField>();

            if (select == null)
                return;

            switch (this.m_ElementType)
            {
                case ElementType.List:
                    select.listBackgroundColor = newColor;
                    break;
                case ElementType.Separator:
                    select.listSeparatorColor = newColor;
                    break;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorUtility.SetDirty(select);
#endif
        }
    }
}

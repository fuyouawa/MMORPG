using UnityEngine;

namespace DuloGames.UI
{
    public class Test_SelectAddOption : MonoBehaviour {

        #pragma warning disable 0649
        [SerializeField] private UISelectField m_SelectField;
        [SerializeField] private string m_Text;
        #pragma warning restore 0649

        [ContextMenu("Add Option")]
        public void AddOption()
        {
            if (this.m_SelectField != null)
            {
                this.m_SelectField.AddOption(this.m_Text);
            }
        }
    }
}

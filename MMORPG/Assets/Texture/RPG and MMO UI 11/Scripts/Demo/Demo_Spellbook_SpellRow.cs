using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    public class Demo_Spellbook_SpellRow : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private UISpellSlot m_Slot;
        [SerializeField] private Text m_NameText;
        [SerializeField] private Text m_RankText;
        [SerializeField] private Text m_DescriptionText;
        [SerializeField] private bool m_IsDemo = false;
        #pragma warning restore 0649

        void Start()
        {
            if (UISpellDatabase.Instance == null || !this.m_IsDemo)
                return;
            
            UISpellInfo[] spells = UISpellDatabase.Instance.spells;
            UISpellInfo spell = spells[Random.Range(0, spells.Length)];

            if (this.m_Slot != null) this.m_Slot.Assign(spell);
            if (this.m_NameText != null) this.m_NameText.text = spell.Name;
            if (this.m_RankText != null) this.m_RankText.text = Random.Range(1, 6).ToString();
            if (this.m_DescriptionText != null) this.m_DescriptionText.text = spell.Description;
        }
    }
}

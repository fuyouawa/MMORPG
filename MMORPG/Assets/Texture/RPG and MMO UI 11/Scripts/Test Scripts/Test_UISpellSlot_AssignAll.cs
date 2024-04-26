using UnityEngine;

namespace DuloGames.UI
{
    public class Test_UISpellSlot_AssignAll : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Transform m_Container;
        #pragma warning restore 0649

        void Start()
        {
            if (this.m_Container == null || UISpellDatabase.Instance == null)
            {
                this.Destruct();
                return;
            }

            UISpellSlot[] slots = this.m_Container.gameObject.GetComponentsInChildren<UISpellSlot>();
            UISpellInfo[] spells = UISpellDatabase.Instance.spells;

            if (slots.Length > 0 && spells.Length > 0)
            {
                foreach (UISpellSlot slot in slots)
                    slot.Assign(spells[Random.Range(0, spells.Length)]);
            }

            this.Destruct();
        }

        private void Destruct()
        {
            DestroyImmediate(this);
        }
    }
}

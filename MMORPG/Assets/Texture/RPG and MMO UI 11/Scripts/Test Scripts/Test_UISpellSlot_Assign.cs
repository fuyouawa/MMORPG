using UnityEngine;

namespace DuloGames.UI
{
    public class Test_UISpellSlot_Assign : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private UISpellSlot slot;
        [SerializeField] private int assignSpell;
        #pragma warning restore 0649

        void Awake()
        {
            if (this.slot == null)
                this.slot = this.GetComponent<UISpellSlot>();
        }

        void Start()
        {
            if (this.slot == null || UISpellDatabase.Instance == null)
            {
                this.Destruct();
                return;
            }

            this.slot.Assign(UISpellDatabase.Instance.GetByID(this.assignSpell));
            this.Destruct();
        }

        private void Destruct()
        {
            DestroyImmediate(this);
        }
    }
}

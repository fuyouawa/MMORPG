using UnityEngine;

namespace DuloGames.UI
{
    public class Test_UIEquipSlot_Assign : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private UIEquipSlot slot;
        [SerializeField] private int assignItem;
        #pragma warning restore 0649

        void Awake()
        {
            if (this.slot == null)
                this.slot = this.GetComponent<UIEquipSlot>();
        }

        void Start()
        {
            if (this.slot == null || UIItemDatabase.Instance == null)
            {
                this.Destruct();
                return;
            }

            this.slot.Assign(UIItemDatabase.Instance.GetByID(this.assignItem));
            this.Destruct();
        }

        private void Destruct()
        {
            DestroyImmediate(this);
        }
    }
}

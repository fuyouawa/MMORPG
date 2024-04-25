using UnityEngine;

namespace DuloGames.UI
{
    public class Test_UIItemSlot_AssignAll : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Transform m_Container;
        #pragma warning restore 0649

        void Start()
        {
            if (this.m_Container == null || UIItemDatabase.Instance == null)
            {
                this.Destruct();
                return;
            }

            UIItemSlot[] slots = this.m_Container.gameObject.GetComponentsInChildren<UIItemSlot>();
            UIItemInfo[] items = UIItemDatabase.Instance.items;

            if (slots.Length > 0 && items.Length > 0)
            {
                foreach (UIItemSlot slot in slots)
                    slot.Assign(items[Random.Range(0, items.Length)]);
            }

            this.Destruct();
        }

        private void Destruct()
        {
            DestroyImmediate(this);
        }
    }
}

using UnityEngine;

namespace DuloGames.UI
{
    public class Test_UISlotBase_Assign : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private UISlotBase slot;
        [SerializeField] private Texture texture;
        [SerializeField] private Sprite sprite;
        #pragma warning restore 0649

        void Start()
        {
            if (this.slot != null)
            {
                if (this.texture != null)
                {
                    this.slot.Assign(this.texture);
                }
                else if (this.sprite != null)
                {
                    this.slot.Assign(this.sprite);
                }
            }
        }
    }
}

using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UIKnapsackSlot : UISlotBase
    {
        public Image Image;

        protected override void Awake()
        {
            base.Awake();
            //Image = transform.Find("Icon").GetComponent<Image>();
        }
       
    }
}

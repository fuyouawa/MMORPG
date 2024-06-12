using MMORPG.Global;
using MMORPG.Tool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UISkillSlot : UISlotBase
    {
        public Image ImageIcon;
        public TextMeshProUGUI TextHotkey;

        public SkillDefine Define { get; private set; }

        public override void Setup(UIInventoryBase inventory, int slotId)
        {
            base.Setup(inventory, slotId);
            TextHotkey.SetText((slotId + 1).ToString());
        }

        public void Assign(SkillDefine define)
        {
            ImageIcon.enabled = true;
            ImageIcon.sprite = Resources.Load<Texture2D>($"{Config.SkillIconPath}/{define.Icon}").ToSprite();
            Define = define;
        }
    }
}

using System.Linq;
using MMORPG.Game;
using MMORPG.System;
using MMORPG.UI;
using QFramework;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.UI
{
    public class UISkillPanel : UIInventoryBase, IController
    {
        public RectTransform SlotsGroup;

        public override RectTransform GroupSlots => SlotsGroup;
        protected override string SlotAssetName => "UISkillSlot";

        private void Start()
        {
            SetSlotCount(5);

            var slots = GroupSlots.GetComponentsInChildren<UISkillSlot>();
            var playerManager = this.GetSystem<IPlayerManagerSystem>();
            var skillManager = playerManager.MineEntity.SkillManager;

            int i = 0;
            foreach (var skill in skillManager.Skills.Where(x => x.Mode == SkillModes.Skill))
            {
                slots[i].Assign(skill.Define);
                i++;
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}

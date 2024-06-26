using System.Linq;
using MMORPG.Event;
using MMORPG.Game;
using MMORPG.System;
using QFramework;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.UI
{
    public class UISkillPanel : UIInventoryBase, IController
    {
        public RectTransform SlotsGroup;

        public CharacterSkillManager SkillManager { get; private set; }

        public bool HasSkillRequestingSpell { get; set; }

        public override RectTransform GroupSlots => SlotsGroup;
        protected override string SlotAssetName => "UISkillSlot";

        private void Awake()
        {
            SetSlotCount(5);
        }

        private async void Start()
        {
            var slots = GroupSlots.GetComponentsInChildren<UISkillSlot>();

            var mine = await this.GetSystem<IPlayerManagerSystem>().GetMineEntityTask();

            SkillManager = mine.GetComponent<ActorController>().SkillManager;

            int i = 0;
            foreach (var skill in SkillManager.Skills.Where(x => x.Mode == SkillModes.Skill))
            {
                slots[i].Assign(skill);
                i++;
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}

using System.Linq;
using MMORPG.Event;
using MMORPG.Game;
using MMORPG.System;
using QFramework;
using UnityEngine;

namespace MMORPG.UI
{
    public class UISkillPanel : UIInventoryBase, IController
    {
        public RectTransform SlotsGroup;

        public GameInputControls InputControls { get; private set; }

        public CharacterSkillManager SkillManager { get; private set; }

        public bool HasSkillRequestingSpell { get; set; }

        public override RectTransform GroupSlots => SlotsGroup;
        protected override string SlotAssetName => "UIPrefab/UIGameHub/UISkillSlot";

        private void Awake()
        {
            SetSlotCount(5);
            InputControls = new();
        }

        private void OnEnable()
        {
            InputControls.Enable();
        }

        private void OnDisable()
        {
            InputControls.Disable();
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

            InputControls.Player.Skill1.started += context => slots[0].Spell();
            InputControls.Player.Skill2.started += context => slots[1].Spell();
            InputControls.Player.Skill3.started += context => slots[2].Spell();
            InputControls.Player.Skill4.started += context => slots[3].Spell();
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}

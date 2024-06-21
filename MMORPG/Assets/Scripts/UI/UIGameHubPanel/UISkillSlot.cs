using MMORPG.Common.Proto.Fight;
using MMORPG.Game;
using MMORPG.Global;
using MMORPG.System;
using MMORPG.Tool;
using QFramework;
using Serilog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UISkillSlot : UISlotBase, IController
    {
        public Image ImageIcon;
        public TextMeshProUGUI TextHotkey;
        public Image ImageCdOverlay;
        public TextMeshProUGUI TextCd;

        public Skill Skill { get; private set; }

        private INetworkSystem _network;

        public UISkillPanel SkillPanel => Inventory as UISkillPanel;
        

        private void Awake()
        {
            _network = this.GetSystem<INetworkSystem>();
        }

        private void Update()
        {
            if (Skill != null)
            {
                if (Skill.CurrentState == Skill.States.Cooling)
                {
                    var radio = Skill.RemainCd / Skill.Define.Cd;
                    ImageCdOverlay.fillAmount = radio;
                    TextCd.SetText($"{Skill.RemainCd:0.00}s");
                }
            }
        }

        public override void Setup(UIInventoryBase inventory, int slotId)
        {
            base.Setup(inventory, slotId);
            TextHotkey.SetText((slotId + 1).ToString());
        }

        public void Assign(Skill skill)
        {
            ImageIcon.enabled = true;
            ImageIcon.sprite = Resources.Load<Texture2D>($"{Config.SkillIconPath}/{skill.Define.Icon}").ToSprite();
            Skill = skill;

            Skill.OnStateChanged += () =>
            {
                var isCooling = Skill.CurrentState == Skill.States.Cooling;
                ImageCdOverlay.enabled = isCooling;
                TextCd.gameObject.SetActive(isCooling);
            };
        }

        public void TriggerSpell()
        {
            Spell();
        }

        public async void Spell()
        {
            if (SkillPanel.HasSkillRequestingSpell) return;
            if (Skill.CurrentState != Skill.States.Idle) return;

            var skillManager = Skill.SkillManager;

            if (skillManager.CurrentSpellingSkill != null)
                return;

            SkillPanel.HasSkillRequestingSpell = true;
            _network.SendToServer(new SpellRequest()
            {
                Info = new()
                {
                    SkillId = Skill.Define.ID,
                    CasterId = skillManager.Character.Entity.EntityId
                }
            });

            var response = await _network.ReceiveAsync<SpellFailResponse>();

            if (response.Reason == CastResult.Success)
            {
                Skill.Use(new CastTargetEntity(skillManager.Character.Entity));
            }
            else
            {
                Log.Error($"技能释放请求失败! 原因:{response.Reason}");
            }
            SkillPanel.HasSkillRequestingSpell = false;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}

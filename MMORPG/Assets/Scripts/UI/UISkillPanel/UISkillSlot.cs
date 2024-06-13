using Common.Proto.Fight;
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

        public SkillDefine Define { get; private set; }

        private INetworkSystem _network;
        private bool _requestingSpell;

        private void Awake()
        {
            _network = this.GetSystem<INetworkSystem>();
        }

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

        public void TriggerSpell()
        {
            Spell();
        }

        public async void Spell()
        {
            if (_requestingSpell) return;

            var skillManager = ((UISkillPanel)Inventory).SkillManager;

            _requestingSpell = true;
            _network.SendToServer(new SpellRequest()
            {
                Info = new()
                {
                    SkillId = Define.ID,
                    CasterId = skillManager.Character.Entity.EntityId
                }
            });

            var response = await _network.ReceiveAsync<SpellFailResponse>();

            if (response.Reason == CastResult.Success)
            {
                skillManager.GetSkill(Define.ID).Use(new CastTargetEntity(skillManager.Character.Entity));
            }
            else
            {
                Log.Error($"技能释放请求失败! 原因:{response.Reason}");
            }
            _requestingSpell = false;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}

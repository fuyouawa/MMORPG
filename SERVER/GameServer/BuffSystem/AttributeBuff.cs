using GameServer.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GameServer.BuffSystem
{
    public class AttributeBuff : Buff
    {
        private JArray _attributeModifier;
        public AttributeBuff(int buffId, BuffManager buffManager, Actor? caster, float duration, string attributeModifier) 
            : base(buffId, buffManager, caster, duration)
        {
            _attributeModifier = JArray.Parse(attributeModifier);
        }

        public override void Start()
        {
            base.Start();
            Modify("Start");
        }

        public override void Exit()
        {
            base.Exit();
            Modify("Exit");
        }

        private void Modify(string stage)
        {
            foreach (JObject obj in _attributeModifier)
            {
                if (obj[stage] == null) return;
                JObject stageValue = obj[stage] as JObject;
                if (stageValue == null) return;

                foreach (var property in stageValue.Properties())
                {
                    string name = property.Name;
                    string? content = property.Value<string>();
                    if (content == null) continue;
                    if (name == "Hp")
                    {
                        BuffManager.OwnerActor.ChangeHP(int.Parse(content));
                    }
                    else if (name == "Mp")
                    {
                        BuffManager.OwnerActor.ChangeMp(int.Parse(content));
                    }
                }
            }
            
        }
    }
}

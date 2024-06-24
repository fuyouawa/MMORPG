using GameServer.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            Change("Start");
        }

        public override void Exit()
        {
            base.Exit();
            Change("Exit");
        }

        private void Change(string stage)
        {
            foreach (JObject obj in _attributeModifier)
            {
                if (obj["name"] == null) return;
                if (obj[stage] == null) return;
                string name = obj["name"].ToString();
                string content = obj[stage].ToString();
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

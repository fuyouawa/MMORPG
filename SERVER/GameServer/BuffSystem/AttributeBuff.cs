using GameServer.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Xml.Linq;
using GameServer.PlayerSystem;

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
                    string? content = property.Value.Value<string>();
                    if (content == null || !content.Any()) continue;

                    int value = 0;
                    if (content[content.Length - 1] == '%')
                    {
                        PropertyInfo propertyInfo = typeof(Actor).GetProperty(name);
                        if (propertyInfo == null) continue;

                        float percentage = int.Parse(content.Substring(0, content.Length - 1));

                        // 获取属性值
                        var tmp = propertyInfo.GetValue(BuffManager.OwnerActor);
                        if (tmp == null || !(tmp is int)) continue;
                        int propertyValue = (int)tmp;
                        // 计算并返回结果
                        value = (int)(propertyValue * percentage / 100);
                    }
                    else
                    {
                        value = int.Parse(content);
                    }

                    switch (name)
                    {
                        case "Hp":
                            BuffManager.OwnerActor.ChangeHP(value);
                            break;
                        case "Mp":
                            BuffManager.OwnerActor.ChangeMp(value);
                            break;
                        case "Speed":
                            break;
                        case "Exp":
                            if (BuffManager.OwnerActor is Player)
                            {
                                ((Player)BuffManager.OwnerActor).ChangeExp(value);
                            }
                            break;
                    }

                }
            }
            
        }
    }
}

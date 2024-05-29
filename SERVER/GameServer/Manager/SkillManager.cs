using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Fight;
using GameServer.Model;

namespace GameServer.Manager
{
    public class SkillManager
    {
        private Actor _actor;
        public List<Skill> SkillList = new();

        public SkillManager(Actor actor)
        {
            _actor = actor;

            // 应该从数据库中读取，当前未设计，直接加载所有技能
            var list = DataManager.Instance.SkillDict.Values
                .Where(s => s.UnitID == _actor.UnitId)
                .ToList();
            foreach (var define in list)
            {
                var skill = new Skill(_actor, define);
                SkillList.Add(skill);
            }
        }
        
        public void Update()
        {
            foreach (var skill in SkillList)
            {
                skill.Update();
            }
        }
    }
}

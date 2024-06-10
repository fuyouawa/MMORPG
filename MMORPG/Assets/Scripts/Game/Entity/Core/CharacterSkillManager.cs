using System.Collections.Generic;
using System.Linq;
using QFramework;

namespace MMORPG.Game
{
    public class CharacterSkillManager: IController
    {
        public EntityView Entity;

        private List<Skill> _skills = new();

        public CharacterSkillManager(EntityView entity)
        {
            Entity = entity;
            Initialize();
        }

        public void Initialize()
        {
            var data = this.GetSystem<IDataManagerSystem>();
            _skills = data.GetUnitSkillsDefine(Entity.UnitId)
                .Select(x => new Skill(this, x))
                .ToList();
        }

        public void Update()
        {
            foreach (var skill in _skills)
            {
                skill.Update();
            }
        }

        public Skill GetSkill(int skillId)
        {
            return _skills.FirstOrDefault(x => x.Define.ID == skillId);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}

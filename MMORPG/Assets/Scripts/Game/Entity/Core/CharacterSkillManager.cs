using System.Collections.Generic;
using System.Linq;
using QFramework;

namespace MMORPG.Game
{
    public class CharacterSkillManager: IController
    {
        public CharacterController Character;

        private List<Skill> _skills = new();

        public CharacterSkillManager(CharacterController character)
        {
            Character = character;
            Initialize();
        }

        public void Initialize()
        {
            var data = this.GetSystem<IDataManagerSystem>();
            _skills = data.GetUnitSkillsDefine(Character.Entity.UnitId)
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

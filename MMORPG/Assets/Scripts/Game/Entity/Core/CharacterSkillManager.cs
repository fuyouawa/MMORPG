using System.Collections.Generic;
using System.Linq;
using QFramework;

namespace MMORPG.Game
{
    public class CharacterSkillManager: IController
    {
        public CharacterController Character;

        public Skill CurrentSpellingSkill;

        public List<Skill> Skills = new();

        public CharacterSkillManager(CharacterController character)
        {
            Character = character;
            Initialize();
        }

        public void Initialize()
        {
            var data = this.GetSystem<IDataManagerSystem>();
            Skills = data.GetUnitSkillsDefine(Character.Entity.UnitId)
                .Select(x => new Skill(this, x))
                .ToList();
        }

        public void Update()
        {
            foreach (var skill in Skills)
            {
                skill.Update();
            }
        }

        public Skill GetSkill(int skillId)
        {
            return Skills.FirstOrDefault(x => x.Define.ID == skillId);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using QFramework;

namespace MMORPG.Game
{
    public class CharacterSkillManager: IController
    {
        public ActorController ActorController;

        public Skill CurrentSpellingSkill;

        public List<Skill> Skills = new();

        public CharacterSkillManager(ActorController actorController)
        {
            ActorController = actorController;
        }

        public void Initialize()
        {
            var data = this.GetSystem<IDataManagerSystem>();
            Skills = data.GetUnitSkillsDefine(ActorController.Entity.UnitDefine.ID)
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

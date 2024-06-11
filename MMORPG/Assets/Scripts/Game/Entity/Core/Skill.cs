using Serilog;

namespace MMORPG.Game
{
    public class Skill
    {
        public CharacterSkillManager SkillManager;
        public SkillDefine Define;

        public bool IsUnitTarget => Define.TargetType == "Unit";
        public bool IsPositionTarget => Define.TargetType == "Position";
        public bool IsNoneTarget => Define.TargetType == "None";

        public Skill(CharacterSkillManager skillManager, SkillDefine define)
        {
            SkillManager = skillManager;
            Define = define;
        }

        public void Update()
        {

        }

        public void Use(CastTarget target)
        {
            Log.Debug($"{SkillManager.Entity.EntityId}使用技能{Define.ID}");
        }
    }
}

namespace MMORPG.Game
{
    public class Skill
    {
        public CharacterSkillManager Owner;
        public SkillDefine Define;

        public bool IsUnitTarget => Define.TargetType == "Unit";
        public bool IsPositionTarget => Define.TargetType == "Position";
        public bool IsNoneTarget => Define.TargetType == "None";

        public Skill(CharacterSkillManager owner, SkillDefine define)
        {
            Owner = owner;
            Define = define;
        }

        public void Update()
        {

        }

        public void Use(CastTarget target)
        {

        }
    }
}

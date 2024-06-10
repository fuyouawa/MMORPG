namespace MMORPG.Game
{
    public class Skill
    {
        public CharacterSkillManager Owner;
        public SkillDefine Define;

        public Skill(CharacterSkillManager owner, SkillDefine define)
        {
            Owner = owner;
            Define = define;
        }

        public void Update()
        {

        }

        public void Use()
        {

        }
    }
}

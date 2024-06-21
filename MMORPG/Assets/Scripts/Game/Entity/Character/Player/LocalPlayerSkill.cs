using QFramework;

namespace MMORPG.Game
{
    public class LocalPlayerSkill : LocalPlayerAbility, IController
    {
        private CharacterSkillManager _skillManager;

        public override void OnStateInit()
        {
            _skillManager = OwnerState.Brain.ActorController.SkillManager;
        }

        public override void OnStateEnter()
        {
            OwnerState.Brain.NetworkUploadTransform(OwnerStateId);
        }

        public override void OnStateNetworkFixedUpdate()
        {
            Brain.NetworkUploadTransform(OwnerStateId);
        }

        [StateCondition]
        public bool IsSpellingSkill()
        {
            return _skillManager.CurrentSpellingSkill != null;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}

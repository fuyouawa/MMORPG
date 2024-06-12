namespace MMORPG.Game
{
    public class LocalPlayerSkill : LocalPlayerAbility
    {
        private CharacterSkillManager _skillManager;
        private SkillsEffectManager _effectManager;

        public override void OnStateInit()
        {
            _skillManager = OwnerState.Brain.CharacterController.Entity.SkillManager;
            _effectManager = OwnerState.Brain.CharacterController.Entity.EffectManager;
        }

        public override void OnStateEnter()
        {
            OwnerState.Brain.NetworkUploadTransform(OwnerStateId);

            OwnerState.Brain.AnimationController.Animator.SetTrigger(_skillManager.CurrentSpellingSkill.Define.Anim2);

            if (_effectManager != null)
                _effectManager.TriggerEffect(_skillManager.CurrentSpellingSkill.Define.ID);
        }

        [StateCondition]
        public bool IsSpellingSkill()
        {
            return _skillManager.CurrentSpellingSkill != null;
        }
    }
}

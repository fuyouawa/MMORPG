using Common.Proto.Fight;
using MMORPG.System;
using QFramework;
using Serilog;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
    public class LocalPlayerSkill : LocalPlayerAbility, IController
    {
        private CharacterSkillManager _skillManager;

        public override void OnStateInit()
        {
            _skillManager = OwnerState.Brain.CharacterController.SkillManager;
        }

        public override void OnStateEnter()
        {
            OwnerState.Brain.NetworkUploadTransform(OwnerStateId);
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

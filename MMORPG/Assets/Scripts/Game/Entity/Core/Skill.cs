using System;
using System.Collections;
using QFramework;
using Serilog;
using UnityEngine;

namespace MMORPG.Game
{
    public enum SkillTargetTypes
    {
        Unit,
        Position,
        None
    }

    public enum SkillModes
    {
        Combo,
        Skill
    }

    public class Skill
    {
        public enum States
        {
            Idle,
            Running,
            Cooling
        }

        public CharacterSkillManager SkillManager { get; }
        public SkillDefine Define { get; }

        public SkillTargetTypes TargetType { get; }
        public SkillModes Mode { get; }

        public States CurrentState { get; private set; }

        public float RemainCd { get; private set; }

        public event Action OnStateChanged; 

        private PlayerHandleWeapon _handleWeapon;

        public Skill(CharacterSkillManager skillManager, SkillDefine define)
        {
            SkillManager = skillManager;
            Define = define;

            _handleWeapon = skillManager.ActorController.GetComponentInChildren<PlayerHandleWeapon>();

            TargetType = define.TargetType switch
            {
                "Unit" => SkillTargetTypes.Unit,
                "Position" => SkillTargetTypes.Position,
                _ => SkillTargetTypes.None
            };

            Mode = define.Mode switch
            {
                "Combo" => SkillModes.Combo,
                "Skill" => SkillModes.Skill,
                _ => throw new Exception("未知技能模式")
            };
        }

        public void Update()
        {
            if (CurrentState == States.Cooling)
            {
                if (RemainCd <= 0f)
                {
                    ChangeState(States.Idle);
                }
                else
                {
                    RemainCd -= Time.deltaTime;
                }
            }
        }

        public void Use(CastTarget target)
        {
            Log.Debug($"{SkillManager.ActorController.Entity.EntityId}使用技能{Define.Name}");
            switch (Mode)
            {
                case SkillModes.Combo:
                    UseCombo(target);
                    break;
                case SkillModes.Skill:
                    UseSkill(target);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UseCombo(CastTarget target)
        {
            if (_handleWeapon == null)
                throw new Exception($"Combo模式的技能({Define.Name})必须由持有PlayerHandleWeapon的对象释放!");
            _handleWeapon.CurrentComboWeapon.ChangeCombo(Define.ID);
            Debug.Assert(_handleWeapon.CurrentWeapon.WeaponId == Define.ID);
            _handleWeapon.CurrentWeapon.TurnWeaponOn();
        }

        private void UseSkill(CastTarget target)
        {
            if (SkillManager.CurrentSpellingSkill != null)
            {
                Log.Warning($"{SkillManager.ActorController.Entity.EntityId}尝试在释放技能({SkillManager.CurrentSpellingSkill.Define.Name})的时候使用其他技能:{Define.Name}");
                return;
            }
            if (CurrentState != States.Idle)
            {
                Log.Warning($"{SkillManager.ActorController.Entity.EntityId}尝试使用正在冷却中的技能:{Define.Name}");
                return;
            }
            SkillManager.ActorController.StartCoroutine(SpellSkillCo(target));
        }

        public IEnumerator SpellSkillCo(CastTarget target)
        {
            SkillManager.ActorController.Animator.SetTrigger(Define.Anim2);

            if (SkillManager.ActorController.EffectManager != null)
                SkillManager.ActorController.EffectManager.TriggerEffect(Define.ID);

            SkillManager.CurrentSpellingSkill = this;
            ChangeState(States.Running);
            yield return new WaitForSeconds(Define.Duration);
            SkillManager.CurrentSpellingSkill = null;
            ChangeState(States.Cooling);

            RemainCd = Define.Cd;
        }

        public void ChangeState(States state)
        {
            CurrentState = state;
            OnStateChanged?.Invoke();
        }
    }
}

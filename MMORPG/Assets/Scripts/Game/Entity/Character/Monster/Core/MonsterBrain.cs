using MMORPG.Common.Proto.Fight;
using MMORPG.Common.Proto.Entity;
using MMORPG.Tool;
using MMORPG.UI;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using AnimationState = MMORPG.Common.Proto.Entity.AnimationState;

namespace MMORPG.Game
{
    public class MonsterBrain : MonoBehaviour, IController
    {
        public float GroundClearance;

        public FSM<AnimationState> FSM = new ();

        public EntityTransformSyncData Data;

        [Required]
        public ActorController ActorController;

        public Transform DamageNumberPoint;
        public UIMonsterCanvas MonsterCanvas;

        [Title("Feedbacks")]
        public FeedbacksManager DeathFeedbacks;
        public FeedbacksManager ResurrectionFeedbacks;
        public FeedbacksManager AttackFeedback;

        private void Awake()
        {
            ActorController.Entity.OnTransformSync += OnTransformEntitySync;

            ActorController.Entity.OnHurt += OnHurt;
        }

        private void Start()
        {
            FSM.AddState(AnimationState.Idle, new MonsterIdleState(FSM, this));
            FSM.AddState(AnimationState.Move, new MonsterMoveState(FSM, this));
            FSM.AddState(AnimationState.Skill, new MonsterSkillState(FSM, this));
            FSM.AddState(AnimationState.Death, new MonsterDeathState(FSM, this));
            FSM.StartState(AnimationState.Idle);

            //TextName.text = ActorController.Entity.UnitDefine.Name;

            CorrectedMove(transform.position);
        }

        private void Update()
        {
            FSM.Update();
            //HPBar.Hp = ActorController.Hp;
            //HPBar.MaxHp = ActorController.MaxHp;
        }

        private void FixedUpdate()
        {
            FSM.FixedUpdate();
        }

        private void OnGUI()
        {
            FSM.OnGUI();
        }

        private void OnDestroy()
        {
            FSM.Clear();
        }


        private void OnHurt(DamageInfo info)
        {
            if (ActorController.Hp - info.Amount <= 0)
            {
                return;
            }

            if (!info.IsMiss)
            {
                ActorController.Animator.SetTrigger("Hurt");
            }
            if (info.IsMiss)
            {
                LevelManager.Instance.TakeText(DamageNumberPoint.position, "Miss");
            }
            else
            {
                LevelManager.Instance.TakeDamage(DamageNumberPoint.position, info.Amount, info.IsCrit);
            }
        }

        private void OnTransformEntitySync(EntityTransformSyncData data)
        {
            Data = data;
            AnimationState state = (AnimationState)data.StateId;
            if (FSM.CurrentStateId != state)
            {
                FSM.ChangeState(state);
            }

            CorrectedMove(data.Position);
            ActorController.SmoothRotate(data.Rotation);
        }


        public void CorrectedMove(Vector3 pos)
        {
            pos = CalculateGroundPosition(pos, LayerMask.NameToLayer("Ground"));
            pos.y += GroundClearance;
            ActorController.SmoothMove(pos);
        }

        private Vector3 CalculateGroundPosition(Vector3 position, int layer)
        {
            int layerMask = 1 << layer;
            if (Physics.Raycast(position, Vector3.down, out var hit, Mathf.Infinity, layerMask))
            {
                return hit.point;
            }
            return position;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}

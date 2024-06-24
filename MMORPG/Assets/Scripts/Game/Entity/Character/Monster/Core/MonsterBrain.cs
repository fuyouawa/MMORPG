using MMORPG.Common.Proto.Fight;
using MMORPG.Common.Proto.Entity;
using MMORPG.Event;
using MMORPG.Tool;
using MMORPG.UI;
using QFramework;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MMORPG.Game
{
    public class MonsterBrain : MonoBehaviour, IController
    {
        public FSM<ActorState> FSM = new ();

        public EntityTransformSyncData Data;

        [Required]
        public ActorController ActorController;

        public Transform DamageNumberPoint;

        [Title("Feedbacks")]
        public FeedbacksManager HurtFeedbacks;
        public FeedbacksManager CritHurtFeedbacks;
        public FeedbacksManager MissHurtFeedbacks;
        public FeedbacksManager DeathFeedbacks;
        public FeedbacksManager ResurrectionFeedbacks;

        private void Awake()
        {
            ActorController.Entity.OnTransformSync += OnTransformEntitySync;

            ActorController.Entity.OnHurt += OnHurt;
        }

        private void Start()
        {
            FSM.AddState(ActorState.Idle, new MonsterIdleState(FSM, this));
            FSM.AddState(ActorState.Move, new MonsterMoveState(FSM, this));
            FSM.AddState(ActorState.Skill, new MonsterAttackState(FSM, this));
            FSM.AddState(ActorState.Death, new MonsterDeathState(FSM, this));
            FSM.StartState(ActorState.Idle);

            //TextName.text = ActorController.Entity.UnitDefine.Name;

            ActorController.SmoothMove(CalculateGroundPosition(transform.position, 6));
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

            if (info.IsCrit)
            {
                if (CritHurtFeedbacks != null)
                    CritHurtFeedbacks.Play();
            }
            else if (info.IsMiss)
            {
                if (MissHurtFeedbacks != null)
                    MissHurtFeedbacks.Play();
            }
            else
            {
                if (HurtFeedbacks != null)
                    HurtFeedbacks.Play();
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
            ActorState state = (ActorState)data.StateId;
            if (FSM.CurrentStateId != state)
            {
                FSM.ChangeState(state);
            }

            ActorController.SmoothMove(CalculateGroundPosition(data.Position, 6));
            ActorController.SmoothRotate(data.Rotation);
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

using MMORPG.Common.Proto.Fight;
using MMORPG.Common.Proto.Monster;
using MMORPG.Event;
using MMORPG.Tool;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class MonsterBrain : MonoBehaviour, IController
    {
        public Animator Animator;

        public FSM<ActorState> FSM = new ();

        public EntityTransformSyncData Data;

        [Required]
        public ActorController ActorController;

        public Transform DamageNumberPoint;

        [Title("Feedbacks")]
        public FeedbacksManager HurtFeedbacks;
        public FeedbacksManager CritHurtFeedbacks;
        public FeedbacksManager MissHurtFeedbacks;

        private void Awake()
        {
            ActorController.Entity.OnTransformSync += OnTransformEntitySync;

            ActorController.Entity.OnHurt += OnHurt;
        }

        private void OnHurt(DamageInfo info)
        {
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
                Animator.SetTrigger("Hurt");
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

        private void Start()
        {
            FSM.AddState(ActorState.Idle, new IdleState(FSM, this));
            FSM.AddState(ActorState.Move, new MoveState(FSM, this));
            FSM.AddState(ActorState.Skill, new AttackState(FSM, this));
            FSM.StartState(ActorState.Idle);
        }

        private void Update()
        {
            FSM.Update();
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

        public Vector3 CalculateGroundPosition(Vector3 position, int layer)
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

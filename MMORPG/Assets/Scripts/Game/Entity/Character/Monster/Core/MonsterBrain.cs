using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Proto.Monster;
using MMORPG.Global;
using MMORPG.Tool;
using QFramework;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

namespace MMORPG.Game
{
    public class MonsterBrain : MonoBehaviour
    {
        public Animator Animator;

        public FSM<ActorState> FSM = new ();

        public EntityTransformSyncData Data;

        public class IdleState : AbstractState<ActorState, MonsterBrain>
        {

            public IdleState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
            {

            }

            protected override void OnEnter()
            {
                
            }

            protected override void OnUpdate()
            {
                
            }
        }

        public class MoveState : AbstractState<ActorState, MonsterBrain>
        {
            public MoveState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
            {

            }

            protected override void OnEnter()
            {
                mTarget.Animator.SetBool("Walking", true);
            }

            protected override void OnExit()
            {
                mTarget.Animator.SetBool("Walking", false);
            }

            protected override void OnUpdate()
            {
                mTarget.CharacterController.SmoothMove(CalculateGroundPosition(mTarget.Data.Position, 6));
                mTarget.CharacterController.SmoothRotate(mTarget.Data.Rotation);
                //mTarget.Position = mTarget.Data.Position;
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
        }

        public class AttackState : AbstractState<ActorState, MonsterBrain>
        {
            public AttackState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
            {

            }

            protected override void OnEnter()
            {
                mTarget.Animator.SetBool("Attack", true);
            }

            protected override void OnExit()
            {
                mTarget.Animator.SetBool("Attack", false);
            }
        }

        [Required]
        public CharacterController CharacterController;
        [Required]
        //public MonsterAnimationController AnimationController;


        private void Awake()
        {
            //AnimationController.Setup(this);
            CharacterController.Entity.OnTransformSync += OnTransformEntitySync;
        }

        private void OnTransformEntitySync(EntityTransformSyncData data)
        {
            ActorState state = (ActorState)data.StateId;
            if (FSM.CurrentStateId != state)
            {
                FSM.ChangeState(state);
            }

            Data = data;
            //state.Actions.ForEach(x => x.TransformEntitySync(data));
        }

        private void Start()
        {
            FSM.AddState(ActorState.Idle, new IdleState(FSM, this));
            FSM.AddState(ActorState.Move, new MoveState(FSM, this));
            FSM.AddState(ActorState.Attack, new AttackState(FSM, this));
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

        private void OnFireStarted(InputAction.CallbackContext obj)
        {
            CharacterController.HandleWeapon.ShootStart();
        }

    }

}

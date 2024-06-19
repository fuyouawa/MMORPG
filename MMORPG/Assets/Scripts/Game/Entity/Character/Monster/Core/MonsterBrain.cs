using MMORPG.Common.Proto.Monster;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class MonsterBrain : MonoBehaviour
    {
        public Animator Animator;

        public FSM<ActorState> FSM = new ();

        public EntityTransformSyncData Data;

        [Required]
        public CharacterController CharacterController;


        private void Awake()
        {
            CharacterController.Entity.OnTransformSync += OnTransformEntitySync;
        }

        private void OnTransformEntitySync(EntityTransformSyncData data)
        {
            ActorState state = (ActorState)data.StateId;
            if (FSM.CurrentStateId != state)
            {
                FSM.ChangeState(state);
            }

            CharacterController.SmoothMove(CalculateGroundPosition(data.Position, 6));
            CharacterController.SmoothRotate(data.Rotation);
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
    }

}

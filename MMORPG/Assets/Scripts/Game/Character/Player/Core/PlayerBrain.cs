using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MMORPG.Global;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace MMORPG.Game
{
    public class PlayerBrain : MonoBehaviour
    {
        [Required]
        public CharacterController CharacterController;
        [Required]
        public PlayerAnimationController AnimationController;

#if UNITY_EDITOR
        [SerializeField]
        [ReadOnly]
        [LabelText("CurrentState")]
        private string _currentStateName = "NONE";
#endif

        // public string StartStateName = string.Empty;

        [InfoBox("Error occur in States!", InfoMessageType.Error, "CheckStatesHasError")]
        [InfoBox("Empty state machine is meaningless", InfoMessageType.Warning, "IsEmptyStates")]
        [InfoBox("The state machine name cannot be the same!", InfoMessageType.Error, "HasRepeatStateName")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Name")]
        public PlayerState[] States;

        public PlayerState CurrentState { get; private set; }

        public GameInputControls InputControls { get; private set; }

        public LocalPlayerAbility[] GetAttachLocalAbilities() => GetAttachAbilities<LocalPlayerAbility>();

        public RemotePlayerAbility[] GetAttachRemoteAbilities() => GetAttachAbilities<RemotePlayerAbility>();

        public bool IsMine => CharacterController.Entity.IsMine;

        private TAbility[] GetAttachAbilities<TAbility>() where TAbility : PlayerAbility
        {
            var total = new List<TAbility>();
            total.AddRange(GetComponents<TAbility>());
            foreach (var node in CharacterController.AdditionalAbilityNodes)
            {
                total.AddRange(node.GetComponents<TAbility>());
            }
            return total.ToArray();
        }

        public void ChangeState(PlayerState state)
        {
            Debug.Assert(state != null);
            Debug.Assert(States.Contains(state));
            CurrentState?.Exit();
            CurrentState = state;
#if UNITY_EDITOR
            _currentStateName = CurrentState.Name;
#endif
            CurrentState.Enter();
        }

        public PlayerState GetState(string stateName)
        {
            return Array.Find(States, x => x.Name == stateName);
        }

        private void Awake()
        {
            AnimationController.Brain = this;
            CurrentState = null;
            InputControls = new();
            if (States.Length == 0) return;
            CharacterController.Entity.OnTransformSync += OnTransformEntitySync;
        }

        private void OnTransformEntitySync(EntityTransformSyncData data)
        {
            Debug.Assert(!CharacterController.Entity.IsMine);
            var state = States[data.StateId];
            Debug.Assert(state != null);
            if (state != CurrentState)
            {
                ChangeState(state);
            }

            state.Actions.ForEach(x => x.TransformEntitySync(data));
        }

        private void Start()
        {
            if (States.IsNullOrEmpty()) return;
            InitStates();
            ChangeState(States[0]);
            StartCoroutine(NetworkFixedUpdate());
        }

        private void Update()
        {
            if (States.IsNullOrEmpty()) return;
            CurrentState?.Update();
        }

        private void FixedUpdate()
        {
            if (States.IsNullOrEmpty()) return;
            CurrentState?.FixedUpdate();
        }


        private IEnumerator NetworkFixedUpdate()
        {
            while (true)
            {
                CurrentState?.NetworkFixedUpdate();
                yield return new WaitForSeconds(Config.NetworkUpdateDeltaTime);
            }
        }

        private void OnEnable()
        {
            InputControls.Enable();
        }

        private void OnDisable()
        {
            InputControls.Disable();
        }

        private void InitStates()
        {
            States.ForEach((x, i) =>
            {
                x.Initialize(this, i);
                x.OnTransitionEvaluated += (transition, condition) =>
                {
                    ChangeState(condition ? transition.TrueState : transition.FalseState);
                };
            });
        }



#if UNITY_EDITOR
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            States?.ForEach(x => x.Brain = this);
        }

        private bool HasRepeatStateName => States.GroupBy(x => x.Name).Any(g => g.Count() > 1);

        private bool IsEmptyStates => States.Length == 0;

        private bool CheckStatesHasError()
        {
            return States.Any(x => x.HasError());
        }
#endif
    }

}

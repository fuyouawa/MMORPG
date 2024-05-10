using System;
using QFramework;
using Sirenix.OdinInspector;

namespace MMORPG.Game
{
    [Serializable]
    public class PlayerState
    {
        [Required("The status name cannot be empty!")]
        public string Name = "TODO";

        [InfoBox("Actions cannot be empty!", InfoMessageType.Error, "IsEmptyActions")]
        [TabGroup("Actions")]
        [TableList(AlwaysExpanded = true)]
        public PlayerAction[] Actions;

        [InfoBox("Transitions cannot be empty!", InfoMessageType.Error, "IsEmptyTransitions")]
        [TabGroup("Transitions")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Label")]
        public PlayerTransition[] Transitions;

        public int StateId { get; private set; }

        public delegate void TransitionEvaluatedHandler(PlayerTransition transition, bool condition);
        public event TransitionEvaluatedHandler OnTransitionEvaluated;

        public PlayerBrain Brain { get; set; }

        public bool IsMine => Brain.IsMine;

        public void Initialize(PlayerBrain brain, int stateId)
        {
            Brain = brain;
            StateId = stateId;
            foreach (var transition in Transitions)
            {
                transition.Initialize(this);
                transition.OnEvaluated += condition => OnTransitionEvaluated?.Invoke(transition, condition);
            }

            foreach (var action in Actions)
            {
                action.Initialize(this, stateId);
            }
        }

        public void Enter()
        {
            Actions.ForEach(x => x.Enter());
        }

        public void Update()
        {
            Actions.ForEach(x => x.Update());
            if (IsMine)
            {
                Transitions.ForEach(x => x.Evaluate());
            }
        }

        public void FixedUpdate()
        {
            Actions.ForEach(x => x.FixedUpdate());
        }

        public void NetworkFixedUpdate()
        {
            Actions.ForEach(x => x.NetworkFixedUpdate());
        }

        public void Exit()
        {
            Actions.ForEach(x => x.Exit());
        }

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            Transitions?.ForEach(x => x.OwnerState = this);
            Actions?.ForEach(x => x.OwnerState = this);
        }

        private bool IsEmptyActions => Actions == null || Actions.Length == 0;
        private bool IsEmptyTransitions => Transitions == null || Transitions.Length == 0;
#endif
    }

    public class StateConditionAttribute : Attribute { }

}

using System;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;

namespace MMORPG.Game
{
    [Serializable]
    public class PlayerState
    {
        [Required("The status name cannot be empty!")]
        public string Name = "TODO";

        [InfoBox("Error occur in Actions!", InfoMessageType.Error, "CheckActionsHasError")]
        [InfoBox("Actions cannot be empty!", InfoMessageType.Error, "IsEmptyActions")]
        [TabGroup("Actions")]
        [TableList(AlwaysExpanded = true)]
        public PlayerAction[] Actions;

        [InfoBox("Error occur in Transitions!", InfoMessageType.Error, "CheckTransitionsHasError")]
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

        private bool IsEmptyActions => Actions.IsNullOrEmpty();
        private bool IsEmptyTransitions => Transitions.IsNullOrEmpty();

        private bool CheckActionsHasError()
        {
            return Actions.Any(x => x.HasError());
        }

        private bool CheckTransitionsHasError()
        {
            return Transitions.Any(x => x.HasError());
        }

        public bool HasError()
        {
            return CheckActionsHasError() || CheckTransitionsHasError() || IsEmptyActions || IsEmptyTransitions;
        }
#endif
    }

    public class StateConditionAttribute : Attribute { }

}

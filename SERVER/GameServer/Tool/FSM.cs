using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tool
{
    public interface IFSMState
    {
        bool Condition();
        void Enter();
        void Update();
        void Exit();
    }

    public class FSM<TStateId> where TStateId : notnull
    {
        private Dictionary<TStateId, IFSMState> _statusDict = new();

        public TStateId? CurrentStateId { get; private set; }
        public IFSMState? CurrentState { get; private set; }


        public void AddState(TStateId stateId, IFSMState state)
        {
            _statusDict[stateId] = state;
        }

        public bool RemoveState(TStateId stateId)
        {
            return _statusDict.Remove(stateId);
        }

        public void ChangeState(TStateId stateId)
        {
            if (CurrentStateId?.Equals(stateId) == true) return;
            if (!_statusDict.ContainsKey(stateId)) return;
            CurrentState?.Exit();
            CurrentStateId = stateId;
            CurrentState = _statusDict[stateId];
            CurrentState.Enter();
        }

        public void Update()
        {
            CurrentState?.Update();
        }
    }

    public abstract class FSMAbstractState<TStateId, TTarget> : IFSMState
        where TStateId : notnull
    {
        protected FSM<TStateId> _fsm;
        protected TTarget _target;

        protected FSMAbstractState(FSM<TStateId> fsm, TTarget target)
        {
            _fsm = fsm;
            _target = target;
        }


        bool IFSMState.Condition()
        {
            return OnCondition();
        }

        void IFSMState.Enter()
        {
            OnEnter();
        }

        void IFSMState.Update()
        {
            OnUpdate();
        }

        void IFSMState.Exit()
        {
            OnExit();
        }

        protected virtual bool OnCondition() => true;

        public virtual void OnEnter() { }

        public virtual void OnUpdate() { }

        public virtual void OnExit() { }
    }

}

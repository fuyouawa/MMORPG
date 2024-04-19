using GameServer.Manager;
using GameServer.Tool;
using GameServer.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.System.Ai
{

    public enum MonsterAiState
    {
        Idle = 1,
        Walk = 2,
    }

    public class MonsterAi : AiBase
    {
        public FSM<MonsterAiState> FSM;

        public MonsterAi(Monster monster)
        {
            FSM = new();
            FSM.AddState(MonsterAiState.Walk, new WalkState(FSM, monster));
        }


        public override void Update()
        {
            FSM.Update();
        }


        public class WalkState : FSMAbstractState<MonsterAiState, Monster>
        {
            private float _lastTime;

            public WalkState(FSM<MonsterAiState> fsm, Monster monster) : 
                base(fsm, monster)
            {
                _lastTime = EntityManager.Instance.Time.time;
            }

            public override void OnEnter()
            {
                _target.MoveStop();
            }

            public override void OnUpdate()
            {
                if (_target.State == ActorState.Idle) 
                {
                    if (_lastTime + 10f < EntityManager.Instance.Time.time)
                    {
                        _lastTime = EntityManager.Instance.Time.time;
                        // 移动到随机位置
                        _target.MoveTo(_target.RandomPointWithBirth(10));
                    }
                }
            }
        }

    }
}

using MMORPG.Command;
using QFramework;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
    public class PlayingState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        public PlayingState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            Tool.Log.Info("Launch", "初始化地图");
            var group = new GameObject("Manager(Auto Create)").transform;
            new GameObject(nameof(MapManager)).AddComponent<MapManager>().transform.SetParent(group);
            new GameObject(nameof(EntityManager)).AddComponent<EntityManager>().transform.SetParent(group);
            new GameObject(nameof(PlayerManager)).AddComponent<PlayerManager>().transform.SetParent(group);
        }

        protected override void OnExit()
        {
            this.SendCommand(new ExitMapCommand());
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}

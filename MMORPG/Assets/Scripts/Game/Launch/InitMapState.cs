using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public class InitMapState : AbstractState<LaunchStatus, LaunchController>
    {
        public InitMapState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
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
    }
}

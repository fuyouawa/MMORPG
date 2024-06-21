using MMORPG.Event;
using QFramework;
using Serilog;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MMORPG.Game
{
    public class InLobbyState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        public InLobbyState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
            this.RegisterEvent<JoinMapEvent>(OnJoinedMap)
                .UnRegisterWhenGameObjectDestroyed(mTarget.gameObject);
        }

        private void OnJoinedMap(JoinMapEvent e)
        {
            //TODO 根据MapId加载地图
            var op = SceneManager.LoadSceneAsync("Space1Scene");

            op.completed += operation =>
            {
                Log.Information("初始化地图");
                var group = new GameObject("Managers(Auto Create)").transform;

                var entityManager = new GameObject(nameof(EntityManager)).AddComponent<EntityManager>();
                entityManager.transform.SetParent(group, false);

                var mapManager = new GameObject(nameof(MapManager)).AddComponent<MapManager>();
                mapManager.transform.SetParent(group, false);

                var fightManager = new GameObject(nameof(FightManager)).AddComponent<FightManager>();
                fightManager.transform.SetParent(group, false);

                mFSM.ChangeState(LaunchStatus.Playing);

                mapManager.OnJoinMap(e.CharacterId);
            };
        }

        protected override void OnEnter()
        {
            Log.Information("开始等待加入地图");
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}

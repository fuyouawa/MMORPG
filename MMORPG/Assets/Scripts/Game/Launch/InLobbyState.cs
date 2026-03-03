using MMORPG.Event;
using MMORPG.System;
using QFramework;
using Serilog;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MMORPG.Game
{
    public class InLobbyState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        private IDataManagerSystem _dataManager;
        private IEntityManagerSystem _entityManager;

        public InLobbyState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
            _dataManager = this.GetSystem<IDataManagerSystem>();
            _entityManager = this.GetSystem<IEntityManagerSystem>();

            this.RegisterEvent<WannaJoinMapEvent>(OnWannaJoinMap)
                .UnRegisterWhenGameObjectDestroyed(mTarget.gameObject);
        }

        private void OnWannaJoinMap(WannaJoinMapEvent e)
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

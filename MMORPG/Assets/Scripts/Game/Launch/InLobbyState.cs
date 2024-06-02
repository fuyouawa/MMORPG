using MMORPG.Event;
using QFramework;
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
                Tool.Log.Info("Launch", "初始化地图");
                var group = new GameObject("Manager(Auto Create)").transform;

                var entityManager = new GameObject(nameof(EntityManager)).AddComponent<EntityManager>();
                entityManager.transform.SetParent(group);

                var mapManager = new GameObject(nameof(MapManager)).AddComponent<MapManager>();
                mapManager.transform.SetParent(group);

                mFSM.ChangeState(LaunchStatus.Playing);

                mapManager.OnJoinMap(e.CharacterId);
            };
        }

        protected override void OnEnter()
        {
            Tool.Log.Info("Launch", "开始等待加入地图");
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}

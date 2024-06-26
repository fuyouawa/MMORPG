using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Player;
using MMORPG.Common.Tool;
using MMORPG.Event;
using MMORPG.System;
using MMORPG.Tool;
using MoonSharp.VsCodeDebugger.SDK;
using QFramework;
using Serilog;
using ThirdPersonCamera;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

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
            var net = this.GetSystem<INetworkSystem>();
            net.SendToServer(new JoinMapRequest
            {
                CharacterId = e.CharacterId,
            });
            net.Receive<JoinMapResponse>(response =>
            {
                if (response.Error != Common.Proto.Base.NetError.Success)
                {
                    Log.Error($"JoinMap Error:{response.Error.GetInfo().Description}");
                    //TODO Error处理
                    return;
                }

                Log.Information($"JoinMap Success, MineId:{response.EntityId}");

                var unitDefine = _dataManager.GetUnitDefine(response.UnitId);

                var resLoader = ResLoader.Allocate();

                var entity = _entityManager.SpawnEntity(
                    resLoader.LoadSync<EntityView>(unitDefine.Resource),    //TODO 角色生成
                    response.EntityId,
                    response.UnitId,
                    EntityType.Player,
                    response.Transform.Position.ToVector3(),
                    Quaternion.Euler(response.Transform.Direction.ToVector3()));

                resLoader.Recycle2Cache();

                GameObject.DontDestroyOnLoad(entity);

                this.GetSystem<IPlayerManagerSystem>().SetMine(entity);

                entity.GetComponent<ActorController>().ApplyNetActor(response.Actor);


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

                    mapManager.OnJoinMap(entity);
                };
            });
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

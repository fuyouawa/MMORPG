using System;
using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Player;
using MMORPG.Common.Tool;
using MMORPG.Event;
using MMORPG.Global;
using MMORPG.Model;
using QFramework;
using MMORPG.System;
using MMORPG.Tool;
using Serilog;
using ThirdPersonCamera;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace MMORPG.Game
{
    /// <summary>
    /// 地图控制器
    /// 负责监听在当前地图中创建角色的事件并将角色加入到地图
    /// </summary>
    public class MapManager : MonoBehaviour, IController, ICanSendEvent
    {
        private IPlayerManagerSystem _playerManager;
        private IEntityManagerSystem _entityManager;
        private IDataManagerSystem _dataManager;

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        void Awake()
        {
            _playerManager = this.GetSystem<IPlayerManagerSystem>();
            _entityManager = this.GetSystem<IEntityManagerSystem>();
            _dataManager = this.GetSystem<IDataManagerSystem>();
        }

        public void OnJoinMap(long characterId)
        {
            var net = this.GetSystem<INetworkSystem>();
            net.SendToServer(new JoinMapRequest
            {
                CharacterId = characterId,
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

                var entity = _entityManager.SpawnEntity(
                    Resources.Load<EntityView>($"{Config.PlayerPrefabsPath}/{unitDefine.Resource}"),
                    response.EntityId,
                    response.UnitId,
                    EntityType.Player,
                    response.Transform.Position.ToVector3(),
                    Quaternion.Euler(response.Transform.Direction.ToVector3()));

                this.GetSystem<IPlayerManagerSystem>().SetMine(entity);

                entity.GetComponent<ActorController>().ApplyNetActor(response.Actor, true);

                Camera.main.GetComponent<CameraController>().InitFromTarget(entity.transform);
                foreach (var followTarget in FindObjectsByType<FollowTarget>(FindObjectsSortMode.None))
                {
                    followTarget.Target = entity.transform;
                }
            });
        }
    }
}

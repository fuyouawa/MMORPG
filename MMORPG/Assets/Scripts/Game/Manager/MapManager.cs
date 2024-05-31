using Common.Proto.Player;
using Common.Tool;
using QFramework;
using MMORPG.System;
using MMORPG.Tool;
using ThirdPersonCamera;
using UnityEngine;


namespace MMORPG.Game
{
    /// <summary>
    /// 地图控制器
    /// 负责监听在当前地图中创建角色的事件并将角色加入到地图
    /// </summary>
    public class MapManager : MonoBehaviour, IController
    {
        private IPlayerManagerSystem _playerManager;
        private IEntityManagerSystem _entityManager;
        private IDataManagerSystem _dataManager;
        private ResLoader _resLoader = ResLoader.Allocate();

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

        public void OnJoinedMap(int characterId)
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
                    Tool.Log.Error("Network", $"JoinMap Error:{response.Error.GetInfo().Description}");
                    //TODO Error处理
                    return;
                }

                Tool.Log.Info("Network", $"JoinMap Success, MineId:{response.EntityId}");

                var unitDefine = _dataManager.GetUnitDefine(response.UnitId);

                var entity = _entityManager.SpawnEntity(
                    _resLoader.LoadSync<EntityView>(unitDefine.Resource),    //TODO 角色生成
                    response.EntityId,
                    EntityType.Player,
                    true,
                    response.Transform.Position.ToVector3(),
                    Quaternion.Euler(response.Transform.Direction.ToVector3()));

                _playerManager.SetMine(entity);

                Camera.main.GetComponent<CameraController>().InitFromTarget(entity.transform);
            });
        }

        void OnDestroy()
        {
            _resLoader.Recycle2Cache();
            _resLoader = null;
        }
    }
}

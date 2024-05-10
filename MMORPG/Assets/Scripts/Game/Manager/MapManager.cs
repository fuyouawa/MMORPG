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
        private ResLoader _resLoader = ResLoader.Allocate();

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        void Awake()
        {
            _playerManager = this.GetSystem<IPlayerManagerSystem>();
            _entityManager = this.GetSystem<IEntityManagerSystem>();
        }

        async void Start()
        {
            var box = this.GetSystem<IBoxSystem>();
            var net = this.GetSystem<INetworkSystem>();
            box.ShowSpinner("");
            net.SendToServer(new JoinMapRequest
            {
                CharacterId = 1,
            });
            var response = await net.ReceiveAsync<JoinMapResponse>();
            box.CloseSpinner();
            if (response.Error != Common.Proto.Base.NetError.Success)
            {
                Tool.Log.Error("Network", $"JoinMap Error:{response.Error.GetInfo().Description}");
                //TODO Error处理
                return;
            }

            Tool.Log.Info("Network", $"JoinMap Success, MineId:{response.EntityId}");
            _playerManager.SetMineId(response.EntityId);

            var entity = _entityManager.SpawnEntity(
                _resLoader.LoadSync<EntityView>("HeroKnightMale"),
                response.EntityId,
                response.Transform.Position.ToVector3(),
                Quaternion.Euler(response.Transform.Direction.ToVector3()),
                true);

            Camera.main.GetComponent<CameraController>().InitFromTarget(entity.transform);
        }

        void OnDestroy()
        {
            _resLoader.Recycle2Cache();
            _resLoader = null;
        }
    }
}

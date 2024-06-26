using System;
using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Player;
using MMORPG.Common.Tool;
using MMORPG.Event;
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

        public void OnJoinMap(EntityView entity)
        {
            SceneManager.MoveGameObjectToScene(entity.gameObject, gameObject.scene);

            Camera.main.GetComponent<CameraController>().InitFromTarget(entity.transform);
        }

        void OnDestroy()
        {
            _resLoader.Recycle2Cache();
            _resLoader = null;
        }
    }
}

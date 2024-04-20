using Common.Proto.Space;
using MMORPG;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPersonCamera;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IController
{
    public GameObject PlayerPrefab;

    private IPlayerManagerSystem _playerManager;

    private void Awake()
    {
        _playerManager = this.GetSystem<IPlayerManagerSystem>();
        this.RegisterEventInUnityThread<PlayerEnterEvent>(OnPlayerEnter)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnPlayerEnter(PlayerEnterEvent e)
    {
        var inst = Instantiate(PlayerPrefab);
        var entity = inst.GetComponent<EntityController>();
        entity.EntityId = e.EntityId;
        entity.IsMine = _playerManager.MineId == e.EntityId;
        if (entity.IsMine) {
            var camera = Camera.main.GetComponent<CameraController>();
            camera.InitFromTarget(inst.transform);
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
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
    private IEntityManagerSystem _entityManager;
    private ResLoader _resLoader = ResLoader.Allocate();

    private void Awake()
    {
        _entityManager = this.GetSystem<IEntityManagerSystem>();
        this.RegisterEventInUnityThread<NetworkEntityEnterEvent>(OnPlayerEnter)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnPlayerEnter(NetworkEntityEnterEvent e)
    {
        var player = Instantiate(_resLoader.LoadSync<NetworkEntity>("DogPBR"));
        player.transform.position = e.EntityInfo.Position;
        player.transform.rotation = e.EntityInfo.Rotation;

        _entityManager.RegisterEntity(e.EntityInfo.EntityId, player, new() { IsMine = false });
        this.SendCommand(new PlayerJoinedCommand(player));
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
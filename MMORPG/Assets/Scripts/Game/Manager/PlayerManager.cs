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
    private IPlayerManagerSystem _playerManager;
    private ResLoader _resLoader = ResLoader.Allocate();

    private void Awake()
    {
        _playerManager = this.GetSystem<IPlayerManagerSystem>();
        this.RegisterEventInUnityThread<NetworkEntityEnterEvent>(OnPlayerEnter)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnPlayerEnter(NetworkEntityEnterEvent e)
    {
        var player = Instantiate(_resLoader.LoadSync<Player>("DogPBR"));
        _playerManager.RegisterPlayer(player, e.EntityInfo.EntityId);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
using MMORPG;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPersonCamera;
using UnityEngine;

//TODO PlayerManager
public class PlayerManager : MonoBehaviour, IController
{
    private IEntityManagerSystem _entityManager;
    private ResLoader _resLoader = ResLoader.Allocate();

    //private void Awake()
    //{
    //    _entityManager = this.GetSystem<IEntityManagerSystem>();
    //    this.RegisterEventInUnityThread<NetworkEntityEnterEvent>(OnNetworkPlayerEnter)
    //        .UnRegisterWhenGameObjectDestroyed(gameObject);
    //}

    //private void OnNetworkPlayerEnter(NetworkEntityEnterEvent e)
    //{
    //    var player = Instantiate(_resLoader.LoadSync<NetworkEntity>("DogPBR"));
    //    player.transform.position = e.Position;
    //    player.transform.rotation = e.Rotation;

    //    _entityManager.RegisterEntity(e.EntityId, player, new() { IsMine = false });
    //    this.SendCommand(new PlayerJoinedCommand(player));
    //}

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }

    void OnDestroy()
    {
        _resLoader.Recycle2Cache();
        _resLoader = null;
    }
}

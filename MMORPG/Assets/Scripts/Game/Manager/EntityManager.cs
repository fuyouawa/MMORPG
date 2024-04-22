using Common.Proto.Entity;
using Common.Proto.Event.Space;
using MMORPG;
using MoonSharp.VsCodeDebugger.SDK;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool;
using UnityEngine;

public interface INetworkEntityCallbacks
{
    public void OnNetworkSync(EntitySyncData data);
}


public class EntityManager : MonoBehaviour, IController, ICanSendEvent
{
    private IEntityManagerSystem _entityManager;
    private ResLoader _resLoader = ResLoader.Allocate();

    private void Awake()
    {
        _entityManager = this.GetSystem<IEntityManagerSystem>();

        this.GetSystem<INetworkSystem>().ReceiveEvent<EntityEnterResponse>(OnEntityEnterReceived)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetSystem<INetworkSystem>().ReceiveEvent<EntitySyncResponse>(OnEntitySyncReceived)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnEntityEnterReceived(EntityEnterResponse response)
    {
        foreach (var netEntity in response.EntityList)
        {
            var entityId = netEntity.EntityId;
            var position = netEntity.Position.ToVector3();
            var rotation = Quaternion.Euler(netEntity.Direction.ToVector3());

            //TODO 根据Entity加载对应的Prefab
            var entity = Instantiate(_resLoader.LoadSync<NetworkEntity>("DogPBR"));
            entity.transform.SetPositionAndRotation(position, rotation);

            _entityManager.RegisterEntity(entityId, entity, new() { IsMine = false });

            this.SendEvent(new EntityEnterEvent(entity));
        }
    }

    private void OnEntitySyncReceived(EntitySyncResponse response)
    {
        var entityId = response.EntitySync.Entity.EntityId;
        var position = response.EntitySync.Entity.Position.ToVector3();
        var rotation = Quaternion.Euler(response.EntitySync.Entity.Direction.ToVector3());

        var entity = _entityManager.GetEntityById(entityId);
        entity.GetComponents<INetworkEntityCallbacks>().ForEach(cb => {
            cb.OnNetworkSync(new(position, rotation));
        });
        this.SendEvent(new EntitySyncEvent(entity, position, rotation));
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
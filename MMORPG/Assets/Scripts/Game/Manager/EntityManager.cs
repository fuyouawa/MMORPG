using Common.Proto.Event.Space;
using MMORPG;
using MoonSharp.VsCodeDebugger.SDK;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool;
using UnityEngine;

public struct NetworkControlData
{
    public float DeltaTime;
}

public struct NetworkSyncData
{
    public Vector3 Postion;
    public Quaternion Rotation;
}


public interface INetworkEntityCallbacks
{
    public void NetworkControlFixedUpdate(NetworkControlData data);
    public void NetworkSyncUpdate(NetworkSyncData data);
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
            _entityManager.SpawnEntity(_resLoader.LoadSync<Entity>("HeroKnightFemale"), entityId, position, rotation, false);
        }
    }

    private void OnEntitySyncReceived(EntitySyncResponse response)
    {
        var entityId = response.EntitySync.Entity.EntityId;
        var position = response.EntitySync.Entity.Position.ToVector3();
        var rotation = Quaternion.Euler(response.EntitySync.Entity.Direction.ToVector3());
        var entity = _entityManager.GetEntityDict(false)[entityId];

        var data = new NetworkSyncData
        {
            Postion = position,
            Rotation = rotation
        };

        entity.GetComponents<INetworkEntityCallbacks>().ForEach(cb => {
            cb.NetworkSyncUpdate(data);
        });
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}

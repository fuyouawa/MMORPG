using Common.Proto.Entity;
using Common.Proto.Event.Space;
using MMORPG;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class NetworkEntityEnterEvent
{
    public int EntityId { get; }
    public Vector3 Position { get; }
    public Quaternion Rotation { get; }

    public NetworkEntityEnterEvent(int entityId, Vector3 position, Quaternion rotation)
    {
        EntityId = entityId;
        Position = position;
        Rotation = rotation;
    }
}
public class EntitySyncData
{
    public Vector3 Postion { get; }
    public Quaternion Rotation { get; }

    public EntitySyncData(Vector3 position, Quaternion rotation)
    {
        Postion = position;
        Rotation = rotation;
    }
}

public class NetworkEntitySyncEvent : EntitySyncData
{
    public int EntityId { get; }

    public NetworkEntitySyncEvent(int entityId, Vector3 position, Quaternion rotation)
        : base(position, rotation)
    {
        EntityId = entityId;
    }
}

public struct EntityInfo
{
    public bool IsMine;
}

public interface IEntityManagerSystem : ISystem
{
    public void RegisterEntity(int entityId, NetworkEntity entity, EntityInfo info);
    public void UnregisterEntity(int entityId);

    public NetworkEntity GetEntityById(int entityId);
    public bool TryGetEntityById(int entityId, out NetworkEntity entity);
}


public class EntityManagerSystem : AbstractSystem, IEntityManagerSystem
{
    private Dictionary<int, NetworkEntity> _entityDict = new();

    public NetworkEntity GetEntityById(int entityId)
    {
        return _entityDict[entityId];
    }

    public void RegisterEntity(int entityId, NetworkEntity entity, EntityInfo info)
    {
        Debug.Assert(!_entityDict.ContainsKey(entityId));
        entity.SetEntityId(entityId);
        entity.SetIsMine(info.IsMine);
        _entityDict[entityId] = entity;
    }

    public bool TryGetEntityById(int entityId, out NetworkEntity entity)
    {
        return _entityDict.TryGetValue(entityId, out entity);
    }

    public void UnregisterEntity(int entityId)
    {
        _entityDict.Remove(entityId);
    }

    protected override void OnInit()
    {
        this.GetSystem<INetworkSystem>().ReceiveEvent<EntityEnterResponse>(OnEntityEnterReceived);
        this.GetSystem<INetworkSystem>().ReceiveEvent<EntitySyncResponse>(OnEntitySyncReceived);
    }

    private void OnEntityEnterReceived(EntityEnterResponse response)
    {
        foreach (var entity in response.EntityList)
        {
            var e = new NetworkEntityEnterEvent(
                entity.EntityId,
                entity.Position.ToVector3(),
                Quaternion.Euler(entity.Direction.ToVector3()));

            Logger.Info("Game", $"实体({entity.EntityId})加入: Position:{e.Position}, Rotation:{e.Rotation}");
            this.SendEvent(e);
        }
    }

    private void OnEntitySyncReceived(EntitySyncResponse response)
    {
        this.SendEvent(new NetworkEntitySyncEvent(
            response.EntitySync.Entity.EntityId,
            response.EntitySync.Entity.Position.ToVector3(),
            Quaternion.Euler(response.EntitySync.Entity.Direction.ToVector3())));
    }
}
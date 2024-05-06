using Common.Proto.Entity;
using Common.Proto.Event;
using Common.Proto.Event.Map;
using MMORPG;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool;
using Unity.VisualScripting;
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

public class NetworkEntitySyncEvent
{
    public int EntityId { get; }
    public Vector3 Postion { get; }
    public Quaternion Rotation { get; }

    public NetworkEntitySyncEvent(int entityId, Vector3 position, Quaternion rotation)
    {
        EntityId = entityId;
        Postion = position;
        Rotation = rotation;
    }
}

public interface IEntityManagerSystem : ISystem
{
    public EntityView SpawnEntity(
        EntityView prefab,
        int entityId,
        Vector3 position,
        Quaternion rotation,
        bool isMine);

    public Dictionary<int, EntityView> GetEntityDict(bool isMine);
}


public class EntityManagerSystem : AbstractSystem, IEntityManagerSystem
{
    public Dictionary<int, EntityView> _mineEntityDict { get; } = new();
    public Dictionary<int, EntityView> _notMineEntityDict { get; } = new();

    public Dictionary<int, EntityView> GetEntityDict(bool isMine)
    {
        return isMine ? _mineEntityDict : _notMineEntityDict;
    }

    public void RegisterNewEntity(EntityView entity)
    {
        Debug.Assert(
            !(_mineEntityDict.ContainsKey(entity.EntityId) ||
            _notMineEntityDict.ContainsKey(entity.EntityId)));

        if (entity.IsMine)
        {
            _mineEntityDict[entity.EntityId] = entity;
        }
        else
        {
            _notMineEntityDict[entity.EntityId] = entity;
        }
        this.SendEvent(new EntityEnterEvent(entity));
    }

    public EntityView SpawnEntity(EntityView prefab, int entityId, Vector3 position, Quaternion rotation, bool isMine)
    {
        Debug.Assert(
            !(_mineEntityDict.ContainsKey(entityId) ||
            _notMineEntityDict.ContainsKey(entityId)));

        var entity = GameObject.Instantiate(prefab, position, rotation);
        entity.transform.SetPositionAndRotation(position, rotation);

        entity.SetEntityId(entityId);
        entity.SetIsMine(isMine);

        if (entity.IsMine)
        {
            _mineEntityDict[entity.EntityId] = entity;
        }
        else
        {
            _notMineEntityDict[entity.EntityId] = entity;
        }
        Logger.Info("Game", $"实体生成成功: id:{entityId}, position:{position}, rotation:{rotation}, isMine:{isMine}");
        this.SendEvent(new EntityEnterEvent(entity));
        return entity;
    }

    protected override void OnInit()
    {
        this.GetSystem<INetworkSystem>().ReceiveEvent<EntityEnterResponse>(OnEntityEnterReceived);
        this.GetSystem<INetworkSystem>().ReceiveEvent<EntityTransformSyncResponse>(OnEntitySyncReceived);
    }

    private void OnEntityEnterReceived(EntityEnterResponse response)
    {
        foreach (var entity in response.Datas)
        {
            var e = new NetworkEntityEnterEvent(
                entity.EntityId,
                entity.Transform.Position.ToVector3(),
                Quaternion.Euler(entity.Transform.Direction.ToVector3()));

            Logger.Info("Game", $"实体({entity.EntityId})加入: Position:{e.Position}, Rotation:{e.Rotation}");
            this.SendEvent(e);
        }
    }

    private void OnEntitySyncReceived(EntityTransformSyncResponse response)
    {
        this.SendEvent(new NetworkEntitySyncEvent(
            response.EntityId,
            response.Transform.Position.ToVector3(),
            Quaternion.Euler(response.Transform.Direction.ToVector3())));
    }
}

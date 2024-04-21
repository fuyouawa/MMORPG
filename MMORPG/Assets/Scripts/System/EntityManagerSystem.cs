using Common.Proto.Entity;
using Common.Proto.Space;
using MMORPG;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool;
using UnityEngine;

public class NetworkEntityInfo
{
    public int EntityId { get; }
    public Vector3 Position { get; }
    public Quaternion Rotation { get; }

    public NetworkEntityInfo(int entityId, Vector3 position, Quaternion rotation)
    {
        EntityId = entityId;
        Position = position;
        Rotation = rotation;
    }
}

public interface IEntityManagerSystem : ISystem
{
}


public class EntityManagerSystem : AbstractSystem, IEntityManagerSystem
{
    protected override void OnInit()
    {
        this.GetSystem<INetworkSystem>().RegisterEmergencyReceive<EntityEnterResponse>(OnEntityEnterReceived);
        this.GetSystem<INetworkSystem>().RegisterEmergencyReceive<EntitySyncResponse>(OnEntitySyncReceived);
    }

    private void OnEntityEnterReceived(EntityEnterResponse response)
    {
        foreach (var entity in response.EntityList)
        {
            var info = new NetworkEntityInfo(entity.EntityId,
                    entity.Position.ToVector3(),
                    Quaternion.Euler(entity.Direction.ToVector3()));

            Logger.Info($"[Game]实体({entity.EntityId})加入: Position:{info.Position}, Rotation:{info.Rotation}");
            this.SendEvent(new NetworkEntityEnterEvent(info));
        }
    }

    private void OnEntitySyncReceived(EntitySyncResponse response)
    {
        
    }
}
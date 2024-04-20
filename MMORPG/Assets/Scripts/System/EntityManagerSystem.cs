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
            this.SendEvent(new EntityEnterEvent(
                entity.EntityId,
                entity.Position.ToVector3(),
                Quaternion.Euler(entity.Direction.ToVector3())
            ));
        }
    }

    private void OnEntitySyncReceived(EntitySyncResponse response)
    {
        
    }
}
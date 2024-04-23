using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EntitySyncEvent : EntitySyncData
{
    public Entity Entity { get; }

    public EntitySyncEvent(Entity entity, Vector3 position, Quaternion rotation) : base(position, rotation)
    {
        Entity = entity;
    }
}
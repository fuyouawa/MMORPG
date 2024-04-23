using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EntityEnterEvent
{
    public Entity Entity { get; }

    public EntityEnterEvent(Entity entity)
    {
        Entity = entity;
    }
}
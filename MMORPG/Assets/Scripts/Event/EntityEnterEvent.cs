using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//TODO 完事Event命名
public class EntityEnterEvent
{
    public Entity Entity { get; }

    public EntityEnterEvent(Entity entity)
    {
        Entity = entity;
    }
}
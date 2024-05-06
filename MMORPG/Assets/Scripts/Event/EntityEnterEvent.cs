using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EntityEnterEvent
{
    public EntityView Entity { get; }

    public EntityEnterEvent(EntityView entity)
    {
        Entity = entity;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//TODO 完事Event命名
public class EntityEnterEvent
{
    public NetworkEntity Entity { get; }

    public EntityEnterEvent(NetworkEntity entity)
    {
        Entity = entity;
    }
}
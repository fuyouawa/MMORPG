using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EntitySummonEvent
{
    public NetworkEntity Entity { get; }

    public EntitySummonEvent(NetworkEntity entity)
    {
        Entity = entity;
    }
}
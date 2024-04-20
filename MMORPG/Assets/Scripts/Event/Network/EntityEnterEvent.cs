using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EntityEnterEvent
{
    public int EntityId;
    public Vector3 Position;
    public Quaternion Rotation;

    public EntityEnterEvent(int entityId, Vector3 postion, Quaternion rotation)
    {
        EntityId = entityId;
        Position = postion;
        Rotation = rotation;
    }
}
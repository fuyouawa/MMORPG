using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerEnterEvent : EntityEnterEvent
{
    public PlayerEnterEvent(int entityId, Vector3 postion, Quaternion rotation) : base(entityId, postion, rotation)
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayer
{
    // public NetObject NetObject;

    private int _entityId;
    private bool _isMine;
        
    public float MoveSpeed;

    public int EntityId { get { return _entityId; } }
    public bool IsMine { get { return _isMine; } }

    public Vector3 Position;
    public Quaternion Rotation;

    public NetPlayer(int entityId, Vector3 position, Quaternion rotation, bool isMine)
    {
        _entityId = entityId;
        _isMine = isMine;
        Position = position;
        Rotation = rotation;
        MoveSpeed = 5;
    }
}

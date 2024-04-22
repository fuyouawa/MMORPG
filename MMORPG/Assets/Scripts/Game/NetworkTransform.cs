using Common.Proto.Event.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool;
using UnityEngine;

public class NetworkTransform : NetworkBehaviour
{
    public NetworkEntity Entity { get; private set; }
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    protected override void Awake()
    {
        base.Awake();
        Entity = GetComponent<NetworkEntity>();
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        if (Entity.IsMine)
        {
            _network.SendToServer(new EntitySyncRequest()
            {
                EntitySync = new()
                {
                    Entity = new()
                    {
                        EntityId = Entity.EntityId,
                        Position = position.ToNetVector3(),
                        Direction = rotation.eulerAngles.ToNetVector3()
                    }
                }
            });
        }
        else
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}
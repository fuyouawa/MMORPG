using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkEntity))]
public class NetworkAutoSync : NetworkBehaviour, INetworkEntityCallbacks
{
    private NetworkEntity _entity;

    protected override void Awake()
    {
        _entity = GetComponent<NetworkEntity>();
    }

    protected override void FixedUpdateNetwork(float deltaTime)
    {
        //TODO ÓÅ»¯´úÂë
        if (_entity.IsMine)
        {
            _entity.NetworkTransform.SetPositionAndRotation(transform.position, transform.rotation);
        }
    }

    public void OnNetworkSync(EntitySyncData data)
    {
        _entity.NetworkTransform.SetPositionAndRotation(data.Postion, data.Rotation);
    }
}

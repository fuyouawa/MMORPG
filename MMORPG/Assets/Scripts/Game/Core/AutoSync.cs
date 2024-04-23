using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动同步
/// </summary>
[RequireComponent(typeof(Entity))]
public class AutoSync : MonoBehaviour, INetworkEntityCallbacks
{
    void INetworkEntityCallbacks.NetworkControlFixedUpdate(NetworkControlData data)
    {
    }

    void INetworkEntityCallbacks.NetworkSyncUpdate(NetworkSyncData data)
    {
        transform.SetPositionAndRotation(data.Postion, data.Rotation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkEntityEnterEvent
{
    public NetworkEntityInfo EntityInfo { get; }

    public NetworkEntityEnterEvent(NetworkEntityInfo info)
    {
        EntityInfo = info;
    }
}

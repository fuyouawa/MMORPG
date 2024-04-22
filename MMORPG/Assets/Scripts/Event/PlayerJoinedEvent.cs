using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerJoinedEvent
{
    public NetworkEntity Player { get; }

    public PlayerJoinedEvent(NetworkEntity player)
    {
        Player = player;
    }
}
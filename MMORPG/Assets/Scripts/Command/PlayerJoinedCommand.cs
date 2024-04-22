using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerJoinedCommand : AbstractCommand
{
    NetworkEntity _player;

    public PlayerJoinedCommand(NetworkEntity player)
    {
        _player = player;
    }

    protected override void OnExecute()
    {
        this.SendEvent(new EntitySummonEvent(_player));
        this.SendEvent(new PlayerJoinedEvent(_player));
    }
}
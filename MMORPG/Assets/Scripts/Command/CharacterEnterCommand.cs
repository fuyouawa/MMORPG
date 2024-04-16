using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


public class CharacterEnterCommand : AbstractCommand
{
    private readonly NetPlayer _player;

    public CharacterEnterCommand(NetPlayer player)
    {
        _player = player;
    }

    protected override void OnExecute()
    {
        var system = this.GetSystem<SpaceSystem>();
        system.CharacterEnter(_player);
    }
}

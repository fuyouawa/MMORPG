using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterPositionChangeCommand : AbstractCommand
{
    private readonly int _entityId;
    private NetPlayer _player;
    private readonly Vector3 _newPosition;
    private readonly Quaternion _newRotation;

    public CharacterPositionChangeCommand(NetPlayer player, Vector3 newPosition, Quaternion newRotation)
    {
        _player = player;
        _newPosition = newPosition;
        _newRotation = newRotation;
    }

    public CharacterPositionChangeCommand(int entityId, Vector3 newPosition, Quaternion newRotation)
    {
        _entityId = entityId;
        _player = null;
        _newPosition = newPosition;
        _newRotation = newRotation;
    }

    protected override void OnExecute()
    {
        var system = this.GetSystem<CharacterSystem>();
        if (_player == null)
        {
            _player = system.GetPlayer(_entityId);
        }
        system.PositionChange(_player, _newPosition, _newRotation);
    }
}

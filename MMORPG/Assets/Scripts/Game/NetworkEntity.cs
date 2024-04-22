using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkEntity : NetworkBehaviour
{
    [SerializeField]
    [ReadOnly]
    private int _entityId;
    [SerializeField]
    [ReadOnly]
    private bool _isMine;

    public int EntityId => _entityId;

    public bool IsMine => _isMine;

    public void SetEntityId(int entityId)
    {
        _entityId = entityId;
    }

    public void SetIsMine(bool isMine)
    {
        _isMine = isMine;
    }
}
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[RequireComponent(typeof(NetworkTransform))]
public class NetworkEntity : NetworkBehaviour
{
    [SerializeField]
    [ReadOnly]
    private int _entityId;
    [SerializeField]
    [ReadOnly]
    private bool _isMine;

    public NetworkTransform NetworkTransform { get; private set; }

    public int EntityId => _entityId;

    public bool IsMine => _isMine;

    protected override void Awake()
    {
        base.Awake();
        NetworkTransform = GetComponent<NetworkTransform>();
    }

    public void SetEntityId(int entityId)
    {
        _entityId = entityId;
    }

    public void SetIsMine(bool isMine)
    {
        _isMine = isMine;
    }
}
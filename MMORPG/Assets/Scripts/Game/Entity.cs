using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Entity : MonoBehaviour, IController
{
    [ReadOnly]
    public int EntityId;
    [ReadOnly]
    public bool IsMine;

    private void Awake()
    {
        
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class PlayerController : MonoBehaviour, IController
{
    private EntityController _entity;

    private void Awake()
    {
        _entity = GetComponent<EntityController>();
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
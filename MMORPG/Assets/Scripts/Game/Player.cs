using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Player : MonoBehaviour, IController
{
    public Entity Entity { get; private set; }

    private void Awake()
    {
        Entity = GetComponent<Entity>();
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour, INetworkEntityCallbacks, IController
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void NetworkControlFixedUpdate(NetworkControlData data)
    {
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;

        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");

    }

    public void NetworkSyncUpdate(NetworkSyncData data)
    {
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}

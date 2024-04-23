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
    private GameConfigObject _config;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _config = this.GetModel<IConfigModel>().GameConfig;
    }

    public void NetworkControlFixedUpdate(NetworkControlData data)
    {
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;

        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");
        _animator.SetBool(_config.AnimParamWalking, Mathf.Abs(horizontalAxis) >= 0.1f || Mathf.Abs(verticalAxis) >= 0.1f);
        _animator.SetFloat(_config.AnimParamHorizontalAxis, horizontalAxis);
        _animator.SetFloat(_config.AnimParamVerticalAxis, verticalAxis);
    }

    public void NetworkSyncUpdate(NetworkSyncData data)
    {
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}

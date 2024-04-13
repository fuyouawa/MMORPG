using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayer : NetObject
{
    public float MoveSpeed = 5;
    public float RotateLerp = 0.5f;

    private Rigidbody _rigidbody;

    protected bool _isMine = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (_isMine)
        {
            ControlPlayer();
        }
    }


    protected virtual void ControlPlayer()
    {
        // 获取摄像机的前方向，即摄像机的观察方向
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f; // 忽略y轴，保持在水平方向

        // 获取输入方向
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // 如果有输入，计算移动方向，基于摄像机的前方向
        Vector3 moveDirection = Vector3.zero;
        if (inputDirection != Vector3.zero)
        {
            moveDirection = Quaternion.LookRotation(cameraForward) * inputDirection;
        }
        // 移动角色
        _rigidbody.velocity = moveDirection * MoveSpeed;

        // 如果有输入，旋转角色朝向移动方向
        if (inputDirection != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, moveDirection.normalized, RotateLerp);
        }
    }
}

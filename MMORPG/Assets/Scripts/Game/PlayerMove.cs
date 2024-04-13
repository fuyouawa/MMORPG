using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    public GameObject Player;
    public CinemachineFreeLook PlayerCamera;
    public float MoveSpeed;

    void FixedUpdate()
    {
        // 获取摄像机的前方向，即摄像机的观察方向
        Vector3 cameraForward = PlayerCamera.transform.forward;
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

        var rb = Player.GetComponent<Rigidbody>();
        // 移动角色
        rb.velocity = moveDirection * MoveSpeed;

        // 如果有输入，旋转角色朝向移动方向
        if (inputDirection != Vector3.zero)
        {
            Player.transform.forward = moveDirection.normalized;
        }
    }
}

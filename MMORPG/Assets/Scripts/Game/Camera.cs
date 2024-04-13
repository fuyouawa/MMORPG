using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var heroCamera = GetComponent<CinemachineFreeLook>();
        heroCamera.m_YAxis.m_MaxSpeed = 5;
        heroCamera.m_XAxis.m_MaxSpeed = 500;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
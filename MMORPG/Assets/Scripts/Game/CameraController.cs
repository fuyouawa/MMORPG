using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook _heroCamera;

    // Start is called before the first frame update
    void Start()
    {
        _heroCamera.m_YAxis.m_MaxSpeed = 5;
        _heroCamera.m_XAxis.m_MaxSpeed = 600;
    }

    public float GetAxisCustom(string axisName)
    {
        if (axisName == "Mouse X")
        {
            if (Input.GetMouseButton(1))
            {
                return UnityEngine.Input.GetAxis("Mouse X");
            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetMouseButton(1))
            {
                return UnityEngine.Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (_heroCamera.m_Lens.FieldOfView <= 62)
            {
                _heroCamera.m_Lens.FieldOfView += 5f;
            }
            //cinemachineFreeLook.m_Lens.OrthographicSize
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (_heroCamera.m_Lens.FieldOfView >= 4)
            {
                _heroCamera.m_Lens.FieldOfView -= 5f;
            }
        }
    }
}
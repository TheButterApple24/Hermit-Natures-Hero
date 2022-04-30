/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              CameraFollow
Description:        Handles camera following a player
Date Created:       06/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    06/10/2021
        - [Jeffrey] Implemented camera follow system
    07/11/2021
        - [Jeffrey] Implemented Lock-On system

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject CameraTarget;

    public float CameraMoveSpeed = 120.0f;
    public float ClampAngle = 80.0f;
    public float InputSensitivity = 150.0f;
    public float MouseX;
    public float MouseY;
    public float FinalInputX;
    public float FinalInputZ;

    [HideInInspector] public bool m_InvertXAxis;
    [HideInInspector] public bool m_InvertYAxis;

    [HideInInspector] public bool m_InMenu;

    [HideInInspector] public bool m_IsPlayerReady = true;
    [HideInInspector] public bool m_IsInputDisabled = false;

    public GameObject m_Target;

    public float RotationX = 0.0f;
    public float RotationY = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        RotationX = rot.x;
        RotationY = rot.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Target == null && m_InMenu == false && m_IsPlayerReady && !m_IsInputDisabled)
        {
            // Get all inputs
            float inputX = Input.GetAxis("RightStickHorizontal");
            float inputZ = -Input.GetAxis("RightStickVertical");
            MouseX = Input.GetAxis("Mouse X");
            MouseY = -Input.GetAxis("Mouse Y");

            // Clamp the mouse so it can't move too fast
            MouseX = Mathf.Clamp(MouseX, -2.0f, 2.0f);
            MouseY = Mathf.Clamp(MouseY, -2.0f, 2.0f);

            // Combine KB/M and Controller inputs
            FinalInputX = inputX + MouseX;
            FinalInputZ = inputZ + MouseY;

           // If inverted, invert each respective axis
            if (m_InvertXAxis)
            {
                FinalInputX = -FinalInputX;
            }

            if (m_InvertYAxis)
            {
                FinalInputZ = -FinalInputZ;
            }

            // Calculate X and Y rotation
            RotationY += FinalInputX * InputSensitivity * Time.deltaTime;
            RotationX += FinalInputZ * InputSensitivity * Time.deltaTime;

            // Clamp rotation
            RotationX = Mathf.Clamp(RotationX, -ClampAngle, ClampAngle);

            // Rotate camera accordignly 
            transform.localEulerAngles = new Vector3(RotationX % 360, RotationY % 360, 0);
        }
    }

    void LateUpdate()
    {
        CameraUpdater();
    }

    void CameraUpdater()
    {
        // Set target
        Transform target = CameraTarget.transform;

        // Move towards target
        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = target.position;

        if (m_Target != null)
        {
            // Look at the target while keeping player in view
            transform.LookAt(m_Target.transform, Vector3.up);
        }
    }

}

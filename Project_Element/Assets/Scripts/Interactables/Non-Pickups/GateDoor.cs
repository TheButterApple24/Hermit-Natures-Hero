/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              GateDoor
Description:        Handles interactable doors
Date Created:       09/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    09/11/2021
        - [Jeffrey] Created base class
    17/11/2021
        - [Zoe] Now calls base.Activate to hide the button prompt

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateDoor : InteractableBase
{
    public GameObject m_TargetPosition;
    public float m_Speed;
    public bool m_IsLocked;

    private bool m_IsOpening;
    private bool m_IsClosing;

    private Vector3 m_StartPos;

    public override void Start()
    {
        base.Start();
        m_StartPos = transform.position;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (m_IsLocked == false)
        {
            if (m_IsOpening)
            {
                OpenDoor();
            }

            if (m_IsClosing)
            {
                CloseDoor();
            }
        }
    }
    public override void Activate()
    {
        if (m_IsLocked == false)
        {
            m_IsOpening = true;
            m_IsClosing = false;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        if (m_IsLocked == false)
        {
            m_IsClosing = true;
            m_IsOpening = false;
        }
    }
    void OpenDoor()
    {
        // Move door towards the target location
        float step = m_Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, m_TargetPosition.transform.position, step);

        if (transform.position == m_TargetPosition.transform.position)
        {
            m_IsOpening = false;
        }
    }

    void CloseDoor()
    {
        // Move door towards the start location
        float step = m_Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, m_StartPos, step);

        if (transform.position == m_TargetPosition.transform.position)
        {
            m_IsClosing = false;
        }
    }
}

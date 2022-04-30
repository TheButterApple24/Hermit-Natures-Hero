/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SlidableDoor
Description:        Handles the movement of the Skill Temple Doors
Date Created:       11/04/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    11/04/2022
        - [Max] Created base class

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidableDoor : MonoBehaviour
{
    public Transform TargetLocation;

    bool m_OpenDoorAnimation = false;
    bool m_CloseDoorAnimation = false;
    float m_OriginalXPos;
    float m_TimeElapsed = 0.0f;
    float m_LerpDuration = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        m_OriginalXPos = transform.localPosition.x;
    }
    public void OpenDoor()
    {
        m_CloseDoorAnimation = false;
        m_OpenDoorAnimation = true;
    }

    public void CloseDoor()
    {
        m_OpenDoorAnimation = false;
        m_CloseDoorAnimation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_OpenDoorAnimation)
        {
            Vector3 pos = transform.localPosition;
            if (m_TimeElapsed < m_LerpDuration)
            {
                pos.x = Mathf.Lerp(m_OriginalXPos, TargetLocation.localPosition.x, m_TimeElapsed / m_LerpDuration);
                transform.localPosition = pos;

                m_TimeElapsed += Time.deltaTime;
            }
            else
            {
                pos.x = TargetLocation.localPosition.x;
                transform.localPosition = pos;
                m_TimeElapsed = 0.0f;

                m_OpenDoorAnimation = false;
            }
        }

        if (m_CloseDoorAnimation)
        {
            Vector3 pos = TargetLocation.localPosition;
            if (m_TimeElapsed < m_LerpDuration)
            {
                pos.x = Mathf.Lerp(TargetLocation.localPosition.x, m_OriginalXPos, m_TimeElapsed / m_LerpDuration);
                transform.localPosition = pos;

                m_TimeElapsed += Time.deltaTime;
            }
            else
            {
                pos.x = m_OriginalXPos;
                transform.localPosition = pos;
                m_TimeElapsed = 0.0f;

                m_CloseDoorAnimation = false;
            }
        }
    }
}

/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Water
Description:        Handles the water trigger
Date Created:       19/01/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/01/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ThirdPersonController>() != null)
        {

            other.gameObject.GetComponent<ThirdPersonController>().m_MovementState = MovementState.Swimming;
            other.gameObject.GetComponent<ThirdPersonController>().m_PlayerSwim.m_WaterHeight = transform.position.y;
            other.gameObject.GetComponent<ThirdPersonController>().m_RigidBody.drag = 2.0f;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<ThirdPersonController>() != null)
        {
            other.gameObject.GetComponent<ThirdPersonController>().m_MovementState = MovementState.OnGround;
            other.gameObject.GetComponent<ThirdPersonController>().m_RigidBody.drag = 0.0f;
        }
    }
}

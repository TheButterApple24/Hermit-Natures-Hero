/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              JumpUpCheck
Description:        Allows player to climb a ledge if they just miss it
Date Created:       17/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/10/2021
        - [Jeffrey] Implemented a basic jump-up system
    20/10/2021
        - [Jeffrey] Polished jump up system

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class JumpUpCheck : MonoBehaviour
{
    public ThirdPersonController m_Player;

    public float m_ClimbTimer = 0.0f;

    private Vector3 m_EndPos = Vector3.zero;

    private bool m_IsCurrentlyMoving = false;

    void Update()
    {
        // If the player is not climbing, check for ledges
        if (!m_Player.m_IsClimbing)
        {
            CheckForLedge();
        }

        // If the player is moving, move it to the target location
        if (m_IsCurrentlyMoving)
        {
            MovePlayerToPosition();
        }
    }

    void MovePlayerToPosition()
    {
        // Disable physics and move the player
        m_Player.GetComponent<CapsuleCollider>().enabled = false;
        float step = 10 * Time.deltaTime;

        m_Player.transform.position = Vector3.MoveTowards(m_Player.transform.position, m_EndPos, step);
    }

    void CheckForLedge()
    {
        // Grab players location and the target direction
        Vector3 origin = m_Player.transform.position;
        float distance = 1.0f;
        Vector3 direction = m_Player.transform.forward.normalized;

        Debug.DrawRay(origin, direction * distance);

        origin += direction * 1;
        direction = -Vector3.up;

        Debug.DrawRay(origin, direction * distance);

        // Check if player is at a ledge
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, 1.0f, m_Player.m_GroundCheckMask))
        {
            // Get the angle of the ledge
            float angle = Vector3.Angle(hit.point, hit.normal);
            if (angle > 60)
            {
                // Store start and end positions
                m_EndPos = hit.point;
                m_Player.m_RigidBody.isKinematic = true;
                m_IsCurrentlyMoving = true;

                // Set climb timer and start coroutine
                m_ClimbTimer = angle / 200;

                StartCoroutine("MovePlayerTimer");
            }
        }

    }

    IEnumerator MovePlayerTimer()
    {
        // Wait for the climb timer before re-enabling physics
        yield return new WaitForSeconds(m_ClimbTimer);
        m_Player.GetComponent<CapsuleCollider>().enabled = true;
        m_Player.m_RigidBody.isKinematic = false;
        m_IsCurrentlyMoving = false;
    }
}

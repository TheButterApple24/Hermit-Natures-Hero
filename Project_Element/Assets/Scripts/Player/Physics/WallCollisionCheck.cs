/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              WallCollisionCheck
Description:        Stops the player from getting stuck in walls
Date Created:       17/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/10/2021
        - [Jeffrey] Implemented basic wall-collision system
    18/10/2021
        - [Jeffrey] Polished basic wall-collision system

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollisionCheck : MonoBehaviour
{
    public ThirdPersonController m_Player;

    void OnTriggerEnter(Collider other)
    {
        // Check if player is collding with wall
        if (other.gameObject.layer == 6)
        {
            m_Player.m_IsCollidingWithWall = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if player is leaving the wall
        if (other.gameObject.layer == 6)
        {
            m_Player.m_IsCollidingWithWall = false;
        }
    }
}

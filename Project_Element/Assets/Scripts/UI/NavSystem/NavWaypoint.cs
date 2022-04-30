/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              NavWaypoint
Description:        Handles navigation system when doing quests
Date Created:       05/12/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    05/12/2021
        - [Jeffrey] Started base implementation.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavWaypoint : MonoBehaviour
{
    public Image m_MarkerImage;
    public Transform m_Target;
    public Camera m_Camera;
    public Player m_Player;
    private bool m_Activated = true;

    private void Update()
    {
        // When NavmarkerToggle is pressed, enable/disable nav marker 
        if (Input.GetButtonDown("NavMarkerToggle"))
        {
            m_Activated = !m_Activated;

            if (m_Activated)
            {
                m_MarkerImage.enabled = true;
            }
            else
            {
                m_MarkerImage.enabled = false;
            }
        }

        // If the nav marker is activated
        if (m_Activated)
        {
            // If the target is not null
            if (m_Target != null)
            {
                // Get the min and max x and y for the screen
                float minX = m_MarkerImage.GetPixelAdjustedRect().width / 2;
                float maxX = Screen.width - minX;

                float minY = m_MarkerImage.GetPixelAdjustedRect().height / 2;
                float maxY = Screen.height - minY;

                // Get the screen position of the target
                Vector2 pos = m_Camera.WorldToScreenPoint(m_Target.position);

                // Check if the target is behind the player, adjust the position accordignly
                if (Vector3.Dot((m_Target.position - transform.position), transform.forward) < 0)
                {
                    if (pos.x < Screen.width / 2)
                    {
                        pos.x = maxX;
                    }
                    else
                    {
                        pos.x = minX;
                    }
                }

                // Clamp the x and y position to the min and max screen dimensions
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minY, maxY);

                // Set the image position to the new position
                m_MarkerImage.transform.position = pos;
            }
        }
    }
}

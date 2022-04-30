

/*===================================================

Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              CameraCollision

Description:        Handles what happens when camera is in/behind a wall

Date Created:       06/10/2021

Author:             Jeffrey MacNab

Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    06/10/2021

        - [Jeffrey] Implemented camera collision system

    08/12/2021

        - [Max] Fixed Bug where Camera would zoom out in menus

    04/01/2022

        - [Aaron] Removed m_Player being set in Awake. Was preventing scene from playing and is already being set in the editor.

    21/02/2022

        - [Zoe] Added a check to scrollwheel zooming, so it only works if there are less than two interactables in range

 ===================================================*/

using System.Collections;

using System.Collections.Generic;

using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public float MinDistance = 1.0f;
    public float MaxDistance = 4.0f;
    public float Smooth = 10.0f;
    public float Distance;
    public Vector3 DollyDirectionAdjusted;
    public LayerMask Player;
    public Player m_Player;
    private Vector3 DollyDirection;

    // Start is called before the first frame update

    void Start()
    {
        DollyDirection = transform.localPosition.normalized;

        Distance = transform.localPosition.magnitude;

        m_Player = PlayerManager.Instance.Player;
    }

    void Update()
    {
        if (m_Player != null && !m_Player.m_IsMenuOpen && m_Player.InteractionHandler.InteractableCount < 2)
        {
            // On scroll, zoom in and out camera
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            MaxDistance -= scroll;
            MaxDistance = Mathf.Clamp(MaxDistance, 2, 10);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Find desired location
        Vector3 desiredCameraPosition = transform.parent.TransformPoint(DollyDirection * MaxDistance);
        RaycastHit hit;
        LayerMask notPlayer = ~Player;

        // If camera collides with wall, move it to a location where it can see the player
        if (Physics.Linecast(transform.parent.position, desiredCameraPosition, out hit, notPlayer))
        {
            Distance = Mathf.Clamp((hit.distance * 0.5f), MinDistance, MaxDistance);
        }
        else
        {
            Distance = MaxDistance;
        }

        // Move camera to desired position
        transform.localPosition = Vector3.Lerp(transform.localPosition, DollyDirection * Distance, Time.fixedDeltaTime * Smooth);
    }
}
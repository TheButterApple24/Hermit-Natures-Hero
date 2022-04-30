/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SittableObject
Description:        Handles objects the player can sit on
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

public class SittableObject : InteractableBase
{
    public Transform m_RootTransform;
    public Transform m_ExitTransform;
    public Collider m_MeshCollider;

    bool m_Interacted;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void Activate()
    {
        if (m_Player != null && m_Interacted == false)
        {
            m_MeshCollider.enabled = false;
            m_Interacted = true;
            //m_PlayerController.Sit(m_RootTransform);
        }
        else
        {
            m_Interacted = false;
           // m_PlayerController.Stand(m_ExitTransform);
            m_MeshCollider.enabled = true;
        }
    }
}

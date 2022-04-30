/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Billboard
Description:        Makes sprite face camera
Date Created:       08/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    08/11/2021
        - [Jeffrey] Created base class

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera m_MainCamera;
    public bool m_OnlyImpactY;

    private void Start()
    {
        m_MainCamera = Camera.main;
    }

    void Update()
    {
        // Make sprite face camera
        transform.LookAt(m_MainCamera.transform.position, -Vector3.up);

        // Make it only impact the Y rotation
        if (m_OnlyImpactY)
        {
            Quaternion yRotation = transform.rotation;
            yRotation.x = 0;
            yRotation.z = 0;
            transform.SetPositionAndRotation(transform.position, yRotation);
        }
    }
}

/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              BobbingAnimation
Description:        Makes sprite bob up and down
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

public class BobbingAnimation : MonoBehaviour
{
    public float Amplutide = 3;
    public float Speed = 3;

    float m_OriginalY;

    private void Awake()
    {
        m_OriginalY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Move up and down at a set speed and amplitude
        transform.localPosition = new Vector3(transform.localPosition.x, m_OriginalY + Mathf.Sin(Time.time * Speed) * Amplutide, transform.localPosition.z);
    }
}

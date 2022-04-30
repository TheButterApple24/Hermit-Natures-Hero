/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DepthTexture
Description:        Allows camera to create a depth texture
Date Created:       07/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    07/11/2021
        - [Jeffrey] Created base class

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DepthTexture : MonoBehaviour
{
    public Camera m_MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        m_MainCamera.depthTextureMode = DepthTextureMode.Depth;
    }
}

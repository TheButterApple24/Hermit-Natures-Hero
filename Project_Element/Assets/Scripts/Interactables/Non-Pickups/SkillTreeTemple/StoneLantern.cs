/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              StoneLantern
Description:        Handles the Stone Lantern object
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

public class StoneLantern : MonoBehaviour
{
    public Light LanternLight;
    public bool IsLightEnabled = false;
    public float ActivationRange = 15.0f;
    public Material ActivatedMat;

    bool m_LightAlreadyActive = false;
    GameObject m_PlayerTarget;

    // Start is called before the first frame update
    void Start()
    {
        // Set Player Target
        m_PlayerTarget = PlayerManager.Instance.Player.gameObject;

        if (LanternLight != null && IsLightEnabled)
        {
            LanternLight.enabled = true;
            m_LightAlreadyActive = true;
        }
    }

    public void Activate()
    {
        Renderer renderer = transform.GetComponent<Renderer>();

        // If statue's renderer + activated material exist, apply material to this object
        if (renderer != null && ActivatedMat != null)
        {
            renderer.material = ActivatedMat;
        }

        if (LanternLight != null)
        {
            LanternLight.enabled = true;

            // Set Light to Enabled/Active
            IsLightEnabled = true;
            m_LightAlreadyActive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (LanternLight != null)
        {
            // Check distance between this object and the Player
            float distance = Vector3.Distance(transform.position, m_PlayerTarget.transform.position);

            // If Player is within activation range
            if (distance < ActivationRange)
            {
                // Enable Light if not already enabled
                if (IsLightEnabled && !m_LightAlreadyActive)
                {
                    LanternLight.enabled = true;
                    m_LightAlreadyActive = true;
                }
            }
            else
            {
                LanternLight.enabled = false;
                m_LightAlreadyActive = false;
            }
        }
    }
}

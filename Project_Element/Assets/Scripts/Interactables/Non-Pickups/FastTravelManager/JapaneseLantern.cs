/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              JapaneseLantern
Description:        Handles the Japanese Lantern Objects
Date Created:       10/12/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    11/02/2022
        - [Max] Created base class
    12/02/2022
        - [Max] Created public struct for materials
    16/02/2022
        - [Max] Added Player distance check
        - [Max] Set Shrine Lantern material dynamically
    18/02/2022
        - [Max] Added PlayerTarget
    04/03/2022
        - [Jeffrey] Re-Factored menu system
    11/03/2022
        - [Max] Optimized code

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;

[System.Serializable]
public struct JapaneseLanternMatList
{
    public Material m_NeutralMat;
    public Material m_FireMat;
    public Material m_WaterMat;
    public Material m_PlantMat;
}

public class JapaneseLantern : MonoBehaviour
{
    public Element m_LanternElement = Element.None;
    public bool m_IsLightEnabled = false;
    public float m_ActivationRange = 15.0f;
    public JapaneseLanternMatList m_MaterialList;

    bool m_LightAlreadyActive = false;
    GameObject m_PlayerTarget;

    public void Start()
    {
        // Set Player Target
        m_PlayerTarget = PlayerManager.Instance.Player.gameObject;

        // Enable renderer component exists
        if (transform.GetComponent<Renderer>() != null)
        {
            // Enable Light if it exists and is enabled LOCAL
            Light light = GetComponent<Light>();

            if (light != null && m_IsLightEnabled)
            {
                light.enabled = true;
                m_LightAlreadyActive = true;
            }

            // Set Lantern Material with shrine element
            Teleporters teleporterObject = transform.parent.GetComponent<Teleporters>();

            if (teleporterObject != null)
            {
                m_LanternElement = teleporterObject.m_ShrineElement;
                SetLanternMaterial(m_LanternElement);
            }
            else
            // Set material to element if not part of a shrine
            {
                // Set with own element
                SetLanternMaterial(m_LanternElement);
            }
        }
    }

    public void Activate(Element element)
    {
        // If this object's renderer exists, apply activated material and light colour to this object based on its element
        Renderer renderer = transform.GetComponent<Renderer>();

        if (renderer != null)
        {
            // Grab light child object and enable it
            Light light = GetComponentInChildren<Light>();
            light.enabled = true;

            // Set Light to Enabled/Active
            m_IsLightEnabled = true;
            m_LightAlreadyActive = true;

            // Set Lantern Material to shrine material
            m_LanternElement = element;
            SetLanternMaterial(m_LanternElement);
        }
    }

    void Update()
    {
        // Check distance between this object and the Player
        float distance = Vector3.Distance(transform.position, m_PlayerTarget.transform.position);
        Light light = transform.GetComponentInChildren<Light>();

        // If Player is within activation range
        if (distance < m_ActivationRange)
        {
            // Enable Light if not already enabled
            if (m_IsLightEnabled && !m_LightAlreadyActive)
            {
                light.enabled = true;
                m_LightAlreadyActive = true;
            }
        }
        else
        {
            // Disable Light if not already disabled
            if (m_IsLightEnabled && m_LightAlreadyActive)
            {
                light.enabled = false;
                m_LightAlreadyActive = false;
            }
        }
    }

    public void SetLanternMaterial(Element element)
    {
        Renderer renderer = transform.GetComponent<Renderer>();
        Light light = transform.GetComponentInChildren<Light>();

        switch (element)
        {
            case Element.None:
                renderer.material = m_MaterialList.m_NeutralMat;
                light.color = new Color(1.0f, 0.0f, 0.0f);
                break;
            case Element.Fire:
                renderer.material = m_MaterialList.m_FireMat;
                light.color = new Color(1.0f, 0.0f, 0.0f);
                break;
            case Element.Water:
                renderer.material = m_MaterialList.m_WaterMat;
                light.color = new Color(0.0f, 0.43f, 1.0f);
                break;
            case Element.Plant:
                renderer.material = m_MaterialList.m_PlantMat;
                light.color = new Color(0.15f, 1.0f, 0.0f);
                break;
            default:
                renderer.material = m_MaterialList.m_NeutralMat;
                light.color = new Color(1.0f, 0.0f, 0.0f);
                break;
        }
    }
}

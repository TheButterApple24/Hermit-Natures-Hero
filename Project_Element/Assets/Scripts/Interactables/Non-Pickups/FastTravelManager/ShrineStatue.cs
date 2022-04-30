/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              ShrineStatue
Description:        Handles the Shrine Statue Objects
Date Created:       10/02/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/02/2022
        - [Max] Created base class
    12/02/2022
        - [Max] Added Particle Effect
        - [Max] Created public struct for materials
    16/02/2022
        - [Max] Added Player distance check
    18/02/2022
        - [Max] Added PlayerTarget
    04/03/2022
        - [Jeffrey] Re-Factored menu system

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;

[System.Serializable]
public struct ShrineStatueMatList
{
    public Material NeutralMat;
    public Material ActivatedMat;
    public Material GhostMat;
}

    public class ShrineStatue : MonoBehaviour
{
    public Element StatueElement = Element.None;
    public bool IsDestroyed;
    public bool IsLightEnabled = false;
    public bool AreParticlesEnabled = false;
    public float ActivationRange = 15.0f;
    public ShrineStatueMatList MaterialList;
    public Light StatueLight;
    public ParticleSystem ParticleEffect;

    bool m_LightAlreadyActive = false;
    bool m_ParticlesAlreadyActive = false;
    GameObject m_PlayerTarget;

    public void Start()
    {
        // Set Player Target
        m_PlayerTarget = PlayerManager.Instance.Player.gameObject;

        // Set Light
        if (StatueLight == null)
        {
            StatueLight = transform.GetComponentInChildren<Light>();
        }

        // Set Particle Effect
        if (ParticleEffect)
        {
            ParticleEffect = transform.GetComponentInChildren<ParticleSystem>();
        }

        if (StatueLight != null && IsLightEnabled)
        {
            StatueLight.enabled = true;
            SetLightColor(StatueLight);
            m_LightAlreadyActive = true;
        }

        // Enable Particles if enabled
        if (ParticleEffect != null && AreParticlesEnabled)
        {
            ParticleEffect.Stop();
            ParticleEffect.Play();
            m_ParticlesAlreadyActive = true;
        }
    }

    public void Activate(bool isShrineElementActive)
    {
        Renderer renderer = transform.GetComponent<Renderer>();

        // If statue's renderer + activated material exist, apply material to this object
        if (renderer != null)
        {
            // If statue isn't destroyed
            if (!IsDestroyed)
            {
                // If Shrine has an element
                if (isShrineElementActive)
                {
                    if (StatueLight != null)
                    {
                        // Grab light child object and enable it
                        StatueLight.enabled = true;

                        // Set light to appropriate colour
                        SetLightColor(StatueLight);

                        // Set Light to Enabled/Active
                        IsLightEnabled = true;
                        m_LightAlreadyActive = true;
                    }

                    // Apply activated material based on Element
                    renderer.material = MaterialList.ActivatedMat;

                    // Start Particle Effect and set Particles to Enabled/Active
                    if (ParticleEffect != null)
                    {
                        ParticleEffect.Stop();
                        ParticleEffect.Play();

                        AreParticlesEnabled = true;
                        m_ParticlesAlreadyActive = true;
                    }
                }
                else
                {
                    renderer.material = MaterialList.NeutralMat;
                }
            }
            else
            {
                // Apply Ghost Material to Statue
                renderer.material = MaterialList.GhostMat;
            }
        }
    }

    void Update()
    {
        // Check distance between this object and the Player
        float distance = Vector3.Distance(transform.position, m_PlayerTarget.transform.position);

        // If Player is within activation range
        if (distance < ActivationRange)
        {
            // Enable Light if not already enabled
            if (IsLightEnabled && !m_LightAlreadyActive)
            {
                GetComponentInChildren<Light>().enabled = true;
                m_LightAlreadyActive = true;
            }

            // Enable Particles if not already enabled
            if (AreParticlesEnabled && !m_ParticlesAlreadyActive)
            {
                ParticleEffect.Stop();
                ParticleEffect.Play();
                m_ParticlesAlreadyActive = true;
            }
        }
        else
        {
            // Disable Light if not already disabled
            if (IsLightEnabled && m_LightAlreadyActive)
            {
                GetComponentInChildren<Light>().enabled = false;
                m_LightAlreadyActive = false;
            }

            // Disable Particles if not already disabled
            if (AreParticlesEnabled && m_ParticlesAlreadyActive)
            {
                ParticleEffect.Stop();
                m_ParticlesAlreadyActive = false;
            }
        }
    }

    public void SetLightColor(Light light)
    {
        // Set light to appropriate colour
        switch (StatueElement)
        {
            case Element.Fire:
                light.color = new Color(1.0f, 0.48f, 0.0f);
                break;
            case Element.Water:
                light.color = new Color(0.0f, 0.43f, 1.0f);
                break;
            case Element.Plant:
                light.color = new Color(0.15f, 1.0f, 0.0f);
                break;
            case Element.None:
                break;
            default:
                break;
        }
    }
}

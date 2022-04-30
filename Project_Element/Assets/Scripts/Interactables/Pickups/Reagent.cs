/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Reagent
Description:        Handles armor item
Date Created:       17/10/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/10/2021
        - [Max] Started Plant.
    18/10/2021
        - [Max] Set m_IsPickupable to false.
    19/10/2021
        - [Max] Integrated Pickups class (old Item class)
    30/10/2021
        - [Max] Added AddToInventory functionality (Missing Inventory System)
    07/11/2021
        - [Max] Implemented Inventory implementation
    08/11/2021
        - [Max] Called base.Activate. Removed m_IsPickupable
    17/11/2021
        - [Zoe] Removed the base.Start, and Awake and Update since it was empty
    25/01/2022
        - [Zoe] Created new version of Reagents
    26/01/2022
        - [Zoe] Added HarvestableObject, harvesting cooldowns, and harvesting animation
    17/02/2022
        - [Zoe] Adjusted Activate functionality to handle the new loot-specific reagents
        - [Zoe] Fixed an issue that cause the interaction particles to still show after a reagent was picked

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PickupTypes;
using ReagentTypes;

public class Reagent : Pickups
{
    public ReagentType m_PlantType;
    public float m_RespawnCooldown = 2.0f;
    public bool m_IsPlanted = true;

    bool m_IsHarvested = false;
    GameObject m_HarvestedObject;
    float m_AnimationTimer = 0.0f;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        // Set this type of pickup to Plant
        m_PickupType = PickupType.Reagent;

        if (m_IsPlanted)
        {
            m_HarvestedObject = transform.Find("HarvestedObject").gameObject;
        }
    }

    public override void Update()
    {
        base.Update();

        if (!m_IsHarvested && m_IsPlanted)
        {
            if (m_HarvestedObject.transform.localScale.x != 1)
            {
                m_AnimationTimer += 2 * Time.deltaTime;
                m_HarvestedObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, m_AnimationTimer);
            }
        }
    }

    public override void Activate()
    {
		base.Activate();
		
        if (!m_IsHarvested)
        {
            // If the reagent is planted in the ground (not the picked/loot version)
            if (m_IsPlanted)
            {
                // "Pick" the reagent by hiding the harvestable section of the mesh
                m_HarvestedObject.transform.localScale = new Vector3(0, 0, 0);
                m_IsHarvested = true;
                m_IsInteractable = false;
                m_AnimationTimer = 0.0f;

                // Trigger the respawn timer
                StartCoroutine("RespawnCooldown");
            }

            // If this was the target object of the player, stop the interaction particles and remove it from the list
            if (m_Player.InteractionHandler.TargetObject == this)
            {
                m_InteractionParticles.Stop();
                m_Player.InteractionHandler.Remove(this);
            }

            // If Inventory is valid, add the reagent
            if (m_Player.m_Inventory != null)
            {
                m_Player.m_Inventory.AddToInventory(this);
                
                // Destroy the picked/loot version that's lying on the ground
                if (!m_IsPlanted)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    IEnumerator RespawnCooldown()
    {
        yield return new WaitForSeconds(m_RespawnCooldown);
        m_IsHarvested = false;
        m_IsInteractable = true;

        // If the player was in range when the reagent respawn, reactive the particles
        if (m_Player.InteractionHandler.TargetObject == this)
        {
            m_InteractionParticles.Play();
        }
    }
}

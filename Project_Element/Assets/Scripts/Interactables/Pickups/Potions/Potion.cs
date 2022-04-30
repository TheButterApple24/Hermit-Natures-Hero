/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Potion
Description:        Handles potion item
Date Created:       17/10/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/10/2021
        - [Max] Started Potion.
    18/10/2021
        - [Max] Set m_IsPickupable to false. Made ActivateEffect function public.
    19/10/2021
        - [Max] Integrated Pickups class (old Item class)
    30/10/2021
        - [Max] Added AddToInventory functionality (Missing Inventory System)
    07/11/2021
        - [Max] Implemented Inventory implementation
    08/11/2021
        - [Max] Called base.Activate. Removed m_IsPickupable
    17/11/2021
        - [Zoe] Removed the base.Start and Update
        - [Zoe] Split up switch statement across child classes
        - [Zoe] Moved to Potions folder
    30/11/2021
        - [Zoe] Made class abstract
        - [Zoe] Removed temporary HidePotion function
    08/12/2021
        - [Zoe] Edited an error in a debug log
    01/02/2022
        - [Zoe] Added HUDTimerIcon
        - [Zoe] Icons scale proportionally to the remaining time in the cooldown
        - [Zoe] Made ActivateEffect pure virtual

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PickupTypes;
using PotionTypes;

public abstract class Potion : Pickups
{
    public PotionType m_PotionType;
    public float m_EffectDuration = 10.0f;

    [HideInInspector] public bool m_Activated = false;

    protected float m_OriginalValue;
    protected float m_TimeRemaining;
    public Image m_HUDTimerIcon;

    public abstract void ActivateEffect();
    public abstract void InitPotion();

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        // Set this type of pickup to Potion
        m_PickupType = PickupType.Potion;
        m_TimeRemaining = m_EffectDuration;
    }

    public override void Update()
    {
        base.Update();

        if (m_Activated)
        {
            if (m_HUDTimerIcon != null)
            {
                // Scale the icon over time, relative to the time remaining of the potion effect
                m_HUDTimerIcon.fillAmount = m_TimeRemaining / m_EffectDuration;
                m_TimeRemaining -= Time.deltaTime;
            }
        }
    }

    public override void Activate()
    {
        // Call Pickups Activate()
        base.Activate();

        // If Inventory is valid
        if (m_Player.m_Inventory != null)
        {
            m_Player.m_Inventory.AddToInventory(this);
        }
    }

    protected virtual IEnumerator StartPotionTimer()
    {
        return null;
    }
}

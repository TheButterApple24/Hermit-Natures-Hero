/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                 PlantPassive
Description:        Gives the Player a small burst of healing every few seconds
Date Created:      14/11/2021
Author:               Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    14/11/2021
        - [Zoe] Added the Plant Passive
    20/01/2022
        - [Max] Added False parameter to ModifyHealth function (NO CRIT)
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPassive : PassiveAbility
{
    public float m_HealDelay = 2.0f;
    public bool m_HealTriggered = false;

    public void Update()
    {
        // If the Player has a Health Component, and their Element currently matches the ability
        //This checks if the Player's current active pet is the plant pet
        if (m_PlayerChar.m_HealthComp && m_PlayerChar.m_Element == AbilityElementType)
        {
            //If the Player is not at full health, Activate the passive's heal
            if (m_PlayerChar.m_HealthComp.m_CurrentHealth < m_PlayerChar.m_HealthComp.MaxHealth)
            {
                ActivateAbility();
            }
        }
    }
    public override void ActivateAbility()
    {
        // If the Player exists, and the healing hasn't already been triggered, start the Coroutine
        // This ensures that the Coroutine isn't triggered more than once
        if (m_PlayerChar != null && !m_HealTriggered)
        {
            m_HealTriggered = true;
            StartCoroutine("BurstHeal");
        }
    }

    public override void DeactivateAbility()
    {
        // If the Player exists, Stop the healing.
        if (m_PlayerChar != null)
        {
            m_HealTriggered = false;
            StopCoroutine("BurstHeal");
        }
    }

    IEnumerator BurstHeal()
    {
        // If the Health Component exists
        if (m_PlayerChar.m_HealthComp != null)
        {
            // While the Player is still missing health, wait a certain amount of time before applying the heal, then repeat.
            while (m_PlayerChar.m_HealthComp.m_CurrentHealth < m_PlayerChar.m_HealthComp.MaxHealth)
            {
                yield return new WaitForSeconds(m_HealDelay);
                m_PlayerChar.m_HealthComp.ModifyHealth(ChangeAmount, false);
            }

            // When the player is now full health, turn off the healing
            m_HealTriggered = false;
        }
    }
}

/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  DotEffect
Description:         Damage over time effect that will apply an amount of damage to a health component over the specific time before removing itself from the gameobject.
Date Created:       18/11/2021
Author:                Aaron Wilson
Verified By:            Jeffrey MacNab [13/04/2022]

Changelog:

    18/11/2021
        - [Aaron] Set up the variables and functions for duration and amount to be applied to a character.
	20/01/2022
        - [Max] Added False parameter to ModifyHealth function (NO CRIT)
    26/01/2022
        - [Aaron] Removed the old logic from update and created a new reset variable for the DOT effect to apply damage every second.
    30/01/2022
        - [Aaron] Change InitEffect from public to private and call it at the start of the script. The cooldown still has the same affect, but no longer needs to be called outside of the class.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotEffect : Effect
{
    float m_Reset = 1.0f;

    // Reduces the attached character's health every second for the duration of the effect's life
    private void FixedUpdate()
    {
        // Run DoT logic once the effect has been set to active
        if (IsActive == true)
        {
            m_Reset -= Time.fixedDeltaTime;

            if (AffectedCharacter)
            {
                // Reduce the affected character's health by the effect's amount (damage)
                if (m_Reset <= 0.0f)
                {
                    m_Reset = 1.0f;
                    AffectedCharacter.m_HealthComp.ModifyHealth(-EffectAmount, false);
                }
            }
        }
    }
}

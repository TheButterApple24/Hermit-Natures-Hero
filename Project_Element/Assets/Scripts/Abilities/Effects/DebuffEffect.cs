/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                 Debuff Effect
Description:        Temporaliy lowers a desired variable on the character this script is attached to
Date Created:      30/01/2022
Author:               Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    30/01/2022
        - [Aaron] Set up the variables and functions for duration and amount to be applied to a character.
    30/01/2022
        - [Aaron] Set up the reduce damage and reset damage functions, along with the reset. Set up the logic in the start after getting the necessary varaibles
    08/02/2022
        - [Aaron] Set up the reduce defense and reset defense functions. Works similar to reduce damage functionality.
    09/03/2022
        - [Max] Weapon class refactor
    24/03/2022
        - [Aaron] Added a null check to the reduce functions incase an enemy doesn't have weapons or armour.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffEffect : Effect
{
    float m_OriginalAttackValue;
    float m_OriginalArmorValue;

    protected override void Awake()
    {
        base.Awake();

        // Set the script's variables using the character's CharacterBase 
        if (m_AffectedCharacter.m_MainWeapon == null)
            return;

        m_OriginalAttackValue = m_AffectedCharacter.m_MainWeapon.GetBaseDamage();

        if (m_AffectedCharacter.m_MainArmor == null)
            return;

        m_OriginalArmorValue = m_AffectedCharacter.m_MainArmor.m_BaseDefense;
    }

    // Sets the character's weapon damage to half of the original value
    public void ReduceDamage()
    {
        if (m_AffectedCharacter.m_MainWeapon == null)
            return;

        m_AffectedCharacter.m_MainWeapon.SetBaseDamage(m_OriginalAttackValue * 0.5f);
        StartCoroutine(ResetDamageTimer());
    }

    // Sets the character's amor defense to half of the original value
    public void ReduceDefense()
    {
        if (m_AffectedCharacter.m_MainArmor == null)
            return;

        m_AffectedCharacter.m_MainArmor.m_BaseDefense = m_OriginalArmorValue * 0.5f;
        StartCoroutine(ResetDefenseTimer());
    }

    // Sets the affected character's weapon damage back to the original value
    void ResetDamage()
    {
        m_AffectedCharacter.m_MainWeapon.SetBaseDamage(m_OriginalAttackValue);
    }

    // Sets the affected character's armor defense back to the original value
    void ResetDefense()
    {
        m_AffectedCharacter.m_MainArmor.m_BaseDefense = m_OriginalArmorValue;
    }

    // Waits the specified time before resetting the damage to the original value and destroying this script
    IEnumerator ResetDamageTimer()
    {
        yield return new WaitForSeconds(EffectDuration);

        ResetDamage();
        Destroy(this);
    }

    // Waits the specified time before resetting the defense to the original value and destroying this script
    IEnumerator ResetDefenseTimer()
    {
        yield return new WaitForSeconds(EffectDuration);

        ResetDefense();
        Destroy(this);
    }
}

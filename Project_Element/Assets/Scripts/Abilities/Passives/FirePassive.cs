/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              FirePassive
Description:        Change the Weapon's Attack Speed to be faster while activate
Date Created:       02/11/2021
Author:             Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    02/11/2021
        - [Aaron] Initialized default properties to specific ones. Set up the framework for the functions
    04/11/2021
        - [Aaron] Added the Modify Speed functions and a new float to track an additional property being modified.
        - [Aaron] Implemented value changes.
        - [Aaron] Changed class name from FuryoftheFlame to FirePassive
    08/11/2021
        - [Aaron] Changed the access to the Weapon variable and removed the initialization from Awake. Weapon will be set in the editor.
    16/11/2021
        - [Max] Set MainWeapon's attack speed to Player's Weapon in Awake
    09/03/2022
        - [Max] Renamed Weapon class public variables

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;

public class FirePassive : PassiveAbility
{
    protected override void Start()
    {
        base.Start();

        if (m_PlayerChar.m_MainWeapon != null)
        {
            // Initialize the original value from the weapon
            m_OriginalValue = m_PlayerChar.m_MainWeapon.BaseAttackSpeed;
        }
    }

    public override void ActivateAbility()
    {
        // If Main Weapon exists
        if (m_PlayerChar.m_MainWeapon != null)
        {
            // Modify the original value by the change amount
            float ModifiedValue = m_PlayerChar.m_MainWeapon.BaseAttackSpeed / ChangeAmount;

            // Change the Weapon's Attack Speed
            m_PlayerChar.m_MainWeapon.SetFinalAttackSpeed(ModifiedValue);
        }
    }

    public override void DeactivateAbility()
    {
        // If Main Weapon exists
        if (m_PlayerChar.m_MainWeapon != null)
        {
            // Reset the Weapon's Attack Speed to original value
            m_PlayerChar.m_MainWeapon.SetFinalAttackSpeed(m_PlayerChar.m_MainWeapon.BaseAttackSpeed);
        }
    }
}

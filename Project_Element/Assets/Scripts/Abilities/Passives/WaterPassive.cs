/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              WaterPassive
Description:        Change the Player's Speed variables to be faster while activate
Date Created:       02/11/2021
Author:             Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    02/11/2021
    - [Aaron] Initialized default properties to specific ones. Set up the framework for the functions
    04/11/2021
    - [Aaron] Added the Modify Speed functions and a new float to track an additional property being modified.
    - [Aaron] Implemented value changes.
    - [Aaron] Changed class name from LandFins to WaterPassive
    08/11/2021
    - [Aaron] Removed the initialization of the Player from Awake. Will be set in the editor

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;

public class WaterPassive : PassiveAbility
{
    float m_OriginalSprint;

    protected override void Start()
    {
        base.Start();

        if (m_PlayerChar != null)
        {
            // Set the Original value for the walk/default speed
            m_OriginalValue = m_PlayerChar.GetComponent<ThirdPersonController>().m_DefaultSpeed;

            // Set the Original value for the sprint speed
            m_OriginalSprint = m_PlayerChar.GetComponent<ThirdPersonController>().m_SprintSpeed;
        }
    }

    // Initial the passive ability and modifies the target's original speed variables
    public override void ActivateAbility()
    {
        ModifyWalkSpeed();
        ModifySprintSpeed();
    }

    //  Return the target variables to their original values
    public override void DeactivateAbility()
    {
        ResetSpeed();
    }

    void ModifyWalkSpeed()
    {
        // Calculate the desired change to the original value
        float ModifiedValue = m_OriginalValue * ChangeAmount;

        // Set the Player's speed to the modified value
        m_PlayerChar.GetComponent<ThirdPersonController>().m_DefaultSpeed = ModifiedValue;
    }

    void ModifySprintSpeed()
    {
        // Calculate the desired change to the original value
        float ModifiedValue = m_OriginalSprint * ChangeAmount;

        // Set the Player's speed to the modified value
        m_PlayerChar.GetComponent<ThirdPersonController>().m_SprintSpeed = ModifiedValue;
    }

    void ResetSpeed()
    {
        // Reset the Player's speed to the original value
        m_PlayerChar.GetComponent<ThirdPersonController>().m_DefaultSpeed = m_OriginalValue;

        // Reset the Player's Sprint Speed to the original value
        m_PlayerChar.GetComponent<ThirdPersonController>().m_SprintSpeed = m_OriginalSprint;
    }
}

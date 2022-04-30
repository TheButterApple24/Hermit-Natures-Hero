/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              StaminaComponent
Description:        Component that handles all stamina draining and regeneration
Date Created:       06/11/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    06/11/2021
        - [Zoe] Started StaminaComponent
    08/11/2021
        - [Zoe] Basic Stamina functionality and Regeneration implemented
        - [Zoe] Stamina functionality hooked up to Player movement, logic overhauled to fix issues with this implementation
    17/11/2021
        - [Zoe] Adjusted some default values so they're more balanced (Draining up, regen delay down)
        - [Zoe] StaminaComponent now properly utilizes Mathf.Clamp

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaComponent : MonoBehaviour
{
    [SkillTreeModifiable] public float MaxStamina = 100.0f;

    public float m_StaminaRegenRate = 20.0f;
    public float m_RegenDelay = 1.0f;

    public float m_SprintDrainRate = 10.0f;
    public float m_ClimbDrainRate = 10.0f;
    public float m_DodgeDrainRate = 30.0f;

    public ThirdPersonController m_PlayerController;
    public Image m_StaminaMeter;

    [HideInInspector] public float m_CurrentStamina;
    [HideInInspector] public bool m_CanRegen = false;
    [HideInInspector] public bool m_DelayTriggered = false;
    
    // Start is called before the first frame update
    void Start()
    {
        m_CurrentStamina = MaxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_PlayerController)
        {

            //Is the Player Climbing
            if (m_PlayerController.m_IsClimbing)
            {
                DrainStamina(m_ClimbDrainRate * Time.deltaTime);
            }

            //Is the player not sprinting, dodging, or climbing (Not performing actions that deplete Stamina)
            if (!m_PlayerController.m_IsCurrentlySprinting && !m_PlayerController.m_HasDodged && !m_PlayerController.m_IsClimbing)
            {
                //Is there Stamina missing
                if (m_CurrentStamina < MaxStamina)
                {
                    //If the Stamina Regen Delay hasn't already been triggered, trigger it.
                    //Bool prevents the Coroutine from activating multiple times.
                    if (!m_DelayTriggered)
                    {
                        StartCoroutine("RegenDelay");
                        m_DelayTriggered = true;
                    }
                    //If the delay is triggered, and Stamina can be regenerating, then initiate it.
                    else if (m_CanRegen)
                    {
                        RegenStamina();
                    }
                }
            }
        }
    }

    public void DrainStamina(float value)
    {
        //Apply Damage
        m_CurrentStamina -= value;
        m_StaminaMeter.fillAmount = m_CurrentStamina / MaxStamina;

        //Turn off regeneration if it was in progress
        m_CanRegen = false;

        //Clamp the Stamina so it doesn't go below 0
        m_CurrentStamina = Mathf.Clamp(m_CurrentStamina, 0, MaxStamina);

        if (m_CurrentStamina <= 0)
        {
            m_PlayerController.DeactivateSprint();
        }
    }

    public void Reset()
    {
        m_CurrentStamina = MaxStamina;
    }

    IEnumerator RegenDelay()
    {
        //Wait a certain amount of time before initiating Stamina regeneration
        yield return new WaitForSeconds(m_RegenDelay);

        m_CanRegen = true;
        m_DelayTriggered = false;
    }

    void RegenStamina()
    {
        m_CurrentStamina += m_StaminaRegenRate * Time.deltaTime;
        m_StaminaMeter.fillAmount = m_CurrentStamina / MaxStamina;

        //Clamp the Stamina so it doesn't go above the maximum
        m_CurrentStamina = Mathf.Clamp(m_CurrentStamina, 0, MaxStamina);

        //Once Stamina is full, turn off regeneration
        if (m_CurrentStamina > MaxStamina)
        {
            m_CanRegen = false;
        }
    }
}
